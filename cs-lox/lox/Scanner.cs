// TODO: place currentChar as class field

class Scanner(string source) {
  readonly string source = source;
  readonly List<Token> tokens = [];
  int startScanningIndex = 0;
  int currentCharIndex = 0;
  int currentScanningLine = 1;

  const char EOF = '\0';
  const char NEW_LINE = '\n';

  static Dictionary<string, TokenType> keywords = [];
  static Scanner() {
    keywords.Add("and", TokenType.AND);
    keywords.Add("class", TokenType.CLASS);
    keywords.Add("else", TokenType.ELSE);
    keywords.Add("false", TokenType.FALSE);
    keywords.Add("for", TokenType.FOR);
    keywords.Add("fun", TokenType.FUN);
    keywords.Add("if", TokenType.IF);
    keywords.Add("nil", TokenType.NIL);
    keywords.Add("or", TokenType.OR);
    keywords.Add("print", TokenType.PRINT);
    keywords.Add("return", TokenType.RETURN);
    keywords.Add("super", TokenType.SUPER);
    keywords.Add("this", TokenType.THIS);
    keywords.Add("true", TokenType.TRUE);
    keywords.Add("var", TokenType.VAR);
    keywords.Add("while", TokenType.WHILE);
  }

  public List<Token> ProduceTokens() {
    while (!IsEOFReached()) {
      // We are at the beginning of the next lexeme.
      startScanningIndex = currentCharIndex;
      ScanToken();
    }

    tokens.Add(new Token(TokenType.EOF, "", null, currentScanningLine));

    return tokens;
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
      case '/': {
        ScanSlash();
        return;
      }

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

      case '"': {
        ScanString();
        return;
      }

      default: {
        if (IsDigit(character)) {
          ScanNumber();
          return;
        }

        if (IsAlphabetic(character)) {
          ScanIdentifier();
          return;
        }

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

  void ScanString() {
    while (
      PeekCurrentChar() != '"' &&
      !IsEOFReached()
    ) {
      if (PeekCurrentChar() == NEW_LINE) {
        MoveToNextLine();
      }
      MoveToNextChar();
    }

    if (IsEOFReached()) {
      Lox.Error(currentScanningLine, "Unterminated string.");
      return;
    }

    // The closing ".
    MoveToNextChar();

    // Trim the surrounding quotes.
    string value = source.Substring(startScanningIndex + 1, currentCharIndex - 1);
    AddToken(TokenType.STRING, value);
  }

  void ScanNumber() {
    while (IsDigit(PeekCurrentChar())) {
      MoveToNextChar();
    }

    // Look for a fractional part.
    var isCurrentCharDot = PeekCurrentChar() == '.';
    var isNextCharDigit = IsDigit(
      PeekNextFollowingChar()
    );
    var shouldScanFractionalPart = isCurrentCharDot && isNextCharDigit;
    if (shouldScanFractionalPart) {
      // Consume the "."
      MoveToNextChar();

      while (IsDigit(PeekCurrentChar())) {
        MoveToNextChar();
      }
    }

    AddToken(
      TokenType.NUMBER,
      float.Parse(SelectCurrentLexeme())
    );
  }

  void ScanSlash() {
    if (MoveToNextCharIfMatched('/')) {
      ScanSingleLineComment();
    } else if (MoveToNextCharIfMatched('*')) {
      ScanMultiLineComment();
    } else {
      AddToken(TokenType.SLASH);
    }
  }

  void ScanSingleLineComment() {
    // A comment goes until the end of the line.
    while (true) {
      var currentChar = MoveToNextChar();
      bool isLineBreak = currentChar == NEW_LINE;
      var isEOF = IsEOFReached();
      if (isLineBreak || isEOF) {
        return;
      }
    }
  }

  void ScanMultiLineComment() {
    while (true) {
      var currentChar = MoveToNextChar();

      var isEOF = IsEOFReached();
      if (isEOF) {
        return;
      }

      var isLineBreak = currentChar == NEW_LINE;
      if (isLineBreak) {
        MoveToNextLine();
        continue;
      }

      bool currentCharIsStar = currentChar == '*';
      bool nextCharIsSlash = MoveToNextCharIfMatched('/');
      bool isMultiLineCommentEnd = currentCharIsStar && nextCharIsSlash;
      if (isMultiLineCommentEnd) {
        return;
      }

      bool currentCharIsSlash = currentChar == '/';
      bool nextCharIsStar = MoveToNextCharIfMatched('*');
      bool isInnerMultiLineCommentStart = currentCharIsSlash && nextCharIsStar;
      if (isInnerMultiLineCommentStart) {
        ScanMultiLineComment();
        continue;
      }
    }
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
    if (IsEOFReached()) {
      return false;
    }

    if (PeekCurrentChar() != expected) {
      return false;
    }

    currentCharIndex++;
    return true;
  }

  char PeekCurrentChar() {
    if (IsEOFReached()) {
      return EOF;
    }

    return source[currentCharIndex];
  }

  char PeekNextFollowingChar() {
    if (currentCharIndex + 1 >= source.Length) {
      return EOF;
    }

    return source[currentCharIndex + 1];
  }

  void MoveToNextLine() {
    currentScanningLine++;
  }

  bool IsDigit(char character ) {
    return char.IsDigit(character);
  }

  bool IsAlphabetic(char character) {
    return (
      (character >= 'a' && character <= 'z') ||
      (character >= 'A' && character <= 'Z') ||
      character == '_'
    );
  }

  void ScanIdentifier() {
    while (IsAlphaNumeric(PeekCurrentChar())) {
      MoveToNextChar();
    }

    var lexeme = SelectCurrentLexeme();
    var lexemeIsKeyword = keywords.TryGetValue(lexeme, out TokenType keywordType);
    var tokenType = lexemeIsKeyword ? keywordType : TokenType.IDENTIFIER;

    AddToken(tokenType);
  }

  bool IsAlphaNumeric(char character) {
    return IsAlphabetic(character) || IsDigit(character);
  }

  bool IsEOFReached() {
    return currentCharIndex >= source.Length;
  }

}
