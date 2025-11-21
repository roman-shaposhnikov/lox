use std::iter::Peekable;

use crate::shared::types::AnyIter;

// TODO: find more idiomatic way to implement this, maybe without extension
// try to use `iter.by_ref().skip_while` instead of this
pub trait IteratorExt {
    type Item;
    fn skip_while_borrow<P>(&mut self, predicate: &mut P) -> &mut Self
        where
            Self: Iterator<Item = <Self as IteratorExt>::Item>,
            P: FnMut(<Self as IteratorExt>::Item) -> bool
    {
        while self.next().is_some_and(|n| predicate(n)) {}
        self
    }
}

impl<T> IteratorExt for AnyIter<T> {
    type Item = T;
}

impl<T> IteratorExt for Peekable<AnyIter<T>> {
    type Item = T;
}
