class LoxFunction(Function declaration) : LoxCallable {
  readonly Function declaration = declaration;

  public int Arity() {
    return declaration.parameters.Length;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    var environment = new EnvironmentRecord(interpreter.globals);
    for (int i = 0; i < declaration.parameters.Length; i++) {
      environment.Define(
        declaration.parameters.ElementAt(i).lexeme,
        arguments.ElementAt(i)
      );
    }

    interpreter.ExecuteBlock(declaration.body, environment);

    return null;
  }

  public override string ToString() {
    return $"<fn {declaration.name.lexeme}>";
  }
}
