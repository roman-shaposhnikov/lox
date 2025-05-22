class LoxFunction(Function declaration, EnvironmentRecord closure) : LoxCallable {
  readonly Function declaration = declaration;
  readonly EnvironmentRecord closure = closure;

  public int Arity() {
    return declaration.parameters.Length;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    var environment = new EnvironmentRecord(closure);
    for (int i = 0; i < declaration.parameters.Length; i++) {
      environment.Define(
        declaration.parameters.ElementAt(i).lexeme,
        arguments.ElementAt(i)
      );
    }

    try {
      interpreter.ExecuteBlock(declaration.body, environment);
    } catch (ReturnException expression) {
      return expression.value;
    }

    return null;
  }

  public LoxFunction Bind(LoxInstance instance) {
    EnvironmentRecord environment = new(closure);
    environment.Define("this", instance);

    return new LoxFunction(declaration, environment);
  }

  public override string ToString() {
    return $"<fn {declaration.name.lexeme}>";
  }
}
