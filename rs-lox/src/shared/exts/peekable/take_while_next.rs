use std::iter::Peekable;

pub struct TakeWhileNext<'a, I: Iterator, P>
    where P: FnMut(&<Peekable<I> as Iterator>::Item) -> bool {
    iter: &'a mut I,
    done: bool,
    predicate: P,
}

impl<'a, I: Iterator, P> TakeWhileNext<'a, Peekable<I>, P>
    where P: FnMut(&<Peekable<I> as Iterator>::Item) -> bool
{
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
        if !self.done {
            let next = self.iter.peek()?;
            if (self.predicate)(next) {
                let current = self.iter.next()?;
                Some(current)
            } else {
                self.done = true;
                None
            }
        } else {
            None
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn not_consume_prediction_stop_item() {
        let mut nums = [1, 2, 3, 4, 5, 6].into_iter().peekable();
        let _ = TakeWhileNext::new(&mut nums, |n| *n < 4).collect::<Vec<i32>>();
        assert_eq!(nums.next().unwrap(), 4);
    }

    #[test]
    fn takes_until_prediction_falsy() {
        let mut nums = [1, 2, 3, 4, 5, 6].into_iter().peekable();
        let iter = TakeWhileNext::new(&mut nums, |n| *n < 4);
        assert_eq!(iter.collect::<Vec<i32>>(), [1, 2, 3]);
    }
}
