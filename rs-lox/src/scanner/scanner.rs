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
        let mut lines: AnyIter<Line> = Box::new(
            input
                // TODO #1 keep newlines inside of LoxString
                .lines()
                // TODO try to fix order of `enumerate -> filter` by types rather then tests
                .enumerate()
                .filter(|(_, l)| !l.is_empty())
                .filter(|(_, l)| !l.trim_start().starts_with("//"))
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

#[cfg(test)]
mod tests {
    use rstest::rstest;

    use super::*;

    #[rstest]
    #[case("\
    4
    \"string\"
    var", vec![1, 2, 3])]
    #[case("\
    ident

    // comment
    true", vec![1, 4])]
    fn token_contains_line(#[case] input: &'static str, #[case] expected: Vec<usize>) {
        let result: Vec<usize> = Scanner::new(input)
            .map(|t| t.pos.line)
            .collect();
        assert_eq!(result, expected);
    }
}
