use std::iter::Peekable;

use crate::shared::types::AnyIter;

pub struct WithoutComments(Peekable<AnyIter<char>>);

impl WithoutComments {
    pub fn new(iter: Peekable<AnyIter<char>>) -> Self {
        Self(iter)
    }
}

impl Iterator for WithoutComments {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        let first = self.0.next()?;
        if first == '/' {
            let second = self.0.peek();
            if let Some('/') = second {
                // TODO: try to use skip_while instead
                while self.0.next().is_some_and(|n| n != '\n') {}
                self.0.next()
            } else {
                Some(first)
            }
        } else {
            Some(first)
        }
    }
}
