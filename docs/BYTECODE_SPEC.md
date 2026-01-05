# Lox Bytecode Specification v1.0

**VM-Independent Bytecode Format**

This specification defines the Lox bytecode format independently of any VM implementation. Any language can implement a VM that executes this bytecode.

---

## Design Goals

1. **VM Independence** - Format defined without reference to implementation language
2. **Portability** - Bytecode files work on any platform/VM
3. **Simplicity** - Easy to implement in any language
4. **Versioning** - Support format evolution
5. **Verifiability** - Bytecode can be validated before execution

---

## File Format

### Binary Structure

```
┌─────────────────────────────────────┐
│ Magic Number (4 bytes)              │  0x4C4F5842 "LOXB"
├─────────────────────────────────────┤
│ Version (2 bytes)                   │  Major.Minor (1.0 = 0x0100)
├─────────────────────────────────────┤
│ Constant Pool Size (4 bytes)        │  N = number of constants
├─────────────────────────────────────┤
│ Constant Pool                       │
│   Constant #0                       │
│   Constant #1                       │
│   ...                               │
│   Constant #N-1                     │
├─────────────────────────────────────┤
│ Code Section Size (4 bytes)         │  M = number of bytes
├─────────────────────────────────────┤
│ Code Section (M bytes)              │  Bytecode instructions
├─────────────────────────────────────┤
│ Line Info Section                   │  For debugging/errors
└─────────────────────────────────────┘
```

**All multi-byte integers are big-endian (network byte order)**

---

## Constant Pool

Each constant has a type tag followed by data:

### Constant Types

```
0x01 - NIL         (no additional data)
0x02 - BOOLEAN     (1 byte: 0x00=false, 0x01=true)
0x03 - NUMBER      (8 bytes: IEEE 754 double, big-endian)
0x04 - STRING      (4 bytes length + UTF-8 bytes)
0x05 - FUNCTION    (function descriptor)
```

### Constant Encoding Examples

**NIL:**
```
[0x01]
```

**BOOLEAN:**
```
[0x02, 0x01]  // true
[0x02, 0x00]  // false
```

**NUMBER:**
```
[0x03, 0x40, 0x09, 0x21, 0xFB, 0x54, 0x44, 0x2D, 0x18]  // 3.14159265
```

**STRING:**
```
[0x04, 0x00, 0x00, 0x00, 0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F]  // "hello"
       └─ length=5 ─┘  └────────── UTF-8 bytes ─────────┘
```

**FUNCTION:**
```
[0x05, name_idx, name_idx, arity, upvalue_count, chunk_size, chunk_size, chunk_size, chunk_size, ...chunk_data...]
       └─ u16 ─┘         u8       u8              └────────── u32 ──────────┘
```

---

## Instruction Set

All instructions are 1 byte opcodes, some followed by operands.

### Format Notation
- `opcode` - 1 byte instruction
- `u8` - 1 byte unsigned integer
- `u16` - 2 bytes unsigned integer (big-endian)

### Stack Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x01 | CONSTANT | u16 | Push constant from pool at index |
| 0x02 | NIL | - | Push nil |
| 0x03 | TRUE | - | Push true |
| 0x04 | FALSE | - | Push false |
| 0x05 | POP | - | Pop and discard top of stack |

### Arithmetic Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x10 | ADD | - | Pop b, pop a, push a + b |
| 0x11 | SUBTRACT | - | Pop b, pop a, push a - b |
| 0x12 | MULTIPLY | - | Pop b, pop a, push a * b |
| 0x13 | DIVIDE | - | Pop b, pop a, push a / b |
| 0x14 | NEGATE | - | Pop a, push -a |

### Comparison Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x20 | EQUAL | - | Pop b, pop a, push a == b |
| 0x21 | GREATER | - | Pop b, pop a, push a > b |
| 0x22 | LESS | - | Pop b, pop a, push a < b |
| 0x23 | NOT | - | Pop a, push !a |

### Variable Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x30 | GET_LOCAL | u8 | Push local variable at slot |
| 0x31 | SET_LOCAL | u8 | Pop value, store in local slot |
| 0x32 | GET_GLOBAL | u16 | Push global variable (name at constant index) |
| 0x33 | DEFINE_GLOBAL | u16 | Pop value, define global (name at constant index) |
| 0x34 | SET_GLOBAL | u16 | Pop value, set global (name at constant index) |
| 0x35 | GET_UPVALUE | u8 | Push upvalue at index |
| 0x36 | SET_UPVALUE | u8 | Pop value, set upvalue at index |

### Control Flow

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x40 | JUMP | u16 | Unconditional jump forward by offset |
| 0x41 | JUMP_IF_FALSE | u16 | Pop value, jump if false |
| 0x42 | LOOP | u16 | Jump backward by offset |

### Function Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x50 | CALL | u8 | Call function with N arguments |
| 0x51 | CLOSURE | u16 | Create closure from function constant |
| 0x52 | CLOSE_UPVALUE | - | Close upvalue on stack top |
| 0x53 | RETURN | - | Return from current function |

### Object Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0x60 | GET_PROPERTY | u16 | Pop instance, push property (name at index) |
| 0x61 | SET_PROPERTY | u16 | Pop value, pop instance, set property |
| 0x62 | CLASS | u16 | Create class (name at constant index) |
| 0x63 | METHOD | u16 | Define method (name at constant index) |
| 0x64 | INHERIT | - | Pop superclass, pop subclass, add inheritance |
| 0x65 | GET_SUPER | u16 | Get superclass method (name at index) |
| 0x66 | INVOKE | u16, u8 | Optimized method call: name index, arg count |
| 0x67 | SUPER_INVOKE | u16, u8 | Optimized super call: name index, arg count |

### Debug Operations

| Opcode | Mnemonic | Operands | Description |
|--------|----------|----------|-------------|
| 0xF0 | PRINT | - | Pop value and print (debug only) |

---

## Line Information Section

Run-length encoded line numbers for debugging:

```
┌─────────────────────────────────────┐
│ Entry Count (4 bytes)               │  N entries
├─────────────────────────────────────┤
│ Entry #0                            │
│   Line Number (4 bytes)             │
│   Instruction Count (4 bytes)       │
├─────────────────────────────────────┤
│ Entry #1                            │
│   Line Number (4 bytes)             │
│   Instruction Count (4 bytes)       │
├─────────────────────────────────────┤
│ ...                                 │
└─────────────────────────────────────┘
```

**Interpretation:**
- First `count` instructions are on line `line_number`
- Next entry's `count` instructions are on that entry's line number
- And so on...

---

## Execution Model

### Stack Machine

The VM maintains:
- **Value Stack** - operand stack for computations
- **Call Stack** - function call frames
- **Globals Table** - global variable storage
- **Heap** - for objects (strings, functions, classes, instances)

### Call Frame

Each function call creates a frame containing:
- Function being executed
- Instruction pointer (IP)
- Stack base pointer (where this frame's locals start)

### Value Types at Runtime

VMs must support these value types:
- **nil** - the null value
- **boolean** - true or false
- **number** - 64-bit floating point (IEEE 754 double)
- **string** - UTF-8 encoded text
- **function** - executable code
- **closure** - function + captured variables
- **class** - class definition
- **instance** - object instance
- **bound method** - method bound to instance

---

## Example: Compiled Function

Source:
```lox
fun add(a, b) {
    return a + b;
}
```

Bytecode (hexadecimal):
```
Constant Pool:
  #0: STRING "add"        [04 00 00 00 03 61 64 64]
  #1: FUNCTION add        [05 00 00 02 00 00 00 00 08 30 00 30 01 10 53]
                                    │  │  └─ chunk size ─┘  │
                                    │  └─ upvalues         │
                                    └─ arity                └─ bytecode:
                                                               30 00    GET_LOCAL 0
                                                               30 01    GET_LOCAL 1
                                                               10       ADD
                                                               53       RETURN

Code Section:
  01 00 01    CONSTANT 1    # Push function constant
  33 00 00    DEFINE_GLOBAL 0  # Define global "add"
```

---

## Validation Rules

Before execution, VMs should validate:

1. **Magic number** matches 0x4C4F5842
2. **Version** is supported
3. **Constant pool** is well-formed (no truncated constants)
4. **Code section** contains valid opcodes
5. **Jump targets** are within code bounds
6. **Constant indices** are within pool bounds
7. **Local slot indices** are within frame limits

---

## Example VM Implementations

A VM must implement:

```pseudocode
class VM {
    stack: Value[]
    globals: Map<String, Value>
    frames: CallFrame[]

    function execute(bytecode: Bytecode) {
        while not at end:
            instruction = read_byte()

            switch instruction:
                case CONSTANT:
                    index = read_u16()
                    push(bytecode.constants[index])

                case ADD:
                    b = pop()
                    a = pop()
                    push(a + b)

                case CALL:
                    arg_count = read_u8()
                    call_function(arg_count)

                // ... handle all opcodes
    }
}
```

---

## Benefits of This Format

1. **Language Agnostic** - Implement VM in any language
2. **Portable** - Same bytecode runs anywhere
3. **Compact** - Binary format is space-efficient
4. **Debuggable** - Line info preserved
5. **Verifiable** - Can validate before execution
6. **Versionable** - Format can evolve

---

## Future Extensions (v2.0+)

Potential additions without breaking v1.0:
- Type annotations section (for static typing)
- Debug symbols section (variable names)
- Optimization hints section
- Module system metadata
- Source map section
