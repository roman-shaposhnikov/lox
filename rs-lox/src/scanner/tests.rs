use rstest::rstest;

use super::*;

#[test]
fn empty_string_contains_eof_at_first_place() {
    let mut scanner = Scanner::new("");
    let token = scanner.scan_token().unwrap();
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
fn match_single_character(#[case] input: &str, #[case] expected: TokenKind) {
    let mut scanner = Scanner::new(input);
    let token = scanner
        .scan_token()
        .expect(&format!("unmatched character in test data -- {input}"));
    assert_eq!(token.kind, expected, "input {input} to be a {expected:?} token");
}
