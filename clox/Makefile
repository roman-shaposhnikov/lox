CC := gcc
# CFLAGS := -g -Wall -Wextra -Werror -O3 -std=gnu11
CFLAGS := -g -Wall -Werror -O3 -std=gnu11

EXTENSION := c
SOURCE := $(wildcard src/*.${EXTENSION})
OBJECTS := $(SOURCE:.${EXTENSION}=.o)
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
