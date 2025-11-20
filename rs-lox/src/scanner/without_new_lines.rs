use crate::{ shared::{ types::{ CharIter } } };

pub struct WithoutNewLines(CharIter);

impl WithoutNewLines {
    pub fn new(source: CharIter) -> Box<Self> {
        Box::new(Self(Box::new(source.filter(|c| *c != '\n'))))
    }
}

impl Iterator for WithoutNewLines {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        self.0.next()
    }
}
