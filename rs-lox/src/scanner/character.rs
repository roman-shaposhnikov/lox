use super::token::TokenKind;

pub struct Character {
    current: char,
    next: Option<char>,
}

impl Character {
    pub fn new(current: char, next: Option<char>) -> Self {
        Self { current, next }
    }

    pub fn token_kind(&self) -> TokenKind {
        match self.current {
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
            '!' => {
                if self.next_is('=') { TokenKind::BangEqual } else { TokenKind::Bang }
            }
            '=' => {
                if self.next_is('=') { TokenKind::EqualEqual } else { TokenKind::Equal }
            }
            '>' => {
                if self.next_is('=') { TokenKind::GreaterEqual } else { TokenKind::Greater }
            }
            '<' => {
                if self.next_is('=') { TokenKind::LessEqual } else { TokenKind::Less }
            }
            _ => TokenKind::Error,
        }
    }

    fn next_is(&self, expected: char) -> bool {
        self.next.is_some_and(|c| c == expected)
    }
}
