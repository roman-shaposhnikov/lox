using FunctionsCollection = System.Collections.Generic.Dictionary<string, LoxFunction>;

class LoxClass(string name, FunctionsCollection methods) : LoxCallable {
  public string name = name;
  readonly FunctionsCollection methods = methods;

  public int Arity() {
    LoxFunction? initializer = FindMethod("init");
    int classArity = initializer?.Arity() ?? 0;

    return classArity;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    LoxInstance instance = new(this);
    LoxFunction? initializer = FindMethod("init");
    initializer?.Bind(instance).Call(interpreter, arguments);

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
