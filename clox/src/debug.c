#include <stdio.h>

#include "debug.h"

static void disassembleInstructions(Chunk* chunk);
static void simpleInstruction(const char* name, int offset);

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
  uint8_t instruction = chunk->code[offset];
  switch (instruction) {
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
