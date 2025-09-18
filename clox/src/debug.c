#include <stdio.h>

#include "debug.h"
#include "value.h"

static void disassembleInstructions(Chunk* chunk);
static void simpleInstruction(const char* name, int offset);
static void constantInstruction(const char* name, Chunk* chunk, int offset);

void disassembleChunk(Chunk* chunk, const char* name) {
  printf("== %s ==\n", name);
  disassembleInstructions(chunk);
}

static void disassembleInstructions(Chunk* chunk) {
  for (int offset = 0; offset < chunk->count;) {
    offset = disassembleInstruction(chunk, offset);
  }
}

int disassembleInstruction(Chunk* chunk, int offset) {
  printf("%04d ", offset);
  if (offset > 0 && chunk->lines[offset] == chunk->lines[offset - 1]) {
    printf("   | ");
  } else {
    printf("%4d ", chunk->lines[offset]);
  }

  uint8_t instruction = chunk->code[offset];
  switch (instruction) {
    case OP_CONSTANT: {
      constantInstruction("OP_CONSTANT", chunk, offset);
      return offset + 2;
    }
    case OP_NIL: {
      simpleInstruction("OP_NIL", offset);
      break;
    }
    case OP_TRUE: {
      simpleInstruction("OP_TRUE", offset);
      break;
    }
    case OP_FALSE: {
      simpleInstruction("OP_FALSE", offset);
      break;
    }
    case OP_ADD: {
      simpleInstruction("OP_ADD", offset);
      break;
    }
    case OP_SUBTRACT: {
      simpleInstruction("OP_SUBTRACT", offset);
      break;
    }
    case OP_MULTIPLY: {
      simpleInstruction("OP_MULTIPLY", offset);
      break;
    }
    case OP_DIVIDE: {
      simpleInstruction("OP_DIVIDE", offset);
      break;
    }
    case OP_NEGATE: {
      simpleInstruction("OP_NEGATE", offset);
      break;
    }
    case OP_RETURN: {
      simpleInstruction("OP_RETURN", offset);
      break;
    }
    default: {
      printf("Unknown opcode %d\n", instruction);
      break;
    }
  }
  return offset + 1;
}

static void simpleInstruction(const char* name, int offset) {
  printf("%s\n", name);
}

static void constantInstruction(const char* name, Chunk* chunk, int offset) {
  uint8_t constant = chunk->code[offset + 1];
  printf("%-16s %4d '", name, constant);
  printValue(chunk->constants.values[constant]);
  printf("'\n");
}
