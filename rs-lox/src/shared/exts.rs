use std::iter::Peekable;

pub trait PeekableExt<I: Iterator> {
    fn take_while_next<P>(&mut self, predicate: P) -> TakeWhileNext<Self, P>
        where Self: Iterator + Sized, P: FnMut(&I::Item) -> bool;
}

impl<I: Iterator + Sized> PeekableExt<I> for Peekable<I> {
    fn take_while_next<P>(&mut self, predicate: P) -> TakeWhileNext<Self, P>
        where Self: Sized, P: FnMut(&I::Item) -> bool
    {
        TakeWhileNext::new(self, predicate)
    }
}

pub struct TakeWhileNext<'a, I: Iterator, P> {
    iter: &'a mut I,
    done: bool,
    predicate: P,
}

impl<'a, I: Iterator, P> TakeWhileNext<'a, Peekable<I>, P> {
    pub fn new(iter: &'a mut Peekable<I>, predicate: P) -> Self {
        Self { iter, done: false, predicate }
    }
}

impl<'a, I: Iterator, P> Iterator
    for TakeWhileNext<'a, Peekable<I>, P>
    where P: FnMut(&I::Item) -> bool
{
    type Item = I::Item;

    fn next(&mut self) -> Option<I::Item> {
        if self.done {
            None
        } else {
            let next = self.iter.peek()?;
            if (self.predicate)(next) {
                let current = self.iter.next()?;
                Some(current)
            } else {
                self.done = true;
                None
            }
        }
    }
}
