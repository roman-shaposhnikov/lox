use std::{ iter::Peekable };

use crate::shared::{ types::CharIter, exts::IteratorExt };

pub struct WithoutComments(Peekable<CharIter>);

impl WithoutComments {
    pub fn new(iter: Peekable<CharIter>) -> Box<Self> {
        Box::new(Self(iter))
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

#[cfg(test)]
mod tests {
    // TODO: what is supposed to be placed here?
    // should i move some comments tests from scanner?
}
