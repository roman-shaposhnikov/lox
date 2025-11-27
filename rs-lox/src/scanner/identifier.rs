use crate::shared::exts::PeekableExt;

use super::{ types::Source, keyword::Keyword, token::TokenKind };

pub struct Identifier<'a>(&'a mut Source);

impl<'a> Identifier<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token_kind(&mut self) -> TokenKind {
        // TODO: maybe move to struct?
        let name: String = self.0.take_while_next(|c| c.is_ascii_alphanumeric()).collect();
        Keyword::new(&name).token_kind().unwrap_or(TokenKind::Identifier)
    }
}
