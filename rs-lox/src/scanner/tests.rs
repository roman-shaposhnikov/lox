use rstest::rstest;

use super::Scanner;
use super::token::{ Token, TokenKind };

#[test]
fn empty_string_produce_zero_tokens() {
    let scanner = Scanner::new("");
    let tokens: Vec<Token> = scanner.collect();
    assert_eq!(tokens.len(), 0);
}

fn match_token_kind(input: &'static str, expected: TokenKind) {
    let mut scanner = Scanner::new(input);
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, expected, "input \"{input}\" to be a {expected:?} token");
}

#[rstest]
#[case("(", TokenKind::LeftParen)]
#[case(")", TokenKind::RightParen)]
#[case("{", TokenKind::LeftBrace)]
#[case("}", TokenKind::RightBrace)]
#[case(",", TokenKind::Comma)]
#[case(".", TokenKind::Dot)]
#[case("-", TokenKind::Minus)]
#[case("+", TokenKind::Plus)]
#[case(";", TokenKind::Semicolon)]
#[case("/", TokenKind::Slash)]
#[case("*", TokenKind::Star)]
fn match_single_character_token(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token_kind(input, expected)
}

#[rstest]
#[case("!", TokenKind::Bang)]
#[case("!=", TokenKind::BangEqual)]
#[case("=", TokenKind::Equal)]
#[case("==", TokenKind::EqualEqual)]
#[case(">", TokenKind::Greater)]
#[case(">=", TokenKind::GreaterEqual)]
#[case("<", TokenKind::Less)]
#[case("<=", TokenKind::LessEqual)]
fn match_one_or_two_character_token(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token_kind(input, expected)
}

#[rstest]
#[case("         ,", TokenKind::Comma)]
#[case(")         ", TokenKind::RightParen)]
#[case("    /     ", TokenKind::Slash)]
#[case("    
    +", TokenKind::Plus)] // newline
#[case("\n+", TokenKind::Plus)] // newline
#[case("	{", TokenKind::LeftBrace)] // tabulation
#[case("\t{", TokenKind::LeftBrace)] // tabulation
#[case("\r abc", TokenKind::Identifier)] // caret return
fn skip_whitespace(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token_kind(input, expected);
}

#[rstest]
#[case("5// test", TokenKind::Number)]
#[case("\
// whole line
*", TokenKind::Star)]
#[case("\
// comment // inside comment
-", TokenKind::Minus)]
fn skip_comments(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token_kind(input, expected);
}

#[rstest]
#[case("and", TokenKind::And)]
#[case("class", TokenKind::Class)]
#[case("else", TokenKind::Else)]
#[case("if", TokenKind::If)]
#[case("nil", TokenKind::Nil)]
#[case("or", TokenKind::Or)]
#[case("print", TokenKind::Print)]
#[case("return", TokenKind::Return)]
#[case("super", TokenKind::Super)]
#[case("var", TokenKind::Var)]
#[case("while", TokenKind::While)]
#[case("false", TokenKind::False)]
#[case("for", TokenKind::For)]
#[case("fun", TokenKind::Fun)]
#[case("this", TokenKind::This)]
#[case("true", TokenKind::True)]
fn match_keyword(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token_kind(input, expected);
}

#[test]
fn match_identifier() {
    match_token_kind("notKeyword", TokenKind::Identifier);
}

#[rstest]
#[case("42")]
#[case("123.124")]
#[case("0")]
#[case("5")]
fn match_number(#[case] input: &'static str) {
    match_token_kind(input, TokenKind::Number);
}

#[rstest]
#[case("\"\"")]
#[case("\"str\"asdfasf")]
#[ignore] // until #1 issue
#[case("\"test\nstring\"")]
#[case("\"-string 'with' quotes?\"")]
#[case("\"12341\"")]
fn match_string(#[case] input: &'static str) {
    match_token_kind(input, TokenKind::String);
}

#[rstest]
#[case("\"\"", vec![TokenKind::String])]
// match identifier after string
#[case("\"str\"asdfasf", vec![TokenKind::String, TokenKind::Identifier])]
#[case("asdfasf\"str\"", vec![TokenKind::Identifier, TokenKind::String])]
#[ignore] // until #1 issue
#[case("\"test\nstring\"", vec![TokenKind::String])]
#[case("\"-string 'with' quotes?\"", vec![TokenKind::String])]
#[case("\"12341\"", vec![TokenKind::String])]
// match identifier after number
#[case("1 a", vec![TokenKind::Number, TokenKind::Identifier])]
fn match_sequence(#[case] input: &'static str, #[case] expected: Vec<TokenKind>) {
    let result: Vec<TokenKind> = Scanner::new(input)
        .map(|t| t.kind)
        .collect();
    assert_eq!(
        result,
        expected,
        "input {input} with a sequence {result:?} be equal to {expected:?}"
    );
}

#[test]
fn produce_error_token_on_unterminated_string() {
    match_token_kind("\"start", TokenKind::Error);
}

#[rstest]
#[case("
    4
    \"string\"
    var
", vec![1, 2, 3])]
#[case("
    ident
    // comment
    true
", vec![1, 3])]
fn token_contains_line(#[case] input: &'static str, #[case] expected: Vec<usize>) {
    let result: Vec<usize> = Scanner::new(input)
        .map(|t| t.line)
        .collect();
    assert_eq!(result, expected);
}
