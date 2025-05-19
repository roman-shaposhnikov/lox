class EnvironmentRecord {
  public EnvironmentRecord? enclosing;
  public readonly Dictionary<string, object?> values = [];

  public EnvironmentRecord() {
    enclosing = null;
  }

  public EnvironmentRecord(EnvironmentRecord enclosing) {
    this.enclosing = enclosing;
  }

  public void Define(string name, object? value) {
    values.Add(name, value);
  }

  public object? GetAt(int distance, string name) {
    return GetAncestor(distance).values[name];
  }

  public void AssignAt(int distance, Token name, object? value) {
    var targetEnvironment = GetAncestor(distance);
    targetEnvironment.values[name.lexeme] = value;
  }

  EnvironmentRecord GetAncestor(int distance) {
    EnvironmentRecord environment = this;
    for (int i = 0; i < distance; i++) {
      environment = environment.enclosing; 
    }

    return environment;
  }

  public object? Get(Token name) {
    if (values.ContainsKey(name.lexeme)) {
      values.TryGetValue(name.lexeme, out object? value);

      return value;
    }

    if (enclosing is not null) {
      return enclosing.Get(name);
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }

  public void Assign(Token name, object? value) {
    if (values.ContainsKey(name.lexeme)) {
      values[name.lexeme] = value;
      return;
    }

    if (enclosing is not null) {
      enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }
}
