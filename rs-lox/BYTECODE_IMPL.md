# Rust Implementation of Lox Bytecode

This document describes the Rust implementation of the [language-agnostic bytecode specification](../docs/BYTECODE_SPEC.md).

## Architecture Overview

```
┌──────────────┐
│ Source Code  │
└──────┬───────┘
       │
       v
┌──────────────┐
│   Scanner    │  (Done ✓)
└──────┬───────┘
       │
       v
┌──────────────┐
│   Compiler   │  Produces bytecode per spec
└──────┬───────┘
       │
       v
┌──────────────┐
│   Bytecode   │  Binary format (spec-compliant)
│    File      │  Can be saved/loaded/transmitted
└──────┬───────┘
       │
       v
┌──────────────┐
│      VM      │  Executes bytecode
└──────────────┘
```

**Key Point:** The bytecode format in the middle is VM-independent. We could write a C VM, Python VM, etc. to execute the same bytecode files.

---

## Module Structure

```
rs-lox/
├── src/
│   ├── scanner/         # Done ✓
│   ├── bytecode/        # New - Bytecode format types
│   │   ├── mod.rs
│   │   ├── opcode.rs    # OpCode enum
│   │   ├── value.rs     # Value enum
│   │   ├── chunk.rs     # Bytecode container
│   │   ├── writer.rs    # Serialize to binary
│   │   └── reader.rs    # Deserialize from binary
│   ├── compiler/        # New - Emit bytecode
│   │   ├── mod.rs
│   │   ├── emitter.rs
│   │   └── codegen.rs
│   └── vm/              # Future - Execute bytecode
│       ├── mod.rs
│       ├── stack.rs
│       └── executor.rs
```

---

## Implementation: Bytecode Module

### OpCode (bytecode/opcode.rs)

Maps directly to spec:

```rust
#[repr(u8)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum OpCode {
    // Stack operations
    Constant = 0x01,
    Nil = 0x02,
    True = 0x03,
    False = 0x04,
    Pop = 0x05,

    // Arithmetic
    Add = 0x10,
    Subtract = 0x11,
    Multiply = 0x12,
    Divide = 0x13,
    Negate = 0x14,

    // Comparison
    Equal = 0x20,
    Greater = 0x21,
    Less = 0x22,
    Not = 0x23,

    // Variables
    GetLocal = 0x30,
    SetLocal = 0x31,
    GetGlobal = 0x32,
    DefineGlobal = 0x33,
    SetGlobal = 0x34,
    GetUpvalue = 0x35,
    SetUpvalue = 0x36,

    // Control flow
    Jump = 0x40,
    JumpIfFalse = 0x41,
    Loop = 0x42,

    // Functions
    Call = 0x50,
    Closure = 0x51,
    CloseUpvalue = 0x52,
    Return = 0x53,

    // Objects
    GetProperty = 0x60,
    SetProperty = 0x61,
    Class = 0x62,
    Method = 0x63,
    Inherit = 0x64,
    GetSuper = 0x65,
    Invoke = 0x66,
    SuperInvoke = 0x67,

    // Debug
    Print = 0xF0,
}

impl OpCode {
    pub fn from_byte(byte: u8) -> Option<Self> {
        // Safe conversion from byte
        match byte {
            0x01 => Some(Self::Constant),
            0x02 => Some(Self::Nil),
            // ... all opcodes
            _ => None,
        }
    }
}
```

### Value (bytecode/value.rs)

Runtime value representation:

```rust
#[derive(Debug, Clone, PartialEq)]
pub enum Value {
    Nil,
    Bool(bool),
    Number(f64),
    String(Rc<String>),
    Function(Rc<Function>),
    // ... others as needed at runtime
}

impl Value {
    /// Encode value for constant pool (per spec)
    pub fn encode(&self) -> Vec<u8> {
        match self {
            Value::Nil => vec![0x01],
            Value::Bool(b) => vec![0x02, if *b { 0x01 } else { 0x00 }],
            Value::Number(n) => {
                let mut bytes = vec![0x03];
                bytes.extend_from_slice(&n.to_bits().to_be_bytes());
                bytes
            }
            Value::String(s) => {
                let mut bytes = vec![0x04];
                let len = s.len() as u32;
                bytes.extend_from_slice(&len.to_be_bytes());
                bytes.extend_from_slice(s.as_bytes());
                bytes
            }
            // ... others
        }
    }

    /// Decode value from constant pool (per spec)
    pub fn decode(bytes: &[u8]) -> Result<(Self, usize), DecodeError> {
        match bytes[0] {
            0x01 => Ok((Value::Nil, 1)),
            0x02 => Ok((Value::Bool(bytes[1] != 0), 2)),
            0x03 => {
                let bits = u64::from_be_bytes(bytes[1..9].try_into()?);
                Ok((Value::Number(f64::from_bits(bits)), 9))
            }
            0x04 => {
                let len = u32::from_be_bytes(bytes[1..5].try_into()?) as usize;
                let s = String::from_utf8(bytes[5..5+len].to_vec())?;
                Ok((Value::String(Rc::new(s)), 5 + len))
            }
            _ => Err(DecodeError::InvalidTag),
        }
    }
}
```

### Chunk (bytecode/chunk.rs)

In-memory representation while compiling:

```rust
#[derive(Debug, Clone)]
pub struct Chunk {
    pub code: Vec<u8>,
    pub constants: Vec<Value>,
    pub lines: LineInfo,
}

impl Chunk {
    pub fn new() -> Self {
        Self {
            code: Vec::new(),
            constants: Vec::new(),
            lines: LineInfo::new(),
        }
    }

    pub fn write(&mut self, byte: u8, line: usize) {
        self.code.push(byte);
        self.lines.add(line);
    }

    pub fn add_constant(&mut self, value: Value) -> u16 {
        self.constants.push(value);
        (self.constants.len() - 1) as u16
    }

    pub fn emit_constant(&mut self, value: Value, line: usize) {
        let idx = self.add_constant(value);
        self.write(OpCode::Constant as u8, line);
        self.write_u16(idx, line);
    }

    fn write_u16(&mut self, value: u16, line: usize) {
        let bytes = value.to_be_bytes();
        self.write(bytes[0], line);
        self.write(bytes[1], line);
    }
}

#[derive(Debug, Clone)]
pub struct LineInfo {
    entries: Vec<(usize, usize)>,  // (line, count)
}

impl LineInfo {
    pub fn new() -> Self {
        Self { entries: Vec::new() }
    }

    pub fn add(&mut self, line: usize) {
        if let Some((last_line, count)) = self.entries.last_mut() {
            if *last_line == line {
                *count += 1;
                return;
            }
        }
        self.entries.push((line, 1));
    }

    pub fn get_line(&self, offset: usize) -> Option<usize> {
        let mut current = 0;
        for (line, count) in &self.entries {
            current += count;
            if offset < current {
                return Some(*line);
            }
        }
        None
    }
}
```

### Binary Writer (bytecode/writer.rs)

Serialize to spec-compliant binary:

```rust
pub struct BytecodeWriter;

impl BytecodeWriter {
    /// Write bytecode to binary format per spec
    pub fn write(chunk: &Chunk) -> Vec<u8> {
        let mut bytes = Vec::new();

        // Magic number: "LOXB"
        bytes.extend_from_slice(&[0x4C, 0x4F, 0x58, 0x42]);

        // Version: 1.0
        bytes.extend_from_slice(&[0x01, 0x00]);

        // Constant pool
        Self::write_constant_pool(&mut bytes, &chunk.constants);

        // Code section
        Self::write_code_section(&mut bytes, &chunk.code);

        // Line info
        Self::write_line_info(&mut bytes, &chunk.lines);

        bytes
    }

    fn write_constant_pool(bytes: &mut Vec<u8>, constants: &[Value]) {
        // Size (u32)
        bytes.extend_from_slice(&(constants.len() as u32).to_be_bytes());

        // Each constant
        for constant in constants {
            bytes.extend_from_slice(&constant.encode());
        }
    }

    fn write_code_section(bytes: &mut Vec<u8>, code: &[u8]) {
        // Size (u32)
        bytes.extend_from_slice(&(code.len() as u32).to_be_bytes());

        // Code bytes
        bytes.extend_from_slice(code);
    }

    fn write_line_info(bytes: &mut Vec<u8>, lines: &LineInfo) {
        // Entry count (u32)
        bytes.extend_from_slice(&(lines.entries.len() as u32).to_be_bytes());

        // Each entry: (line u32, count u32)
        for (line, count) in &lines.entries {
            bytes.extend_from_slice(&(*line as u32).to_be_bytes());
            bytes.extend_from_slice(&(*count as u32).to_be_bytes());
        }
    }
}
```

### Binary Reader (bytecode/reader.rs)

Deserialize spec-compliant binary:

```rust
pub struct BytecodeReader;

impl BytecodeReader {
    pub fn read(bytes: &[u8]) -> Result<Chunk, ReadError> {
        let mut cursor = 0;

        // Verify magic number
        if &bytes[0..4] != b"LOXB" {
            return Err(ReadError::InvalidMagic);
        }
        cursor += 4;

        // Check version
        let major = bytes[cursor];
        let minor = bytes[cursor + 1];
        if major != 1 || minor != 0 {
            return Err(ReadError::UnsupportedVersion(major, minor));
        }
        cursor += 2;

        // Read constant pool
        let (constants, consumed) = Self::read_constant_pool(&bytes[cursor..])?;
        cursor += consumed;

        // Read code section
        let (code, consumed) = Self::read_code_section(&bytes[cursor..])?;
        cursor += consumed;

        // Read line info
        let (lines, _consumed) = Self::read_line_info(&bytes[cursor..])?;

        Ok(Chunk { code, constants, lines })
    }

    // ... helper methods for reading sections
}
```

---

## Compiler Implementation (Next Step)

The compiler will:

1. Take tokens from Scanner
2. Parse expressions/statements
3. Emit bytecode instructions into Chunk
4. Produce a bytecode file (via BytecodeWriter)

```rust
pub struct Compiler {
    chunk: Chunk,
    // ... parser state
}

impl Compiler {
    pub fn compile(tokens: Vec<Token>) -> Result<Vec<u8>, CompileError> {
        let mut compiler = Self::new();

        // Parse and emit
        compiler.parse(tokens)?;

        // Serialize to binary
        Ok(BytecodeWriter::write(&compiler.chunk))
    }

    fn emit_add(&mut self, line: usize) {
        self.chunk.write(OpCode::Add as u8, line);
    }

    fn emit_constant(&mut self, value: Value, line: usize) {
        self.chunk.emit_constant(value, line);
    }

    // ... more emit methods
}
```

---

## Benefits of This Approach

1. **Clear Separation** - Bytecode format is independent of Rust
2. **Testable** - Can test binary format separately
3. **Portable** - Bytecode files work with any VM
4. **Interoperable** - C VM can run Rust-compiled bytecode
5. **Future-Proof** - Format is versioned and extensible

---

## Testing Strategy

```rust
#[test]
fn test_bytecode_roundtrip() {
    let mut chunk = Chunk::new();
    chunk.emit_constant(Value::Number(42.0), 1);
    chunk.write(OpCode::Return as u8, 1);

    // Write to binary
    let bytes = BytecodeWriter::write(&chunk);

    // Read back
    let loaded = BytecodeReader::read(&bytes).unwrap();

    assert_eq!(chunk.code, loaded.code);
    assert_eq!(chunk.constants, loaded.constants);
}

#[test]
fn test_spec_compliance() {
    // Verify magic number, version, etc.
    let chunk = Chunk::new();
    let bytes = BytecodeWriter::write(&chunk);

    assert_eq!(&bytes[0..4], b"LOXB");
    assert_eq!(bytes[4], 1);  // major version
    assert_eq!(bytes[5], 0);  // minor version
}
```

---

## Next Implementation Steps

1. ✅ Scanner complete
2. **→ Implement bytecode module** (types, encoding, decoding)
3. Implement compiler (emit bytecode)
4. Save/load bytecode files
5. Implement VM (execute bytecode)
6. Test interoperability (optional: write simple C VM)
