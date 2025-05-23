class LoxFunction(Function declaration, EnvironmentRecord closure, bool isInitializer) : LoxCallable {
  readonly Function declaration = declaration;
  readonly EnvironmentRecord closure = closure;
  readonly bool isInitializer = isInitializer;

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
      if (isInitializer) {
        return closure.GetAt(0, "this");
      }

      return expression.value;
    }

    if (isInitializer) {
      return closure.GetAt(0, "this");
    }

    return null;
  }

  public LoxFunction Bind(LoxInstance instance) {
    EnvironmentRecord environment = new(closure);
    environment.Define("this", instance);

    return new LoxFunction(declaration, environment, isInitializer);
  }

  public override string ToString() {
    return $"<fn {declaration.name.lexeme}>";
  }
}
