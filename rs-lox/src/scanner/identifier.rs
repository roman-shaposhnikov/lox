use crate::shared::exts::PeekableExt;

use super::{ types::Source, token::TokenKind, sequence::SeqToken };

pub struct Identifier(String);

impl Identifier {
    /// Create a new Identifier parser.
    ///
    /// # Panics
    ///
    /// The `new` function will panic if the source starts with non ascii alphabetic char.
    pub fn new(source: &mut Source) -> Self {
        let starts_with_letter = source.peek().is_some_and(|(_, c)| c.is_ascii_alphabetic());
        assert!(starts_with_letter, "first char of identifier should be letter");
        let name = source
            .take_while_next(|c| c.1.is_ascii_alphanumeric())
            .map(|c| c.1)
            .collect::<String>();
        Self(name)
    }

    pub fn token(self) -> SeqToken {
        let (len, kind) = self.token_kind();
        SeqToken { kind, len }
    }

    fn token_kind(self) -> (usize, TokenKind) {
        match self.0.as_str() {
            "and" => (3, TokenKind::And),
            "class" => (5, TokenKind::Class),
            "else" => (4, TokenKind::Else),
            "if" => (2, TokenKind::If),
            "nil" => (3, TokenKind::Nil),
            "or" => (2, TokenKind::Or),
            "print" => (5, TokenKind::Print),
            "return" => (6, TokenKind::Return),
            "super" => (5, TokenKind::Super),
            "var" => (3, TokenKind::Var),
            "while" => (5, TokenKind::While),
            "false" => (5, TokenKind::False),
            "for" => (3, TokenKind::For),
            "fun" => (3, TokenKind::Fun),
            "this" => (4, TokenKind::This),
            "true" => (4, TokenKind::True),
            name => (name.len(), TokenKind::Identifier(self.0)),
        }
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;

    use crate::shared::types::CharIter;

    use super::*;

    #[rstest]
    #[should_panic]
    #[case("")]
    #[should_panic]
    #[case("5")]
    fn panic_if_starts_with_non_ascii_alphabetic(#[case] input: &'static str) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        Identifier::new(&mut source);
    }

    #[rstest]
    #[case("and", TokenKind::And)]
    #[case("class", TokenKind::Class)]
    #[case("else", TokenKind::Else)]
    #[case("if", TokenKind::If)]
    #[case("nil", TokenKind::Nil)]
    #[case("or", TokenKind::Or)]
    #[case("print", TokenKind::Print)]
    #[case("return", TokenKind::Return)]
    #[case("super", TokenKind::Super)]
    #[case("var", TokenKind::Var)]
    #[case("while", TokenKind::While)]
    #[case("false", TokenKind::False)]
    #[case("for", TokenKind::For)]
    #[case("fun", TokenKind::Fun)]
    #[case("this", TokenKind::This)]
    #[case("true", TokenKind::True)]
    fn match_keyword(#[case] input: &'static str, #[case] expected: TokenKind) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Identifier::new(&mut source).token();
        assert_eq!(token.kind, expected);
    }

    #[rstest]
    #[case("notKeyword")]
    #[case("a")]
    #[case("ab")]
    fn match_identifier(#[case] input: &'static str) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Identifier::new(&mut source).token();
        assert_eq!(token.kind, TokenKind::Identifier(String::from(input)));
    }

    #[test]
    fn stops_parsing_identifier_on_non_ascii_alphabetic() {
        let chars: CharIter = Box::new("name=".chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Identifier::new(&mut source).token();
        assert_eq!(token.kind, TokenKind::Identifier(String::from("name")));
    }

    #[rstest]
    #[case("test", "")]
    #[case("a b", " b")]
    fn consumes_n_chars_from_source(#[case] input: &'static str, #[case] expected: &'static str) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        Identifier::new(&mut source).token();
        let rest = source.map(|(_, c)| c).collect::<String>();
        assert_eq!(rest, expected);
    }
}
