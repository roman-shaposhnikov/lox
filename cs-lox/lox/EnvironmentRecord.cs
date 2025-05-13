class EnvironmentRecord {
  readonly Dictionary<string, object?> values = [];

  public void Define(string name, object? value) {
    values.Add(name, value);
  }

  public object? Get(Token name) {
    if (values.ContainsKey(name.lexeme)) {
      values.TryGetValue(name.lexeme, out object? value);

      return value;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }

  public void Assign(Token name, object? value) {
    if (values.ContainsKey(name.lexeme)) {
      values[name.lexeme] = value;
      return;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }
}
