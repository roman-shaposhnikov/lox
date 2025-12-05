use crate::scanner::sequence::{ LineToken, Sequence };
use crate::scanner::token::Position;
use crate::shared::types::CharIter;

use super::token::Token;
use super::skip_comments::SkipComments;

pub struct Line {
    index: usize,
    sequence: Sequence,
}

impl Line {
    // TODO: try to avoid 'static lifetime
    pub fn new((index, source): (usize, &'static str)) -> Self {
        let source: CharIter = Box::new(source.chars());
        let source: CharIter = SkipComments::new(source.peekable());
        Self { index, sequence: Sequence::new(source) }
    }
}

impl Iterator for Line {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        let LineToken { kind, col, len } = self.sequence.next()?;
        let pos = Position { line: self.index + 1, col, len };
        Some(Token { kind, pos })
    }
}
