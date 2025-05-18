class ClockCallable : LoxCallable {
  public int Arity() {
    return 0;
  }

  public object? Call(Interpreter interpreter, object?[] arguments) {
    return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
  }

  public override string ToString() {
    return "<native fn>";
  }
}
