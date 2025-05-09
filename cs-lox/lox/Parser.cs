// TODO: place currentToken as class field

class Parser(Token[] tokens) {
  readonly Token[] tokens = tokens;
  int currentTokenIndex = 0;

  Expression ParseExpression() {
    return ParseEquality();
  }

  Expression ParseEquality() {
    Expression expression = ParseComparison();

    TokenType[] equalityOperators = [TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL];
    while (MoveToNextIfMatchOneOf(equalityOperators)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseComparison();
      expression = new Binary(expression, oper, right);
    }

    return expression;
  }

  Expression ParseComparison() {
    Expression expression = ParseTerm();

    TokenType[] comparisonOperators = [
      TokenType.GREATER,
      TokenType.GREATER_EQUAL,
      TokenType.LESS,
      TokenType.LESS_EQUAL
    ];
    while(MoveToNextIfMatchOneOf(comparisonOperators)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseTerm();
      expression = new Binary(expression, oper, right);
    }

    return expression;
  }

  Expression ParseTerm() {
    Expression expression = ParseFactor();

    TokenType[] termOperators = [
      TokenType.MINUS,
      TokenType.PLUS
    ];
    while (MoveToNextIfMatchOneOf(termOperators)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseFactor();
      expression = new Binary(expression, oper, right);
    }

    return expression;
  }

  Expression ParseFactor() {
    Expression expression = ParseUnary();

    TokenType[] factorOperators = [
      TokenType.SLASH,
      TokenType.STAR
    ];
    while (MoveToNextIfMatchOneOf(factorOperators)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseUnary();
      expression = new Binary(expression, oper, right);
    }

    return expression;
  }

  Expression ParseUnary() {
    TokenType[] unaryOperators = [
      TokenType.BANG,
      TokenType.MINUS
    ];
    if (MoveToNextIfMatchOneOf(unaryOperators)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseUnary();

      return new Unary(oper, right);
    }

    return ParsePrimary();
  }

  Expression ParsePrimary() {
    if (MoveToNextIfMatchOneOf(TokenType.FALSE)) {
      return new Literal(false);
    }
    if (MoveToNextIfMatchOneOf(TokenType.TRUE)) {
      return new Literal(true);
    }
    if (MoveToNextIfMatchOneOf(TokenType.NIL)) {
      return new Literal(null);
    }

    if (MoveToNextIfMatchOneOf(TokenType.NUMBER, TokenType.STRING)) {
      return new Literal(PeekPreviousToken().literal);
    }

    if (MoveToNextIfMatchOneOf(TokenType.LEFT_PAREN)) {
      Expression expr = ParseExpression();
      ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after expression.");

      return new Grouping(expr);
    }
  }

  bool MoveToNextIfMatchOneOf(params TokenType[] types) {
    foreach (TokenType type in types) {
      if (CurrentTokenIsTypeOf(type)) {
        MoveToNextToken();
        return true;
      }
    }

    return false;
  }

  bool CurrentTokenIsTypeOf(TokenType type) {
    if (IsAtEnd()) {
      return false;
    }

    return PeekCurrentToken().IsTypeOf(type);
  }

  Token MoveToNextToken() {
    if (!IsAtEnd()) {
      currentTokenIndex++;
    }

    return PeekPreviousToken();
  }

  bool IsAtEnd() {
    var currentToken = PeekCurrentToken();

    return currentToken.IsTypeOf(TokenType.EOF);
  }

  Token PeekCurrentToken() {
    return tokens.ElementAt(currentTokenIndex);
  }

  Token PeekPreviousToken() {
    return tokens.ElementAt(currentTokenIndex - 1);
  }

  Token ReportErrorIfNotMatch(TokenType type, string message) {}
}
