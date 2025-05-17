interface LoxCallable {
  int Arity();
  object Call(Interpreter interpreter, object?[] arguments);
}
