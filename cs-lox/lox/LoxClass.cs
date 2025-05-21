class LoxClass(string name) : LoxCallable {
  public string name = name;

  public int Arity() {
    return 0;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    LoxInstance instance = new LoxInstance(this);

    return instance;
  }

  public override string ToString() {
    return name;
  }
}
