mod tests;
pub mod token;
pub mod character;

use token::*;
use character::*;
use crate::shared::types::AnyIter;

struct Scanner {
    source: AnyIter<char>,
}

impl Scanner {
    // TODO: try to avoid 'static lifetime
    fn new(input: &'static str) -> Self {
        Self {
            source: Box::new(
                WithoutComments::new(
                    Box::new(
                        input
                            .chars()
                            .filter(|c| *c != ' ')
                            .filter(|c| *c != '\t') // tabulation
                            .filter(|c| *c != '\r') // caret return
                    )
                ).filter(|c| *c != '\n') // newline
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

struct WithoutComments {
    iter: AnyIter<char>,
    // TODO: try to remove this field and use Peekable instead
    prev: Option<char>,
}

impl WithoutComments {
    fn new(iter: AnyIter<char>) -> Self {
        Self {
            iter,
            prev: None,
        }
    }
}

impl Iterator for WithoutComments {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        if self.prev.is_some() {
            return self.prev.take();
        }

        let first = self.iter.next()?;
        if first == '/' {
            let second = self.iter.next();
            if let Some('/') = second {
                // TODO: try to use skip_while instead
                while self.iter.next().is_some_and(|n| n != '\n') {}
                self.iter.next()
            } else {
                self.prev = second;
                Some(first)
            }
        } else {
            Some(first)
        }
    }
}
