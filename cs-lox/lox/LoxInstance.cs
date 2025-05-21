class LoxInstance(LoxClass loxClass) {
  private readonly LoxClass loxClass = loxClass;
  private readonly Dictionary<string, object?> fields = [];

  public object? Get(Token name) {
    if (fields.TryGetValue(name.lexeme, out object? value)) {
      return value;
    }

    throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
  }

  public void Set(Token name, object? value) {
    if (!fields.TryAdd(name.lexeme, value)) {
      fields[name.lexeme] = value;
    }
  }

  public override string ToString() {
    return $"{loxClass.name} instance";
  }
}
