use std::iter::Peekable;

use crate::shared::{ types::AnyIter, exts::IteratorExt };

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
                self.0.skip_while_borrow(&mut (|n| n != '\n')).next()
            } else {
                Some(first)
            }
        } else {
            Some(first)
        }
    }
}
