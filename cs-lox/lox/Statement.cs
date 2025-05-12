// ##############################################
// ####  This file was generated!            ####
// ####  Do not change it manually           ####
// ####  Check: cs-lox/tools/generateAst.sh  ####
// ##############################################

interface StatementNodeVisitor<ReturnValue> {
  ReturnValue VisitPrintStatement(Print statement);
  ReturnValue VisitExpressionStatement(ExpressionStatement statement);
}

abstract class Statement {
  public abstract ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor);
}

class Print(
  Expression expression
) : Statement {
  public readonly Expression expression = expression;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitPrintStatement(this);
  }
}

class ExpressionStatement(
  Expression expression
) : Statement {
  public readonly Expression expression = expression;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitExpressionStatement(this);
  }
}
