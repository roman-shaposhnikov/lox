use crate::shared::types::CharIter;

pub struct WithoutWhiteSpace(CharIter);

impl WithoutWhiteSpace {
    pub fn new(source: CharIter) -> Box<Self> {
        Box::new(
            Self(
                Box::new(
                    source
                        .filter(|c| *c != ' ')
                        .filter(|c| *c != '\t') // tabulation
                        .filter(|c| *c != '\r') // caret return
                )
            )
        )
    }
}

impl Iterator for WithoutWhiteSpace {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        self.0.next()
    }
}
