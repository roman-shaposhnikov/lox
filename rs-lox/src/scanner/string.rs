use crate::scanner::token::TokenKind;

use super::types::Source;

pub struct LoxString<'a>(&'a mut Source);

impl<'a> LoxString<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token_kind(&mut self) -> TokenKind {
        self.0.next(); // consume first quote
        // TODO: should keep newlines inside string somehow
        // but currently it's splits by newline at start
        let mut rest = self.0
            .by_ref()
            .skip_while(|c| *c != '"')
            .peekable();
        if rest.next_if_eq(&'"').is_some() {
            TokenKind::String
        } else {
            TokenKind::Error
        }
    }
}
