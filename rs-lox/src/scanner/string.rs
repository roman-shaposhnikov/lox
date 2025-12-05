use crate::shared::exts::PeekableExt;

use super::{ types::Source, token::TokenKind, sequence::SeqToken };

pub struct LoxString<'a>(&'a mut Source);

impl<'a> LoxString<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token(&mut self) -> SeqToken {
        self.0.next(); // consume first quote
        // TODO #1 should keep newlines inside string somehow
        // but currently it's splits by newline at start
        let raw = self.0
            .by_ref()
            .take_while_next(|(_, c)| *c != '"')
            .map(|c| c.1)
            .collect::<String>();
        let quoted_len = raw.len() + 2;
        let kind = if self.0.next_if(|(_, c)| *c == '"').is_some() {
            TokenKind::String(raw)
        } else {
            TokenKind::Error
        };
        SeqToken { kind, len: quoted_len }
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;
    use crate::shared::types::CharIter;

    use super::*;

    #[test]
    fn produce_error_token_on_unterminated_string() {
        let input = "\"start";
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = LoxString::new(&mut source).token();
        assert_eq!(token.kind, TokenKind::Error);
    }

    #[rstest]
    #[case("\"\"", TokenKind::String(String::from("")))]
    #[ignore] // until #1 issue
    #[case("\"test\nstring\"", TokenKind::String(String::from("test\nstring")))]
    #[case("\"-string 'with' quotes?\"", TokenKind::String(String::from("-string 'with' quotes?")))]
    #[case("\"12341\"", TokenKind::String(String::from("12341")))]
    fn match_value(#[case] input: &'static str, #[case] expected: TokenKind) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = LoxString::new(&mut source).token();
        assert_eq!(token.kind, expected);
    }
}
