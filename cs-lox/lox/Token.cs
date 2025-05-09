class Token(
  TokenType type,
  string lexeme,
  object? literal,
  int line // TODO: keep position as offset and lexeme length
) {
  public readonly TokenType type = type;
  public readonly string lexeme = lexeme;
  public readonly object? literal = literal;
  public readonly int line = line;

  override public string ToString() {
    return string.Format(null, "{0} {1} {2}", type, lexeme, literal) ;
  }

  public bool IsTypeOf(TokenType type) {
    return this.type == type;
  }
}
