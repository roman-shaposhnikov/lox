// ##############################################
// ####  This file was generated!            ####
// ####  Do not change it manually           ####
// ####  Check: cs-lox/tools/generateAst.sh  ####
// ##############################################

interface StatementNodeVisitor<ReturnValue> {
  ReturnValue VisitPrintStatement(Print statement);
  ReturnValue VisitReturnStatement(Return statement);
  ReturnValue VisitIfStatement(If statement);
  ReturnValue VisitBlockStatement(Block statement);
  ReturnValue VisitExpressionStatement(ExpressionStatement statement);
  ReturnValue VisitWhileStatement(While statement);
  ReturnValue VisitClassStatement(Class statement);
  ReturnValue VisitFunctionStatement(Function statement);
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

class Return(
  Token keyword,
  Expression? value
) : Statement {
  public readonly Token keyword = keyword;
  public readonly Expression? value = value;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitReturnStatement(this);
  }
}

class If(
  Expression condition,
  Statement thenBranch,
  Statement? elseBranch
) : Statement {
  public readonly Expression condition = condition;
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

class While(
  Expression condition,
  Statement body
) : Statement {
  public readonly Expression condition = condition;
  public readonly Statement body = body;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitWhileStatement(this);
  }
}

class Class(
  Token name,
  Variable? superclass,
  Function[] methods
) : Statement {
  public readonly Token name = name;
  public readonly Variable? superclass = superclass;
  public readonly Function[] methods = methods;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitClassStatement(this);
  }
}

class Function(
  Token name,
  Token[] parameters,
  Statement[] body
) : Statement {
  public readonly Token name = name;
  public readonly Token[] parameters = parameters;
  public readonly Statement[] body = body;
  public override ReturnType Accept<ReturnType>(StatementNodeVisitor<ReturnType> visitor) {
    return visitor.VisitFunctionStatement(this);
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
