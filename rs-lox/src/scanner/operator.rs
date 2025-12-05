use super::{ types::Source, token::TokenKind, sequence::SeqToken };

pub struct Operator<'a>(&'a mut Source);

impl<'a> Operator<'a> {
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token(&mut self) -> SeqToken {
        let (length, kind) = self.token_kind();
        SeqToken { kind, len: length }
    }

    fn token_kind(&mut self) -> (usize, TokenKind) {
        let (_, next) = self.0.next().expect("Operator char exists");
        match next {
            '(' => (1, TokenKind::LeftParen),
            ')' => (1, TokenKind::RightParen),
            '{' => (1, TokenKind::LeftBrace),
            '}' => (1, TokenKind::RightBrace),
            ';' => (1, TokenKind::Semicolon),
            ',' => (1, TokenKind::Comma),
            '.' => (1, TokenKind::Dot),
            '-' => (1, TokenKind::Minus),
            '+' => (1, TokenKind::Plus),
            '/' => (1, TokenKind::Slash),
            '*' => (1, TokenKind::Star),
            '!' => {
                if self.next_is('=') { (2, TokenKind::BangEqual) } else { (1, TokenKind::Bang) }
            }
            '=' => {
                if self.next_is('=') { (2, TokenKind::EqualEqual) } else { (1, TokenKind::Equal) }
            }
            '>' => {
                if self.next_is('=') {
                    (2, TokenKind::GreaterEqual)
                } else {
                    (1, TokenKind::Greater)
                }
            }
            '<' => {
                if self.next_is('=') { (2, TokenKind::LessEqual) } else { (1, TokenKind::Less) }
            }
            _ => (1, TokenKind::Error),
        }
    }

    fn next_is(&mut self, expected: char) -> bool {
        let tmp = &expected;
        self.0.next_if(|(_, next)| next == tmp).is_some()
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;

    use crate::shared::types::CharIter;

    use super::*;

    #[rstest]
    #[case("(", TokenKind::LeftParen)]
    #[case(")", TokenKind::RightParen)]
    #[case("{", TokenKind::LeftBrace)]
    #[case("}", TokenKind::RightBrace)]
    #[case(",", TokenKind::Comma)]
    #[case(".", TokenKind::Dot)]
    #[case("-", TokenKind::Minus)]
    #[case("+", TokenKind::Plus)]
    #[case(";", TokenKind::Semicolon)]
    #[case("/", TokenKind::Slash)]
    #[case("*", TokenKind::Star)]
    fn match_single_character_token(#[case] input: &'static str, #[case] expected: TokenKind) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Operator::new(&mut source).token();
        assert_eq!(token.kind, expected);
    }

    #[rstest]
    #[case("!", TokenKind::Bang)]
    #[case("!=", TokenKind::BangEqual)]
    #[case("=", TokenKind::Equal)]
    #[case("==", TokenKind::EqualEqual)]
    #[case(">", TokenKind::Greater)]
    #[case(">=", TokenKind::GreaterEqual)]
    #[case("<", TokenKind::Less)]
    #[case("<=", TokenKind::LessEqual)]
    fn match_one_or_two_character_token(#[case] input: &'static str, #[case] expected: TokenKind) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Operator::new(&mut source).token();
        assert_eq!(token.kind, expected);
    }
}
