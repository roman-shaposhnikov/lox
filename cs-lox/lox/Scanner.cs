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

  void ScanToken() {}
}
