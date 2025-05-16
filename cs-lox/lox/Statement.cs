// ##############################################
// ####  This file was generated!            ####
// ####  Do not change it manually           ####
// ####  Check: cs-lox/tools/generateAst.sh  ####
// ##############################################

interface StatementNodeVisitor<ReturnValue> {
  ReturnValue VisitPrintStatement(Print statement);
  ReturnValue VisitIfStatement(If statement);
  ReturnValue VisitBlockStatement(Block statement);
  ReturnValue VisitExpressionStatement(ExpressionStatement statement);
  ReturnValue VisitVarStatement(Var statement);
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

class If(
  Expression,
  Statement thenBranch,
  Statement? elseBranch
) : Statement {
  public readonly Expression  = ;
  public readonly Statement thenBranch = thenBranch;
  public readonly Statement? elseBranch = elseBranch;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitIfStatement(this);
  }
}

class Block(
  Statement[] statements
) : Statement {
  public readonly Statement[] statements = statements;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitBlockStatement(this);
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

class Var(
  Token name,
  Expression? initializer
) : Statement {
  public readonly Token name = name;
  public readonly Expression? initializer = initializer;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitVarStatement(this);
  }
}
