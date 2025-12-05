use crate::shared::exts::PeekableExt;

use super::{ types::Source, token::TokenKind, sequence::SeqToken };

pub struct Number<'a>(&'a mut Source);

impl<'a> Number<'a> {
    // TODO: panic if first char non ascii digit
    pub fn new(start: &'a mut Source) -> Self {
        Self(start)
    }

    pub fn token(&mut self) -> SeqToken {
        let ceil = self.ceil();
        if self.is_float() {
            self.float(ceil)
        } else {
            self.int(ceil)
        }
    }

    fn ceil(&mut self) -> String {
        self.0
            .take_while_next(|(_, c)| c.is_ascii_digit())
            .map(|c| c.1)
            .collect::<String>()
    }

    fn is_float(&mut self) -> bool {
        self.0
            .peekable()
            .next_if(|(_, c)| *c == '.')
            .is_some()
    }

    fn int(&mut self, ceil: String) -> SeqToken {
        SeqToken { len: ceil.len(), kind: TokenKind::Number(ceil) }
    }

    fn float(&mut self, ceil: String) -> SeqToken {
        let mantis = self.ceil();
        let number = format!("{}.{}", ceil, mantis);
        let len = number.len();
        let kind = if mantis.is_empty() { TokenKind::Error } else { TokenKind::Number(number) };
        SeqToken { len, kind }
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;

    use crate::shared::types::CharIter;

    use super::*;

    #[rstest]
    #[case("42", TokenKind::Number(String::from("42")))]
    #[case("123.124", TokenKind::Number(String::from("123.124")))]
    #[case("0", TokenKind::Number(String::from("0")))]
    #[case("5", TokenKind::Number(String::from("5")))]
    fn match_number(#[case] input: &'static str, #[case] expected: TokenKind) {
        let chars: CharIter = Box::new(input.chars());
        let mut source: Source = chars.enumerate().peekable();
        let token = Number::new(&mut source).token();
        assert_eq!(token.kind, expected);
    }
}
