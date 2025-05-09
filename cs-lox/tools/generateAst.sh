#!/bin/bash

source "./lib.sh"
source "./ast.sh"

BASE_CLASS_NAME="Expression"
astNodesDescription=(
  "Binary : ${BASE_CLASS_NAME} left , Token oper, ${BASE_CLASS_NAME} right"
  "Grouping : ${BASE_CLASS_NAME} expression "
  "Literal : object? value "
  "Unary : Token oper , ${BASE_CLASS_NAME} right"
)

generateAst "../lox" "Expression" "${astNodesDescription[@]}"
