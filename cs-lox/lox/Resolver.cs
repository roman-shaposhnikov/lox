class Resolver(Interpreter interpreter) : ExpressionNodeVisitor<VoidType>, StatementNodeVisitor<VoidType> {
  readonly Interpreter interpreter = interpreter;
  readonly Stack<Dictionary<string, bool>> scopes = new();
  FunctionType currentFunction = FunctionType.NONE;
  ClassType currentClass = ClassType.NONE;

  enum FunctionType {
    NONE,
    FUNCTION,
    INITIALIZER,
    METHOD,
  }

  enum ClassType {
    NONE,
    CLASS,
    SUBCLASS,
  }

  public VoidType VisitBlockStatement(Block statement)
  {
    BeginScope();
    Resolve(statement.statements);
    EndScope();

    return new VoidType();
  }

  public VoidType VisitClassStatement(Class statement) {
    ClassType enclosingClass = currentClass;
    currentClass = ClassType.CLASS;

    Declare(statement.name);
    Define(statement.name);

    if (statement.superclass is not null) {
      if (statement.name.lexeme.Equals(statement.superclass.name.lexeme)) {
        throw new RuntimeError(statement.superclass.name, "A class cannot inherit from itself.");
      }

      currentClass = ClassType.SUBCLASS;
      Resolve(statement.superclass);

      BeginScope();
      scopes.Peek().Add("super", true);
    }

    BeginScope();
    scopes.Peek().Add("this", true);

    foreach (Function method in statement.methods) {
      var isInitMethod = method.name.lexeme.Equals("init");
      var functionType = isInitMethod ? FunctionType.INITIALIZER : FunctionType.METHOD;
      ResolveFunction(method, functionType);
    }

    EndScope();

    if (statement.superclass is not null) {
      EndScope();
    }

    currentClass = enclosingClass;

    return new();
  }

  public VoidType VisitVarStatement(Var statement){
    Declare(statement.name);

    if (statement.initializer is not null) {
      Resolve(statement.initializer);
    }

    Define(statement.name);

    return new VoidType();
  }

  public VoidType VisitVariableExpression(Variable expression) {
    var scopeExists = scopes.Count > 0;
    if (scopeExists) {
      var currentScope = scopes.Peek();
      var variableDeclared = currentScope.TryGetValue(expression.name.lexeme, out bool variableDefined);
      if (variableDeclared && !variableDefined) {
        Lox.Error(expression.name, "Can't read local variable in its own initializer.");
      }
    }

    ResolveLocal(expression, expression.name);

    return new();
  }

  public VoidType VisitAssignExpression(Assign expression) {
    Resolve(expression.value);
    ResolveLocal(expression, expression.name);
    return new();
  }

  public VoidType VisitFunctionStatement(Function statement) {
    Declare(statement.name);
    Define(statement.name);
    ResolveFunction(statement, FunctionType.FUNCTION);

    return new();
  }

  public VoidType VisitExpressionStatement(ExpressionStatement statement) {
    Resolve(statement.expression);

    return new();
  }

  public VoidType VisitIfStatement(If statement) {
    Resolve(statement.condition);
    Resolve(statement.thenBranch);

    if (statement.elseBranch != null) {
      Resolve(statement.elseBranch);
    }

    return new();
  }

  public VoidType VisitPrintStatement(Print statement) {
    Resolve(statement.expression);

    return new();
  }

  public VoidType VisitReturnStatement(Return statement) {
    if (currentFunction is FunctionType.NONE) {
      Lox.Error(statement.keyword, "Can't return from top-level code.");
    }

    if (statement.value != null) {
      if (currentFunction is FunctionType.INITIALIZER) {
        Lox.Error(statement.keyword, "Can't return a value from an initializer.");
      }

      Resolve(statement.value);
    }

    return new();
  }

  public VoidType VisitWhileStatement(While statement) {
    Resolve(statement.condition);
    Resolve(statement.body);

    return new();
  }

  public VoidType VisitBinaryExpression(Binary expression) {
    Resolve(expression.left);
    Resolve(expression.right);

    return new();
  }

  public VoidType VisitCallExpression(Call expression) {
    Resolve(expression.callee);

    foreach (Expression argument in expression.arguments) {
      Resolve(argument);
    }

    return new();
  }

  public VoidType VisitGetExpression(Get expression) {
    Resolve(expression.obj);

    return new();
  }

  public VoidType VisitGroupingExpression(Grouping expression) {
    Resolve(expression.expression);

    return new();
  }

  public VoidType VisitLiteralExpression(Literal _) {
    return new();
  }

  public VoidType VisitLogicalExpression(Logical expression) {
    Resolve(expression.left);
    Resolve(expression.right);

    return new();
  }

  public VoidType VisitSetExpression(Set expression) {
    Resolve(expression.value);
    Resolve(expression.obj);

    return new();
  }

  public VoidType VisitSuperExpression(Super expression) {
    if (currentClass is ClassType.NONE) {
      Lox.Error(expression.keyword, "Can't use 'super' outside of a class.");
    } else if (currentClass is not ClassType.SUBCLASS) {
      Lox.Error(expression.keyword, "Can't use 'super' in a class with no superclass.");
    }

    ResolveLocal(expression, expression.keyword);

    return new();
  }

  public VoidType VisitThisExpression(This expression) {
    if (currentClass is ClassType.NONE) {
      Lox.Error(expression.keyword, "Can't use 'this' outside of a class.");

      return new();
    }

    ResolveLocal(expression, expression.keyword);

    return new();
  }

  public VoidType VisitUnaryExpression(Unary expression) {
    Resolve(expression.right);

    return new();
  }

  void BeginScope() {
    scopes.Push([]);
  }

  void EndScope() {
    scopes.Pop();
  }

  public void Resolve(Statement[] statements) {
    foreach (Statement statement in statements) {
      Resolve(statement);
    }
  }

  void Resolve(Statement statement) {
    statement.Accept(this);
  }

  void Resolve(Expression expression) {
    expression.Accept(this);
  }

  void ResolveFunction(Function function, FunctionType type) {
    FunctionType enclosingFunction = currentFunction;
    currentFunction = type;

    BeginScope();

    foreach (Token param in function.parameters) {
      Declare(param);
      Define(param);
    }

    Resolve(function.body);
    EndScope();
    currentFunction = enclosingFunction;
  }

  void Declare(Token name) {
    if (scopes.Count == 0) {
      return;
    }

    var scope = scopes.Peek();
    if (scope.ContainsKey(name.lexeme)) {
      Lox.Error(name, "Already a variable with this name in this scope.");
    }

    scope.Add(name.lexeme, false);
  }

  void Define(Token name) {
    if (scopes.Count == 0) {
      return;
    }

    var scope = scopes.Peek();
    scope[name.lexeme] = true;
  }

  void ResolveLocal(Expression expression, Token name) {
    for (int i = scopes.Count - 1; i >= 0; i--) {
      // wtf?! какого хрена в стек новые ел-ты добав. слева
      var scope = scopes.Reverse().ElementAt(i);
      var variableDeclared = scope.ContainsKey(name.lexeme);
      if (variableDeclared) {
        var skippedScopesCount = scopes.Count - 1 - i;
        interpreter.Resolve(expression, skippedScopesCount);
        return;
      }
    }
  }
}
