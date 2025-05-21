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
      if (MoveToNextIfMatchOneOf(TokenType.CLASS)) {
        return ParseClassDeclaration();
      }

      if (MoveToNextIfMatchOneOf(TokenType.FUN)) {
        return ParseFunction("function");
      }

      if (MoveToNextIfMatchOneOf(TokenType.VAR)) {
        return ParseVarDeclaration();
      }

      return ParseStatement();
    } catch (ParseError) {
      Synchronize();
      return null;
    }
  }

  Statement ParseClassDeclaration() {
    Token name = ReportErrorIfNotMatch(TokenType.IDENTIFIER, "Expect class name.");
    ReportErrorIfNotMatch(TokenType.LEFT_BRACE, "Expect '{' before class body.");

    List<Function> methods = [];
    while (!CurrentTokenIsTypeOf(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
      methods.Add(ParseFunction("method"));
    }

    ReportErrorIfNotMatch(TokenType.RIGHT_BRACE, "Expect '}' after class body.");

    return new Class(name, methods.ToArray());
  }

  Function ParseFunction(String kind) {
    Token name = ReportErrorIfNotMatch(TokenType.IDENTIFIER, $"Expect {kind} name.");

    ReportErrorIfNotMatch(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");

    List<Token> parameters = [];
    if (!CurrentTokenIsTypeOf(TokenType.RIGHT_PAREN)) {
      do {
        if (parameters.Count >= 5) {
          CreateParseError(PeekCurrentToken(), "Can't have more than 4 parameters.");
        }

        parameters.Add(ReportErrorIfNotMatch(TokenType.IDENTIFIER, "Expect parameter name."));
      } while (MoveToNextIfMatchOneOf(TokenType.COMMA));
    }

    ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

    ReportErrorIfNotMatch(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
    var body = ParseBlock();

    return new Function(name, parameters.ToArray(), body);
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

  Statement ParseWhileStatement() {
    ReportErrorIfNotMatch(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
    Expression condition = ParseExpression();
    ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");

    Statement body = ParseStatement();

    return new While(condition, body);
  }

  Statement ParseStatement() {
    if (MoveToNextIfMatchOneOf(TokenType.FOR)) {
      return ParseForStatement();
    }

    if (MoveToNextIfMatchOneOf(TokenType.IF)) {
      return ParseIfStatement();
    }

    if (MoveToNextIfMatchOneOf(TokenType.PRINT)) {
      return ParsePrintStatement();
    }

    if (MoveToNextIfMatchOneOf(TokenType.RETURN)) {
      return ParseReturnStatement();
    }

    if (MoveToNextIfMatchOneOf(TokenType.WHILE)) {
      return ParseWhileStatement();
    }

    if (MoveToNextIfMatchOneOf(TokenType.LEFT_BRACE)) {
      return new Block(ParseBlock());
    }

    return ParseExpressionStatement();
  }

  Statement ParseForStatement() {
    ReportErrorIfNotMatch(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

    Statement? initializer = null;
    if (MoveToNextIfMatchOneOf(TokenType.SEMICOLON)) {
      initializer = null;
    } else if (MoveToNextIfMatchOneOf(TokenType.VAR)) {
      initializer = ParseVarDeclaration();
    } else {
      initializer = ParseExpressionStatement();
    }

    Expression? condition = null;
    if (!CurrentTokenIsTypeOf(TokenType.SEMICOLON)) {
      condition = ParseExpression();
    }

    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after loop condition.");

    Expression? increment = null;
    if (!CurrentTokenIsTypeOf(TokenType.RIGHT_PAREN)) {
      increment = ParseExpression();
    }

    ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

    Statement body = ParseStatement();

    if (increment is not null) {
      body = new Block([body, new ExpressionStatement(increment)]);
    }

    condition ??= new Literal(true);
    body = new While(condition, body);

    if (initializer is not null) {
      body = new Block([initializer, body]);
    }

    return body;
  }

  Statement ParseIfStatement() {
    ReportErrorIfNotMatch(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
    Expression condition = ParseExpression();
    ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

    Statement thenBranch = ParseStatement();
    Statement? elseBranch = null;
    if (MoveToNextIfMatchOneOf(TokenType.ELSE)) {
      elseBranch = ParseStatement();
    }

    return new If(condition, thenBranch, elseBranch);
  }

  Statement ParsePrintStatement() {
    var value = ParseExpression();
    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after value.");

    return new Print(value);
  }

  Statement ParseReturnStatement() {
    var keyword = PeekPreviousToken();
    Expression? value = null;
    if (!CurrentTokenIsTypeOf(TokenType.SEMICOLON)) {
      value = ParseExpression();
    }

    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after return value.");

    return new Return(keyword, value);
  }

  Statement ParseExpressionStatement() {
    var expression = ParseExpression();
    ReportErrorIfNotMatch(TokenType.SEMICOLON, "Expect ';' after expression.");

    return new ExpressionStatement(expression);
  }

  Statement[] ParseBlock() {
    List<Statement> statements = [];

    while (!CurrentTokenIsTypeOf(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
      var declaration = ParseDeclaration();

      if (declaration is not null) {
        statements.Add(declaration);
      }
    }

    ReportErrorIfNotMatch(TokenType.RIGHT_BRACE, "Expect '}' after block.");

    return statements.ToArray();
  }

  Expression ParseExpression() {
    return ParseAssignment();
  }

  Expression ParseAssignment() {
    Expression expression = ParseOrExpression();

    if (MoveToNextIfMatchOneOf(TokenType.EQUAL)) {
      Token equals = PeekPreviousToken();
      Expression value = ParseAssignment();

      if (expression is Variable) {
        Token name = ((Variable)expression).name;

        return new Assign(name, value);
      } else if (expression is Get getExpression) {
        return new Set(getExpression.obj, getExpression.name, value);
      }

      CreateParseError(equals, "Invalid assignment target."); 
    }

    return expression;
  }

  Expression ParseOrExpression() {
    Expression expression = ParseAndExpression();

    while (MoveToNextIfMatchOneOf(TokenType.OR)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseAndExpression();
      expression = new Logical(expression, oper, right);
    } 

    return expression;
  }

  Expression ParseAndExpression() {
    Expression expression = ParseEquality();

    while (MoveToNextIfMatchOneOf(TokenType.AND)) {
      Token oper = PeekPreviousToken();
      Expression right = ParseEquality();
      expression = new Logical(expression, oper, right);
    }

    return expression;
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

    return ParseCall();
  }

  Expression ParseCall() {
    Expression expression = ParsePrimary();

    while (true) {
      if (MoveToNextIfMatchOneOf(TokenType.LEFT_PAREN)) {
        expression = ParseFinishCall(expression);
      } else if (MoveToNextIfMatchOneOf(TokenType.DOT)) {
        Token name = ReportErrorIfNotMatch(TokenType.IDENTIFIER, "Expect property name after '.'.");
        expression = new Get(expression, name);
      } else {
        break;
      }
    }

    return expression;
  }

  Expression ParseFinishCall(Expression callee) {
    List<Expression> arguments = [];

    if (!CurrentTokenIsTypeOf(TokenType.RIGHT_PAREN)) {
      do {
        if (arguments.Count > 4) {
          CreateParseError(PeekCurrentToken(), "Can't have more than 4 arguments.");
        }

        arguments.Add(ParseExpression());
      } while (MoveToNextIfMatchOneOf(TokenType.COMMA));
    }

    Token rightParen = ReportErrorIfNotMatch(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

    return new Call(callee, rightParen, arguments.ToArray());
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
