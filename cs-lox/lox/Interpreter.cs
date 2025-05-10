class Interpreter : AstVisitor<object?> {
  object? Evaluate(Expression expression) {
    return expression.Accept(this);
  }

  public object? VisitGroupingExpression(Grouping expression) {
    return Evaluate(expression.expression);
  }

  public object? VisitBinaryExpression(Binary expression) {
    var left = Evaluate(expression.left);
    var right = Evaluate(expression.right);

    return expression.oper.type switch {
      TokenType.MINUS => (float)left - (float)right,
      TokenType.SLASH => (float)left / (float)right,
      TokenType.STAR => (float)left * (float)right,
      TokenType.PLUS => HandlePlusOperator(left, right),
      TokenType.GREATER => (float)left > (float)right,
      TokenType.GREATER_EQUAL => (float)left >= (float)right,
      TokenType.LESS => (float)left < (float)right,
      TokenType.LESS_EQUAL => (float)left <= (float)right,
      TokenType.BANG_EQUAL => !IsEqual(left, right),
      TokenType.EQUAL_EQUAL => IsEqual(left, right),
      _ => null,
    };
  }

  object? HandlePlusOperator(object? left, object? right) {
    if (left is float floatLeft && right is float floatRight) {
      return floatLeft + floatRight;
    } 

    if (left is string stringLeft  && right is string stringRight) {
      return stringLeft + stringRight;
    }

    return null;
  }

  public object? VisitUnaryExpression(Unary expression) {
    var right = Evaluate(expression.right);

    return expression.oper.type switch {
      TokenType.MINUS => -(float)right,
      TokenType.BANG => !IsTruthy(right),
      _ => null,
    };
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
}
