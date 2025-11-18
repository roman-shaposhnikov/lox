mod tests;
pub mod token;
pub mod character;
pub mod without_comments;

use std::iter::Peekable;

use token::*;
use character::*;
use without_comments::WithoutComments;
use crate::shared::types::AnyIter;

struct Scanner {
    source: Peekable<AnyIter<char>>,
}

impl Scanner {
    // TODO: try to avoid 'static lifetime
    fn new(input: &'static str) -> Self {
        let iter: AnyIter<char> = Box::new(
            input
                .chars()
                .filter(|c| *c != ' ')
                .filter(|c| *c != '\t') // tabulation
                .filter(|c| *c != '\r') // caret return
        );
        let iter: AnyIter<char> = Box::new(
            WithoutComments::new(iter.peekable()).filter(|c| *c != '\n') // newline
        );

        Self {
            source: iter.peekable(),
        }
    }

    fn scan_token_kind(&mut self) -> TokenKind {
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
        let kind = self.scan_token_kind();
        Some(Token { kind })
    }
}
