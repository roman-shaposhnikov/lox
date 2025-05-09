// TODO: place currentChar as class field

class Scanner(string source) {
  readonly string source = source;
  readonly List<Token> tokens = [];
  int startScanningIndex = 0;
  int currentCharIndex = 0;
  int currentScanningLine = 1;

  public List<Token> ProduceTokens() {
    while (!CheckIsEOFReached()) {
      // We are at the beginning of the next lexeme.
      startScanningIndex = currentCharIndex;
      ScanToken();
    }

    tokens.Add(new Token(TokenType.EOF, "", null, currentScanningLine));

    return tokens;
  }

  bool CheckIsEOFReached() {
    return currentCharIndex >= source.Length;
  }

  void ScanToken() {
    var character = MoveToNextChar();
    var singleCharacterScanResult = ScanSingleCharacterToken(character);
    if (singleCharacterScanResult != null) {
      AddToken(singleCharacterScanResult.Value);
      return;
    }

    Lox.Error(currentScanningLine, "Unexpected character.");
  }

  char MoveToNextChar() {
    return source[currentCharIndex++];
  }

  TokenType? ScanSingleCharacterToken(char character) {
    return character switch {
      '(' => TokenType.LEFT_PAREN,
      ')' => TokenType.RIGHT_PAREN,
      '{' => TokenType.LEFT_BRACE,
      '}' => TokenType.RIGHT_BRACE,
      ',' => TokenType.COMMA,
      '.' => TokenType.DOT,
      '-' => TokenType.MINUS,
      '+' => TokenType.PLUS,
      ';' => TokenType.SEMICOLON,
      '*' => TokenType.STAR,
      _ => null,
    };
  }

  void AddToken(TokenType type) {
    AddToken(type, null);
  }

  void AddToken(TokenType type, object? literal) {
    var lexeme = SelectCurrentLexeme();
    var token = new Token(type, lexeme, literal, currentScanningLine);

    tokens.Add(token);
  }

  string SelectCurrentLexeme() {
    return source.Substring(startScanningIndex, currentCharIndex);
  }
}
