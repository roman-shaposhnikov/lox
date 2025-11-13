#[derive(PartialEq, Debug)]
pub enum TokenKind {
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

pub struct Token {
    pub kind: TokenKind,
}
