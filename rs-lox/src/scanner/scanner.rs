use crate::shared::types::AnyIter;

use super::token::Token;
use super::line::Line;

pub struct Scanner {
    lines: AnyIter<Line>,
    current: Option<Line>,
}

// TODO: should avoid scanner and move it logic to Script object?
impl Scanner {
    // TODO: try to avoid 'static lifetime
    pub fn new(input: &'static str) -> Self {
        // TODO: should make it fuse?
        let mut lines: AnyIter<Line> = Box::new(
            input
                // TODO #1 keep newlines inside of LoxString
                .lines()
                .filter(|l| !l.is_empty())
                .enumerate()
                .map(Line::new)
        );
        Self { current: lines.next(), lines }
    }
}

impl Iterator for Scanner {
    type Item = Token;

    fn next(&mut self) -> Option<Self::Item> {
        if let Some(line) = &mut self.current {
            line.next().or_else(|| {
                self.current = self.lines.next();
                self.next()
            })
        } else {
            None
        }
    }
}
