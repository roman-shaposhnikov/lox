#!/bin/bash

source "./ast.sh"

TARGET_DIR="../lox"

EXPRESSION_BASE_CLASS_NAME="Expression"
expressionsNodesDescription=(
  "Binary : ${EXPRESSION_BASE_CLASS_NAME} left , Token oper, ${EXPRESSION_BASE_CLASS_NAME} right"
  "Grouping : ${EXPRESSION_BASE_CLASS_NAME} expression "
  "Literal : object? value "
  "Unary : Token oper , ${EXPRESSION_BASE_CLASS_NAME} right"
)

generateAst $TARGET_DIR $EXPRESSION_BASE_CLASS_NAME "${expressionsNodesDescription[@]}"

STATEMENT_BASE_CLASS_NAME="Statement"
statementsNodesDescription=(
  "ExpressionStatement : ${EXPRESSION_BASE_CLASS_NAME} expression"
  "Print : ${EXPRESSION_BASE_CLASS_NAME} expression"
  "Var : Token name, ${EXPRESSION_BASE_CLASS_NAME}? initializer"
)

generateAst $TARGET_DIR $STATEMENT_BASE_CLASS_NAME "${statementsNodesDescription[@]}"
