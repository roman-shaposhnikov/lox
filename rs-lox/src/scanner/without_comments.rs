use crate::shared::types::AnyIter;

pub struct WithoutComments {
    iter: AnyIter<char>,
    // TODO: try to remove this field and use Peekable instead
    prev: Option<char>,
}

impl WithoutComments {
    pub fn new(iter: AnyIter<char>) -> Self {
        Self {
            iter,
            prev: None,
        }
    }
}

impl Iterator for WithoutComments {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        if self.prev.is_some() {
            return self.prev.take();
        }

        let first = self.iter.next()?;
        if first == '/' {
            let second = self.iter.next();
            if let Some('/') = second {
                // TODO: try to use skip_while instead
                while self.iter.next().is_some_and(|n| n != '\n') {}
                self.iter.next()
            } else {
                self.prev = second;
                Some(first)
            }
        } else {
            Some(first)
        }
    }
}
