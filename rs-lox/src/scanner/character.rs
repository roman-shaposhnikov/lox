use super::token::TokenKind;

pub struct Character {
    value: char,
}

impl Character {
    pub fn new(value: char) -> Self {
        Self { value }
    }

    pub fn token_kind(&self) -> TokenKind {
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
