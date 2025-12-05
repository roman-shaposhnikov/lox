use crate::shared::types::CharIter;

use super::token::TokenKind;
use super::operator::Operator;
use super::identifier::Identifier;
use super::types::Source;
use super::number::Number;
use super::string::LoxString;

pub struct Sequence {
    source: Source,
}

impl Sequence {
    pub fn new(source: CharIter) -> Self {
        Self {
            source: source.enumerate().peekable(),
        }
    }

    fn token(&mut self, current: char) -> SeqToken {
        let source = &mut self.source;
        if current.is_ascii_alphabetic() {
            Identifier::new(source).token()
        } else if current.is_ascii_digit() {
            Number::new(source).token()
        } else if current == '"' {
            LoxString::new(source).token()
        } else {
            Operator::new(source).token()
        }
    }

    fn skip_whitespace(&mut self) -> Option<()> {
        while self.source.peek()?.1.is_whitespace() {
            self.source.next();
        }
        Some(())
    }
}

impl Iterator for Sequence {
    type Item = LineToken;

    fn next(&mut self) -> Option<Self::Item> {
        self.skip_whitespace()?;
        let (start, character) = self.source.by_ref().peek().copied()?;
        let SeqToken { kind, len } = self.token(character);
        Some(LineToken { kind, col: start + 1, len })
    }
}

#[derive(Debug)]
pub struct LineToken {
    pub kind: TokenKind,
    // TODO: convert to u32 or u64
    pub col: usize,
    pub len: usize,
}

#[derive(Debug)]
pub struct SeqToken {
    pub kind: TokenKind,
    pub len: usize,
}

#[cfg(test)]
mod tests {
    use itertools::Itertools;
    use rstest::rstest;
    use super::*;

    #[test]
    fn match_sequence() {
        let expected = vec![
            TokenKind::Number(String::from("123")),
            TokenKind::Identifier(String::from("name")),
            TokenKind::String(String::from("string")),
            TokenKind::BangEqual
        ]
            .into_iter()
            .permutations(4);
        let perms = ["123", "name", "\"string\"", "!="]
            .into_iter()
            .permutations(4)
            .map(|vec| vec.join(" ").chars().collect::<Vec<_>>());

        perms.zip(expected).for_each(|(chars, expected)| {
            let result = Sequence::new(Box::new(chars.clone().into_iter()))
                .map(|t| t.kind)
                .collect::<Vec<TokenKind>>();
            let input = chars.into_iter().join("");
            assert_eq!(
                result,
                expected,
                "input \"{input}\" with a sequence {result:?} be equal to {expected:?}"
            );
        });
    }

    #[rstest]
    #[case("         ,", TokenKind::Comma)]
    #[case(")         ", TokenKind::RightParen)]
    #[case("    /     ", TokenKind::Slash)]
    #[case("    
    +", TokenKind::Plus)] // newline
    #[case("\n+", TokenKind::Plus)] // newline
    #[case("	{", TokenKind::LeftBrace)] // tabulation
    #[case("\t{", TokenKind::LeftBrace)] // tabulation
    #[case("\r abc", TokenKind::Identifier(String::from("abc")))] // caret return
    fn skip_whitespace(#[case] input: &'static str, #[case] expected: TokenKind) {
        let source: CharIter = Box::new(input.chars());
        let mut scanner = Sequence::new(source);
        let token = scanner.next().unwrap();
        assert_eq!(
            token.kind,
            expected,
            "input \"{input}\" with a sequence {token:?} be equal to {expected:?}"
        );
    }
}
