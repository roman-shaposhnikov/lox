#!/bin/bash

source "./ast.sh"

TARGET_DIR="../lox"

EXPRESSION_BASE_CLASS_NAME="Expression"
expressionsNodesDescription=(
  "Assign : Token name, ${EXPRESSION_BASE_CLASS_NAME} value"
  "Binary : ${EXPRESSION_BASE_CLASS_NAME} left , Token oper, ${EXPRESSION_BASE_CLASS_NAME} right"
  "Call : ${EXPRESSION_BASE_CLASS_NAME} callee, Token paren, ${EXPRESSION_BASE_CLASS_NAME}[] arguments"
  "Get : ${EXPRESSION_BASE_CLASS_NAME} obj, Token name"
  "Grouping : ${EXPRESSION_BASE_CLASS_NAME} expression "
  "Literal : object? value "
  "Logical : ${EXPRESSION_BASE_CLASS_NAME} left, Token oper, ${EXPRESSION_BASE_CLASS_NAME} right"
  "Set : ${EXPRESSION_BASE_CLASS_NAME} obj, Token name, ${EXPRESSION_BASE_CLASS_NAME} value"
  "Super : Token keyword, Token method"
  "This : Token keyword"
  "Unary : Token oper , ${EXPRESSION_BASE_CLASS_NAME} right"
  "Variable : Token name"
)

generateAst $TARGET_DIR $EXPRESSION_BASE_CLASS_NAME "${expressionsNodesDescription[@]}"

STATEMENT_BASE_CLASS_NAME="Statement"
statementsNodesDescription=(
  "Block : ${STATEMENT_BASE_CLASS_NAME}[] statements"
  "Class : Token name, Variable? superclass, Function[] methods"
  "ExpressionStatement : ${EXPRESSION_BASE_CLASS_NAME} expression"
  "Function : Token name, Token[] parameters, ${STATEMENT_BASE_CLASS_NAME}[] body"
  "If : Expression condition, ${STATEMENT_BASE_CLASS_NAME} thenBranch, ${STATEMENT_BASE_CLASS_NAME}? elseBranch"
  "Print : ${EXPRESSION_BASE_CLASS_NAME} expression"
  "Return : Token keyword, ${EXPRESSION_BASE_CLASS_NAME}? value"
  "Var : Token name, ${EXPRESSION_BASE_CLASS_NAME}? initializer"
  "While : Expression condition, ${STATEMENT_BASE_CLASS_NAME} body"
)

generateAst $TARGET_DIR $STATEMENT_BASE_CLASS_NAME "${statementsNodesDescription[@]}"
