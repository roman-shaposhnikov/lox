use std::iter::Peekable;

use crate::shared::types::AnyIter;

pub trait PeekableExt {
    type Item;
    fn take_while_next<P>(&mut self, predicate: &mut P) -> &mut Self
        where
            Self: Iterator<Item = <Self as PeekableExt>::Item>,
            P: FnMut(<Self as PeekableExt>::Item) -> bool;
}

impl<T> PeekableExt for Peekable<AnyIter<T>> {
    type Item = T;
    fn take_while_next<P>(&mut self, predicate: &mut P) -> &mut Self {
        if predicate(self.peek()) {
            while self.next().is_some_and(|n| predicate(n)) {}
        }
        self
    }
}

// Pass element until predicate returns true on peek() item
struct TakeWhileNext<I, P>() {
    iter: I,
    done: bool,
    predicate: P,
}
