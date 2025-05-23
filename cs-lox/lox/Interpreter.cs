using System.Reflection;

class Interpreter : ExpressionNodeVisitor<object?>, StatementNodeVisitor<VoidType> {
  public readonly EnvironmentRecord globals;
  EnvironmentRecord environment;
  readonly Dictionary<Expression, int?> locals = [];

  public Interpreter() {
    globals = new();
    environment = globals;

    globals.Define("clock", new ClockCallable());
  }

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

  public void Resolve(Expression expression, int depth) {
    locals.Add(expression, depth);
  }

  public VoidType VisitBlockStatement(Block statement) {
    ExecuteBlock(statement.statements, new EnvironmentRecord(environment));

    return new VoidType();
  }

  public VoidType VisitClassStatement(Class statement) {
    LoxClass? superclass = null;
    if (statement.superclass is not null) {
      var evaluatedSuperclass = Evaluate(statement.superclass);
      if (evaluatedSuperclass is not LoxClass) {
        throw new RuntimeError(statement.superclass.name, "Superclass must be a class.");
      }

      superclass = (LoxClass)evaluatedSuperclass;
    }

    environment.Define(statement.name.lexeme, null);

    if (statement.superclass is not null) {
      environment = new EnvironmentRecord(environment);
      environment.Define("super", superclass);
    }

    var methods = new Dictionary<string, LoxFunction>();
    foreach (Function method in statement.methods) {
      var function = new LoxFunction(method, environment, method.name.lexeme.Equals("init"));
      methods.Add(method.name.lexeme, function);
    }

    var loxClass = new LoxClass(statement.name.lexeme, superclass, methods);

    if (superclass is not null) {
      environment = environment.enclosing;
    }

    environment.Assign(statement.name, loxClass);

    return new();
  }

  public void ExecuteBlock(Statement[] statements, EnvironmentRecord environment) {
    EnvironmentRecord previous = this.environment;

    try {
      this.environment = environment;

      foreach (Statement statement in statements) {
        Execute(statement);
      }
    } finally {
      this.environment = previous;
    }
  }

  public VoidType VisitPrintStatement(Print statement) {
    var value = Evaluate(statement.expression);
    Console.WriteLine(Stringify(value));

    return new VoidType();
  }

  public VoidType VisitReturnStatement(Return statement) {
    object? value = null;
    if (statement.value is not null) {
      value = Evaluate(statement.value);
    }

    throw new ReturnException(value);
  }

  public VoidType VisitVarStatement(Var statement) {
    object? value = null;
    if (statement.initializer is not null) {
      value = Evaluate(statement.initializer);
    }

    environment.Define(statement.name.lexeme, value);

    return new();
  }

  public VoidType VisitWhileStatement(While statement) {
    while (IsTruthy(Evaluate(statement.condition))) {
      Execute(statement.body);
    }

    return new VoidType();
  }

  public object? VisitAssignExpression(Assign expression) {
    var value = Evaluate(expression.value);

    int? distance = locals[expression];
    if (distance is not null) {
      environment.AssignAt((int)distance, expression.name, value);
    } else {
      globals.Assign(expression.name, value);
    }

    environment.Assign(expression.name, value);

    return value;
  }

  public object? VisitVariableExpression(Variable expression) {
    return LookUpVariable(expression.name, expression);
  }

  object? LookUpVariable(Token name, Expression expression) {
    var hasValue = locals.TryGetValue(expression, out int? distance);
    if (hasValue && distance is not null) {
      return environment.GetAt((int)distance, name.lexeme);
    } else {
      return globals.Get(name);
    }
  }

  public VoidType VisitExpressionStatement(ExpressionStatement statement) {
    Evaluate(statement.expression);

    return new VoidType();
  }

  public VoidType VisitFunctionStatement(Function statement) {
    var function = new LoxFunction(statement, environment, false);
    environment.Define(statement.name.lexeme, function);

    return new VoidType();
  }

  public VoidType VisitIfStatement(If statement) {
    var condition = Evaluate(statement.condition);
    if (IsTruthy(condition)) {
      Execute(statement.thenBranch);
    } else if (statement.elseBranch is not null) {
      Execute(statement.elseBranch);
    }

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

  public object? VisitCallExpression(Call expression) {
    var callee = Evaluate(expression.callee);

    List<object?> arguments = [];
    foreach (Expression argument in expression.arguments) {
      arguments.Add(Evaluate(argument));
    }

    if (callee is not LoxCallable) {
      throw new RuntimeError(expression.paren, "Can only call functions and classes.");
    }

    var function = (LoxCallable)callee;
    if (arguments.Count != function.Arity()) {
      throw new RuntimeError(expression.paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
    }

    return function.Call(this, arguments.ToArray());
  }

  public object? VisitGetExpression(Get expression) {
    object? obj = Evaluate(expression.obj);
    if (obj is LoxInstance instance) {
      return instance.Get(expression.name);
    }

    throw new RuntimeError(expression.name, "Only instances have properties.");
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

  public object? VisitLogicalExpression(Logical expression) {
    var left = Evaluate(expression.left);

    if (expression.oper.type == TokenType.OR) {
      if (IsTruthy(left)) {
        return left;
      }
    } else {
      if (!IsTruthy(left)) {
        return left;
      }
    }

    return Evaluate(expression.right);
  }

  public object? VisitSetExpression(Set expression) {
    var obj = Evaluate(expression.obj);
    if (obj is not LoxInstance instance) {
      throw new RuntimeError(expression.name, "Only instances have fields.");
    }

    var value = Evaluate(expression.value);
    instance.Set(expression.name, value);

    return value;
  }

  public object? VisitSuperExpression(Super expression) {
    int? distance = locals[expression];
    var superclass = (LoxClass)environment.GetAt((int)distance, "super");
    var obj = (LoxInstance)environment.GetAt((int)distance - 1, "this");

    var method = superclass.FindMethod(expression.method.lexeme);
    if (method is null) {
      throw new RuntimeError(expression.method, $"Undefined property '{expression.method.lexeme}'.");
    }

    return method.Bind(obj);
  }

  public object? VisitThisExpression(This expression) {
    return LookUpVariable(expression.keyword, expression);
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
