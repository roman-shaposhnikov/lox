use crate::scanner::token::TokenKind;

use super::types::Source;

pub struct Number<'a>(&'a mut Source);

impl<'a> Number<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token_kind(&mut self) -> TokenKind {
        TokenKind::Number
    }
}
