CC := gcc
CFLAGS := -g -Wall -Werror -O3 -std=gnu11

SOURCE := $(wildcard src/*.c)
OBJECTS := $(SOURCE:.c=.o)
TARGET := bin/run

build: $(TARGET) finish

finish:
	rm -f $(OBJECTS)

run:
	./$(TARGET)

clean:
	rm -f $(TARGET)

.PHONY: run clean finish

$(TARGET): $(OBJECTS)
	$(CC) $(CFLAGS) $(OBJECTS) -o $@
