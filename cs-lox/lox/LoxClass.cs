using FunctionsCollection = System.Collections.Generic.Dictionary<string, LoxFunction>;

class LoxClass(string name, FunctionsCollection methods) : LoxCallable {
  public string name = name;
  readonly FunctionsCollection methods = methods;

  public int Arity() {
    return 0;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    LoxInstance instance = new LoxInstance(this);

    return instance;
  }

  public LoxFunction? FindMethod(string name) {
    if (methods.TryGetValue(name, out LoxFunction? method)) {
      return method;
    }

    return null;
  }

  public override string ToString() {
    return name;
  }
}
