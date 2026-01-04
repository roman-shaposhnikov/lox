# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an implementation of the Lox programming language from the book "Crafting Interpreters". The repository contains two implementations:

1. **clox** - A C-based bytecode virtual machine (feature-complete)
2. **rs-lox** - A Rust implementation currently under development (scanner/tokenizer phase)

The project goal is to evolve from a dynamic language interpreter to a statically-typed language through Rust implementation, with eventual separation of compilation and execution phases.

## Build and Test Commands

### Rust Implementation (rs-lox)

```bash
# From repository root or rs-lox directory:
cd rs-lox

# Run tests
make test
# Or with specific test pattern:
make test <test_name>

# Run the scanner (current implementation phase)
make run <path_to_lox_file>
# Example:
make run test.local.lox

# Direct cargo commands also work:
cargo test
cargo run <path_to_lox_file>
```

### C Implementation (clox)

```bash
# From repository root or clox directory:
cd clox

# Build the VM
make build

# Run the REPL
make run

# Run a script file
./bin/run <path_to_lox_file>

# Clean build artifacts
make clean
```

## Architecture

### C Implementation (clox) - Complete VM

The C implementation follows a bytecode VM architecture:

- **Scanner** (scanner.c/h) - Lexical analysis, converts source to tokens
- **Compiler** (compiler.c/h) - Single-pass compilation from tokens to bytecode
- **Chunk** (chunk.c/h) - Bytecode container with constant pool
- **VM** (vm.c/h) - Stack-based bytecode interpreter with call frames
- **Object** (object.c/h) - Heap-allocated objects (strings, functions, closures, classes, instances)
- **Value** (value.c/h) - Tagged union for runtime values
- **Table** (table.c/h) - Hash table for globals and string interning
- **Memory** (memory.c/h) - Memory management with mark-and-sweep garbage collector
- **Debug** (debug.c/h) - Bytecode disassembler for debugging

Features: local/global variables, functions with closures, classes with inheritance, garbage collection.

Exit codes: 64 (usage error), 65 (compile error), 70 (runtime error), 74 (file I/O error).

### Rust Implementation (rs-lox) - In Progress

Current phase: **Scanner/Tokenizer only**

Module structure:
- **script** - Entry point for loading and processing Lox files
- **scanner** - Tokenization logic split into focused modules:
  - `scanner` - Main iterator-based scanner orchestrating line processing
  - `line` - Per-line token extraction
  - `token` - Token types (TokenKind enum with variants for literals, operators, keywords)
  - `sequence` - Multi-character token sequences
  - `operator` - Operator token recognition (single and double-char like `!`, `!=`)
  - `identifier` - Identifier and keyword recognition
  - `number` - Number literal parsing
  - `string` - String literal parsing
  - `skip_comments` - Comment filtering
- **shared** - Shared utilities and type extensions
  - `types` - Type aliases like `AnyIter<T>` for boxed iterators
  - `exts` - Iterator extensions (e.g., `Peekable` extensions)

The scanner uses an iterator-based design where:
1. `Script` reads the file and leaks it to `'static` lifetime (current approach)
2. `Scanner` iterates over lines, filtering empty lines and full-line comments
3. `Line` iterates over tokens within each line
4. Tokens maintain position information (line, column, length)

Implementation follows functional patterns with extensive use of iterators and trait implementations.

## Language Grammar

Lox is a dynamically-typed language with C-like syntax. See `docs/grammar.md` for the complete EBNF grammar.

Key features:
- Data types: booleans, numbers (doubles), strings, nil
- Variables: `var` declarations
- Control flow: `if`, `while`, `for`
- Functions: `fun` keyword, first-class functions, closures
- Classes: `class` keyword, inheritance with `<`, `this`, `super`
- Operators: arithmetic, comparison, logical (`and`, `or`), assignment
- Built-in: `print` statement

## Development Notes

### Rust Implementation

- Uses Rust 2024 edition
- Dependencies: `itertools` (0.14.0), `rstest` (0.26.1) for parameterized tests
- Tests use `rstest` for table-driven testing
- Current limitation: Uses leaked `'static` strings to avoid lifetime complexity (noted as TODO)
- Code style: Functional, iterator-heavy approach

### C Implementation

- Uses GNU C11 standard (`-std=gnu11`)
- Compiler flags: `-g -Wall -Werror -O3`
- No external dependencies, only C standard library
- Single-pass compiler directly to bytecode (no AST)
- Manual memory management with tracing GC

## Repository Structure

```
lox/
├── clox/          # C bytecode VM implementation (complete)
│   ├── src/       # C source files
│   ├── bin/       # Build output
│   └── Makefile
├── rs-lox/        # Rust implementation (in progress)
│   ├── src/       # Rust source files
│   └── Cargo.toml
├── docs/          # Documentation
│   └── grammar.md # Complete Lox language grammar
└── lox-vscode.local/  # VSCode extension (local development)
```

Files with `.local.*` extension are gitignored (test files, scratch work).
