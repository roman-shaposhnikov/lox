// TODO: place currentChar as class field

class Scanner(string source) {
  readonly string source = source;
  readonly List<Token> tokens = [];
  int startScanningIndex = 0;
  int currentCharIndex = 0;
  int currentScanningLine = 1;

  const char EOF = '\0';
  const char NEW_LINE = '\n';

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

    var doubleCharacterScanResult = ScanDoubleCharacterToken(character);
    if (doubleCharacterScanResult != null) {
      AddToken(doubleCharacterScanResult.Value);
      return;
    }

    switch (character) {
      // Meaningless characters.
      case ' ':
      case '\r':
      case '\t': {
        return; // Ignore whitespace.
      }

      case NEW_LINE: {
        MoveToNextLine();
        return;
      }

      default: {
        Lox.Error(currentScanningLine, "Unexpected character.");
        return;
      }
    }
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

  TokenType? ScanDoubleCharacterToken(char character) {
    return character switch {
      '!' => MoveToNextCharIfMatched('=')
          ? TokenType.BANG_EQUAL
          : TokenType.BANG,
      '=' => MoveToNextCharIfMatched('=')
          ? TokenType.EQUAL_EQUAL
          : TokenType.EQUAL,
      '<' => MoveToNextCharIfMatched('=')
          ? TokenType.LESS_EQUAL
          : TokenType.LESS,
      '>' => MoveToNextCharIfMatched('=')
          ? TokenType.GREATER_EQUAL
          : TokenType.GREATER,
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

  bool MoveToNextCharIfMatched(char expected) {
    if (CheckIsEOFReached()) {
      return false;
    }

    if (PeekCurrentChar() != expected) {
      return false;
    }

    currentCharIndex++;
    return true;
  }

  char PeekCurrentChar() {
    if (CheckIsEOFReached()) {
      return EOF;
    }

    return source[currentCharIndex];
  }

  void MoveToNextLine() {
    currentScanningLine++;
  }
}
