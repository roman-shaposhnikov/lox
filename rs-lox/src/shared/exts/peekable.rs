mod take_while_next;

use std::iter::Peekable;

use take_while_next::TakeWhileNext;

pub trait PeekableExt {
    fn take_while_next<P>(&mut self, predicate: P) -> TakeWhileNext<Self, P>
        where Self: Iterator + Sized, P: FnMut(&Self::Item) -> bool;
}

impl<I: Iterator + Sized> PeekableExt for Peekable<I> {
    fn take_while_next<P>(&mut self, predicate: P) -> TakeWhileNext<Self, P>
        where Self: Iterator + Sized, P: FnMut(&<Peekable<I> as Iterator>::Item) -> bool
    {
        TakeWhileNext::new(self, predicate)
    }
}
