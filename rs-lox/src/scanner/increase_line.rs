use crate::shared::types::CharIter;

pub struct IncreaseLine(CharIter);

impl IncreaseLine {
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

impl Iterator for IncreaseLine {
    type Item = char;

    fn next(&mut self) -> Option<Self::Item> {
        self.0.next()
    }
}
