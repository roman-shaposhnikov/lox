// TODO: place currentToken as class field

class Parser(Token[] tokens) {
  readonly Token[] tokens = tokens;
  int currentTokenIndex = 0;

  public Statement?[] Parse() {
    List<Statement?> statements = [];
    while (!IsAtEnd()) {
      statements.Add(ParseDeclaration());
    }

    return statements.ToArray();
  }

  Statement? ParseDeclaration() {
    try {
    if (MoveToNextIfMatchOneOf(TokenType.VAR)) {
        return ParseVarDeclaration();
      }

      return ParseStatement();
    } catch (ParseError) {
      Synchronize();
      return null;
    }
  }

  Statement ParseVarDeclaration() {
    Token name = ReportErrorIfNotMatch(TokenType.IDENTIFIER, "Expect variable name.");

    Expression? initializer = null;
    if (MoveToNextIfMatchOneOf(TokenType.EQUAL)) {
      initializer = ParseExpression();
    }

    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after variable declaration.");

    return new Var(name, initializer);
  }

  Statement ParseStatement() {
    if (MoveToNextIfMatchOneOf(TokenType.PRINT)) {
      return ParsePrintStatement();
    }

    return ParseExpressionStatement();
  }

  Statement ParsePrintStatement() {
    var value = ParseExpression();
    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after value.");

    return new Print(value);
  }

  Statement ParseExpressionStatement() {
    var expression = ParseExpression();
    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after expression.");

    return new ExpressionStatement(expression);
  }

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

    if (MoveToNextIfMatchOneOf(TokenType.IDENTIFIER)) {
      return new Variable(PeekPreviousToken());
    }

    if (MoveToNextIfMatchOneOf(TokenType.LEFT_PAREN)) {
      Expression expr = ParseExpression();
      ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after expression.");

      return new Grouping(expr);
    }

    throw CreateParseError(PeekCurrentToken(), "Expect expression.");
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

  Token ReportErrorIfNotMatch(TokenType type, string message) {
    if (CurrentTokenIsTypeOf(type)) {
      return MoveToNextToken();
    }

    throw CreateParseError(PeekCurrentToken(), message);
  }

  ParseError CreateParseError(Token token, string message) {
    Lox.Error(token, message);

    return new ParseError();
  }

  void Synchronize() {
    MoveToNextToken();

    while (!IsAtEnd()) {
      if (PeekPreviousToken().IsTypeOf(TokenType.SEMICOLON)) {
        return;
      }

      TokenType[] statementStartTokens = [
        TokenType.CLASS,
        TokenType.FUN,
        TokenType.VAR,
        TokenType.FOR,
        TokenType.IF,
        TokenType.WHILE,
        TokenType.PRINT,
        TokenType.RETURN
      ];

      TokenType currentTokenType = PeekCurrentToken().type;
      bool currentTokenStartsStatement = statementStartTokens.Contains(currentTokenType);

      if (currentTokenStartsStatement) {
        return;
      }

      MoveToNextToken();
    }
  }
}
