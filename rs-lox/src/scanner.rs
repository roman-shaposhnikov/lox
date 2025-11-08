use std::{ str::Chars };

#[derive(PartialEq, Debug)]
enum TokenKind {
    Eof,
    // Single-character tokens.
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Semicolon,
    Slash,
    Star,
}

struct Token {
    kind: TokenKind,
}

struct Scanner<'a> {
    source: Chars<'a>,
}

impl<'a> Scanner<'a> {
    fn new(source: &'a str) -> Self {
        Self {
            source: source.chars(),
        }
    }

    fn scan_token(&mut self) -> Result<Token, ()> {
        if let Some(kind) = self.scan_token_kind() { Ok(Token { kind }) } else { Err(()) }
    }

    fn scan_token_kind(&mut self) -> Option<TokenKind> {
        if let Some(value) = self.source.next() {
            let c = Character::new(value);
            c.token_kind()
        } else {
            Some(TokenKind::Eof)
        }
    }
}

struct Character {
    value: char,
}

impl Character {
    fn new(value: char) -> Self {
        Self { value }
    }

    fn token_kind(&self) -> Option<TokenKind> {
        match self.value {
            '(' => Some(TokenKind::LeftParen),
            ')' => Some(TokenKind::RightParen),
            '{' => Some(TokenKind::LeftBrace),
            '}' => Some(TokenKind::RightBrace),
            ';' => Some(TokenKind::Semicolon),
            ',' => Some(TokenKind::Comma),
            '.' => Some(TokenKind::Dot),
            '-' => Some(TokenKind::Minus),
            '+' => Some(TokenKind::Plus),
            '/' => Some(TokenKind::Slash),
            '*' => Some(TokenKind::Star),
            _ => None,
        }
    }
}

#[cfg(test)]
mod tests {
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
}
