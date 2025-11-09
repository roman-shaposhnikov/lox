mod tests;

#[derive(PartialEq, Debug)]
enum TokenKind {
    Eof,
    Error,
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

struct Scanner {
    source: Box<dyn Iterator<Item = char>>,
}

impl Scanner {
    // TODO: try to avoid 'static lifetime
    fn new(input: &'static str) -> Self {
        Self {
            source: Box::new(
                input
                    .chars()
                    .filter(|c| *c != ' ')
                    .filter(|c| *c != '\n') // newline
                    .filter(|c| *c != '\t') // tabulation
                    .filter(|c| *c != '\r') // caret return
            ),
        }
    }

    fn scan_token_kind(&mut self) -> TokenKind {
        if let Some(value) = self.source.next() {
            Character::new(value).token_kind()
        } else {
            TokenKind::Eof
        }
    }
}

impl Iterator for Scanner {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        let kind = self.scan_token_kind();
        Some(Token { kind })
    }
}

struct Character {
    value: char,
}

impl Character {
    fn new(value: char) -> Self {
        Self { value }
    }

    fn token_kind(&self) -> TokenKind {
        match self.value {
            '(' => TokenKind::LeftParen,
            ')' => TokenKind::RightParen,
            '{' => TokenKind::LeftBrace,
            '}' => TokenKind::RightBrace,
            ';' => TokenKind::Semicolon,
            ',' => TokenKind::Comma,
            '.' => TokenKind::Dot,
            '-' => TokenKind::Minus,
            '+' => TokenKind::Plus,
            '/' => TokenKind::Slash,
            '*' => TokenKind::Star,
            _ => TokenKind::Error,
        }
    }
}
