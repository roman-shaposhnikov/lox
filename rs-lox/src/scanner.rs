mod tests;
pub mod types;
pub mod token;
pub mod character;
pub mod without_comments;
pub mod without_white_space;
pub mod without_new_lines;
pub mod identifier;
pub mod keyword;
pub mod number;
pub mod string;

use token::*;
use character::*;
use without_comments::WithoutComments;
use identifier::Identifier;
use types::Source;
use number::Number;
use string::LoxString;
use without_new_lines::WithoutNewLines;
use without_white_space::WithoutWhiteSpace;
use crate::shared::types::CharIter;

pub struct Scanner {
    source: Source,
    done: bool,
}

impl Scanner {
    // TODO: try to avoid 'static lifetime
    pub fn new(input: &'static str) -> Self {
        // TODO: the order of this transformation should be fixed by tests or types
        let iter = Box::new(input.chars());
        let iter: CharIter = WithoutWhiteSpace::new(iter);
        let iter: CharIter = WithoutNewLines::new(WithoutComments::new(iter.peekable()));

        Self { source: iter.peekable(), done: false }
    }

    fn scan(&mut self) -> TokenKind {
        let current = self.source.peek().copied();
        if let Some(c) = current {
            self.match_token(c)
        } else {
            self.done = true;
            TokenKind::Eof
        }
    }

    // TODO: move logic to Token struct?
    fn match_token(&mut self, current: char) -> TokenKind {
        if current.is_ascii_alphabetic() {
            Identifier::new(&mut self.source).token_kind()
        } else if current.is_ascii_digit() {
            Number::new(&mut self.source).token_kind()
        } else if current == '"' {
            LoxString::new(&mut self.source).token_kind()
        } else {
            Character::new(&mut self.source).token_kind()
        }
    }
}

impl Iterator for Scanner {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        if self.done {
            None
        } else {
            let kind = self.scan();
            Some(Token { kind })
        }
    }
}
