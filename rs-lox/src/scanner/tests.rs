use rstest::rstest;

use super::Scanner;
use super::token::TokenKind;

#[test]
fn empty_string_contains_eof_at_first_place() {
    let mut scanner = Scanner::new("");
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, TokenKind::Eof);
}

fn match_token(input: &'static str, expected: TokenKind) {
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
    match_token(input, expected)
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
    match_token(input, expected)
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
#[case("\r", TokenKind::Eof)] // caret return
fn skip_whitespace(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token(input, expected);
}

#[rstest]
#[case("// test", TokenKind::Eof)]
#[case("\
// whole line
*", TokenKind::Star)]
#[case("\
// comment // inside comment
-", TokenKind::Minus)]
fn skip_comments(#[case] input: &'static str, #[case] expected: TokenKind) {
    match_token(input, expected);
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
    match_token(input, expected);
}

#[test]
fn match_identifier() {
    match_token("notKeyword", TokenKind::Identifier);
}
