interface LoxCallable {
  object Call(Interpreter interpreter, object?[] arguments);
}
