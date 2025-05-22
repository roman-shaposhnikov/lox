// ##############################################
// ####  This file was generated!            ####
// ####  Do not change it manually           ####
// ####  Check: cs-lox/tools/generateAst.sh  ####
// ##############################################

interface ExpressionNodeVisitor<ReturnValue> {
  ReturnValue VisitCallExpression(Call expression);
  ReturnValue VisitSetExpression(Set expression);
  ReturnValue VisitGroupingExpression(Grouping expression);
  ReturnValue VisitBinaryExpression(Binary expression);
  ReturnValue VisitLogicalExpression(Logical expression);
  ReturnValue VisitAssignExpression(Assign expression);
  ReturnValue VisitUnaryExpression(Unary expression);
  ReturnValue VisitVariableExpression(Variable expression);
  ReturnValue VisitThisExpression(This expression);
  ReturnValue VisitLiteralExpression(Literal expression);
  ReturnValue VisitGetExpression(Get expression);
}

abstract class Expression {
  public abstract ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor);
}

class Call(
  Expression callee,
  Token paren,
  Expression[] arguments
) : Expression {
  public readonly Expression callee = callee;
  public readonly Token paren = paren;
  public readonly Expression[] arguments = arguments;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitCallExpression(this);
  }
}

class Set(
  Expression obj,
  Token name,
  Expression value
) : Expression {
  public readonly Expression obj = obj;
  public readonly Token name = name;
  public readonly Expression value = value;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitSetExpression(this);
  }
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

class Logical(
  Expression left,
  Token oper,
  Expression right
) : Expression {
  public readonly Expression left = left;
  public readonly Token oper = oper;
  public readonly Expression right = right;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitLogicalExpression(this);
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

class This(
  Token keyword
) : Expression {
  public readonly Token keyword = keyword;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitThisExpression(this);
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

class Get(
  Expression obj,
  Token name
) : Expression {
  public readonly Expression obj = obj;
  public readonly Token name = name;
  public override ReturnType Accept<ReturnType>(ExpressionNodeVisitor<ReturnType> visitor) {
    return visitor.VisitGetExpression(this);
  }
}
