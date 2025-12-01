use crate::shared::types::CharIter;

use super::token::{ Token, TokenKind };
use super::operator::Operator;
use super::without_comments::WithoutComments;
use super::identifier::Identifier;
use super::types::Source;
use super::number::Number;
use super::string::LoxString;
use super::without_white_space::WithoutWhiteSpace;

pub struct Line {
    index: usize,
    source: Source,
}

impl Line {
    // TODO: try to avoid 'static lifetime
    pub fn new((index, source): (usize, &'static str)) -> Self {
        // TODO: the order of this transformation should be fixed by tests or types
        let iter = Box::new(source.chars());
        // TODO: keep whitespaces inside LoxString
        let iter: CharIter = WithoutWhiteSpace::new(iter);
        let iter: CharIter = WithoutComments::new(iter.peekable());
        Self { index, source: iter.peekable() }
    }

    fn scan(&mut self) -> Option<TokenKind> {
        let current = self.source.peek().copied()?;
        Some(self.match_token(current))
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
            Operator::new(&mut self.source).token_kind()
        }
    }
}

impl Iterator for Line {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        let kind = self.scan()?;
        Some(Token { kind, line: self.index + 1 })
    }
}
