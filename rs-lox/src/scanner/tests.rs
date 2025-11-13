use rstest::rstest;

use super::Scanner;
use super::token::TokenKind;

#[test]
fn empty_string_contains_eof_at_first_place() {
    let mut scanner = Scanner::new("");
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, TokenKind::Eof);
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
fn match_single_character(#[case] input: &'static str, #[case] expected: TokenKind) {
    let mut scanner = Scanner::new(input);
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, expected, "input {input} to be a {expected:?} token");
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
    let mut scanner = Scanner::new(input);
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, expected);
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
    let mut scanner = Scanner::new(input);
    let token = scanner.next().unwrap();
    assert_eq!(token.kind, expected);
}
