use std::{ iter::Peekable };

use crate::shared::{ types::CharIter };

pub struct SkipComments(Peekable<CharIter>);

impl SkipComments {
    pub fn new(iter: Peekable<CharIter>) -> Box<Self> {
        Box::new(Self(iter))
    }
}

impl Iterator for SkipComments {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        let first = self.0.next()?;
        if first == '/' {
            let second = self.0.peek();
            if let Some('/') = second {
                // A comment goes until the end of the line.
                // TODO: implement it by method skip_line, maybe inside CharIter struct
                self.0
                    .by_ref()
                    .skip_while(|n| *n != '\n')
                    .next();
                self.0.next()
            } else {
                Some(first)
            }
        } else {
            Some(first)
        }
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;

    use super::*;

    #[rstest]
    #[case("5// test", "5")]
    #[case("// whole line", "")]
    #[case("// comment // inside comment", "")]
    fn skip_comments(#[case] input: &'static str, #[case] expected: &'static str) {
        let source: CharIter = Box::new(input.chars());
        let source: CharIter = SkipComments::new(source.peekable());
        let result = source.collect::<String>();
        assert_eq!(result, expected);
    }
}
