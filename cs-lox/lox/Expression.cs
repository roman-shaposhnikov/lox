// ##############################################
// ####  This file was generated!            ####
// ####  Do not change it manually           ####
// ####  Check: cs-lox/tools/generateAst.sh  ####
// ##############################################

interface ExpressionNodeVisitor<ReturnValue> {
  ReturnValue VisitGroupingExpression(Grouping expression);
  ReturnValue VisitBinaryExpression(Binary expression);
  ReturnValue VisitAssignExpression(Assign expression);
  ReturnValue VisitUnaryExpression(Unary expression);
  ReturnValue VisitVariableExpression(Variable expression);
  ReturnValue VisitLiteralExpression(Literal expression);
}

abstract class Expression {
  public abstract ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor);
}

class Grouping(
  Expression expression
) : Expression {
  public readonly Expression expression = expression;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitGroupingExpression(this);
  }
}

class Binary(
  Expression left,
  Token oper,
  Expression right
) : Expression {
  public readonly Expression left = left;
  public readonly Token oper = oper;
  public readonly Expression right = right;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitBinaryExpression(this);
  }
}

class Assign(
  Token name,
  Expression value
) : Expression {
  public readonly Token name = name;
  public readonly Expression value = value;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitAssignExpression(this);
  }
}

class Unary(
  Token oper,
  Expression right
) : Expression {
  public readonly Token oper = oper;
  public readonly Expression right = right;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitUnaryExpression(this);
  }
}

class Variable(
  Token name
) : Expression {
  public readonly Token name = name;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitVariableExpression(this);
  }
}

class Literal(
  object? value
) : Expression {
  public readonly object? value = value;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitLiteralExpression(this);
  }
}
