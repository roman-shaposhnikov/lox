use crate::scanner::types::Source;

use super::token::TokenKind;

pub struct Operator<'a>(&'a mut Source);

impl<'a> Operator<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token_kind(&mut self) -> TokenKind {
        match self.0.next() {
            Some('(') => TokenKind::LeftParen,
            Some(')') => TokenKind::RightParen,
            Some('{') => TokenKind::LeftBrace,
            Some('}') => TokenKind::RightBrace,
            Some(';') => TokenKind::Semicolon,
            Some(',') => TokenKind::Comma,
            Some('.') => TokenKind::Dot,
            Some('-') => TokenKind::Minus,
            Some('+') => TokenKind::Plus,
            Some('/') => TokenKind::Slash,
            Some('*') => TokenKind::Star,
            Some('!') => {
                if self.next_is('=') { TokenKind::BangEqual } else { TokenKind::Bang }
            }
            Some('=') => {
                if self.next_is('=') { TokenKind::EqualEqual } else { TokenKind::Equal }
            }
            Some('>') => {
                if self.next_is('=') { TokenKind::GreaterEqual } else { TokenKind::Greater }
            }
            Some('<') => {
                if self.next_is('=') { TokenKind::LessEqual } else { TokenKind::Less }
            }
            _ => TokenKind::Error,
        }
    }

    fn next_is(&mut self, expected: char) -> bool {
        let tmp = &expected;
        self.0.next_if_eq(tmp).is_some()
    }
}
