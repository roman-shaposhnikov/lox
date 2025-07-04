CC=gcc
CFLAGS=-Iinclude

SRC=$(wildcard src/*.c)
OBJ=$(SRC:.c=.o)
TARGET=bin/run

all: $(TARGET)

$(TARGET): $(OBJ)
	$(CC) $(OBJ) -o $@

%.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	rm -f src/*.o
