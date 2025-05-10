class RuntimeError(Token token, string message) : Exception(message) {
  public Token token = token;
  public string message = message;
}
