#!/bin/bash

source "./ast.sh"

EXPRESSION_BASE_CLASS_NAME="Expression"
expressionsNodesDescription=(
  "Binary : ${EXPRESSION_BASE_CLASS_NAME} left , Token oper, ${EXPRESSION_BASE_CLASS_NAME} right"
  "Grouping : ${EXPRESSION_BASE_CLASS_NAME} expression "
  "Literal : object? value "
  "Unary : Token oper , ${EXPRESSION_BASE_CLASS_NAME} right"
)

generateAst "../lox" "${EXPRESSION_BASE_CLASS_NAME}" "${expressionsNodesDescription[@]}"

STATEMENT_BASE_CLASS_NAME="Statement"
statementsNodesDescription=(
  "ExpressionStatement : ${EXPRESSION_BASE_CLASS_NAME} expression"
  "Print : ${EXPRESSION_BASE_CLASS_NAME} expression"
)

generateAst "../lox" "${STATEMENT_BASE_CLASS_NAME}" "${statementsNodesDescription[@]}"
