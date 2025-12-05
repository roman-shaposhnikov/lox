use rstest::rstest;

use super::Scanner;
use super::token::Token;

#[test]
fn empty_string_produce_zero_tokens() {
    let scanner = Scanner::new("");
    let tokens: Vec<Token> = scanner.collect();
    assert_eq!(tokens.len(), 0);
}

#[rstest]
#[case("4 \"string\" var ", vec![1, 3, 12])]
fn token_contains_col(#[case] input: &'static str, #[case] expected: Vec<usize>) {
    let result: Vec<usize> = Scanner::new(input)
        .map(|t| t.pos.col)
        .collect();
    assert_eq!(result, expected);
}

#[rstest]
#[case("4 \"string\" var ", vec![1, 8, 3])]
fn token_contains_len(#[case] input: &'static str, #[case] expected: Vec<usize>) {
    let result: Vec<usize> = Scanner::new(input)
        .map(|t| t.pos.len)
        .collect();
    assert_eq!(result, expected);
}
