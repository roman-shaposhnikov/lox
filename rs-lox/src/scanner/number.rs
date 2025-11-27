use crate::{ scanner::token::TokenKind, shared::exts::PeekableExt };

use super::types::Source;

pub struct Number<'a>(&'a mut Source);

impl<'a> Number<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token_kind(&mut self) -> TokenKind {
        self.0.take_while_next(|c| c.is_ascii_digit()).collect::<String>();
        TokenKind::Number
    }
}
