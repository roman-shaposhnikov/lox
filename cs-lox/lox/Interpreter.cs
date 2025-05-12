class Interpreter : ExpressionNodeVisitor<object?>, StatementNodeVisitor<VoidType> {
  readonly EnvironmentRecord environment = new();

  public void Interpret(Statement[] statements) { 
    try {
      foreach (Statement statement in statements) {
        Execute(statement);
      }
    } catch (RuntimeError error) {
      Lox.ReportRuntimeError(error);
    }
  }

  object? Evaluate(Expression expression) {
    return expression.Accept(this);
  }

  void Execute(Statement statement) {
    statement.Accept(this);
  }

  public VoidType VisitPrintStatement(Print statement) {
    var value = Evaluate(statement.expression);
    Console.WriteLine(Stringify(value));

    return new VoidType();
  }

  public VoidType VisitVarStatement(Var statement) {
    object? value = null;
    if (statement.initializer is not null) {
      value = statement.initializer;
    }

    environment.Define(statement.name.lexeme, value);

    return new();
  }

  public object? VisitVariableExpression(Variable expression) {
    return environment.Get(expression.name);
  }

  public VoidType VisitExpressionStatement(ExpressionStatement statement) {
    Evaluate(statement.expression);

    return new VoidType();
  }

  public object? VisitGroupingExpression(Grouping expression) {
    return Evaluate(expression.expression);
  }

  public object? VisitBinaryExpression(Binary expression) {
    var left = Evaluate(expression.left);
    var right = Evaluate(expression.right);

    switch (expression.oper.type) {
      case TokenType.MINUS: {
        CheckNumberOperands(expression.oper, left, right);

       return (float)left - (float)right;
      }
      case TokenType.SLASH: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left / (float)right;
      }
      case TokenType.STAR: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left * (float)right;
      }
      case TokenType.PLUS: {
        return HandlePlusOperator(expression.oper, left, right);
      }
      case TokenType.GREATER: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left > (float)right;
      }
      case TokenType.GREATER_EQUAL: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left >= (float)right;
      }
      case TokenType.LESS: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left < (float)right;
      }
      case TokenType.LESS_EQUAL: {
        CheckNumberOperands(expression.oper, left, right);

        return (float)left <= (float)right;
      }
      case TokenType.BANG_EQUAL: {
        CheckNumberOperands(expression.oper, left, right);

        return !IsEqual(left, right);
      }
      case TokenType.EQUAL_EQUAL: {
        return IsEqual(left, right);
      }
      default: {
        return  null;
      }
    }
  }

  object? HandlePlusOperator(Token oper, object? left, object? right) {
    if (left is float floatLeft && right is float floatRight) {
      return floatLeft + floatRight;
    } 

    if (left is string stringLeft  && right is string stringRight) {
      return stringLeft + stringRight;
    }

    throw new RuntimeError(oper, "Operands must be two numbers or two strings.");
  }

  public object? VisitUnaryExpression(Unary expression) {
    var right = Evaluate(expression.right);

    switch (expression.oper.type) {
      case TokenType.MINUS: {
        CheckNumberOperand(expression.oper, right);

        return -(float)right;
      }
      case TokenType.BANG: {
        return !IsTruthy(right);
      }
      default: {
        return null;
      }
    }
  }

  public object? VisitLiteralExpression(Literal expression) {
    return expression.value;
  }

  bool IsTruthy(object? value) {
    if (value == null) {
      return false;
    }

    if (value is bool v) {
      return v;
    }

    return true;
  }

  bool IsEqual(object? left, object? right) {
    if (left is null && right is null) {
      return true;
    }

    if (left is null) {
      return false;
    }

    return left.Equals(right);
  }

  void CheckNumberOperands(Token oper, object? left, object? right) {
    if (left is not float || right is not float) {
      throw new RuntimeError(oper, "Operands must be numbers.");
    }
  }

  void CheckNumberOperand(Token oper, object? operand) {
    if (operand is not float) {
      throw new RuntimeError(oper, "Operand must be a number.");
    }
  }

  string Stringify(object? value) {
    if (value is null) {
      return "nil";
    }

    if (value is float floatValue) {
      string text = floatValue.ToString();
      if (text.EndsWith(".0")) {
        text = text.Substring(0, text.Length - 2);
      }

      return text;
    }

    return value.ToString();
  }
}
