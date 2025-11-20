mod tests;
pub mod token;
pub mod character;
pub mod without_comments;
pub mod without_white_space;
pub mod without_new_lines;

use std::{ iter::Peekable };

use token::*;
use character::*;
use without_comments::WithoutComments;
use crate::{
    scanner::{ without_new_lines::WithoutNewLines, without_white_space::WithoutWhiteSpace },
    shared::{ exts::IteratorExt, types::CharIter },
};

type Source = Peekable<CharIter>;

struct Scanner {
    source: Source,
}

impl Scanner {
    // TODO: try to avoid 'static lifetime
    fn new(input: &'static str) -> Self {
        // TODO: the order of this transformation should be fixed by tests or types
        let iter = Box::new(input.chars());
        let iter: CharIter = WithoutWhiteSpace::new(iter);
        let iter: CharIter = WithoutNewLines::new(WithoutComments::new(iter.peekable()));

        Self { source: iter.peekable() }
    }

    fn scan(&mut self) -> TokenKind {
        if let Some(current) = self.source.next() {
            let next = self.source.peek().copied();
            Character::new(current, next).token_kind()
        } else {
            TokenKind::Eof
        }
    }
}

impl Iterator for Scanner {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        let kind = self.scan();
        Some(Token { kind })
    }
}
