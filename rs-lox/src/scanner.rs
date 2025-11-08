#[derive(PartialEq, Debug)]
enum TokenKind {
    EOF,
}

struct Token {
    kind: TokenKind,
}

struct Scanner {}

impl Scanner {
    fn new(source: String) -> Self {
        Self {}
    }

    fn scanToken(&self) -> Token {
        Token {
            kind: TokenKind::EOF,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn empty_string_contains_eof_at_first_place() {
        let scanner = Scanner::new(String::from(""));
        let token = scanner.scanToken();
        assert_eq!(token.kind, TokenKind::EOF);
    }
}
