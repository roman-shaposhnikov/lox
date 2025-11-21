use super::token::TokenKind;

pub struct Keyword<'a>(&'a str);

impl<'a> Keyword<'a> {
    pub fn new(name: &'a str) -> Self {
        Self(name)
    }

    pub fn token_kind(&self) -> Option<TokenKind> {
        match self.0 {
            "and" => Some(TokenKind::And),
            "class" => Some(TokenKind::Class),
            "else" => Some(TokenKind::Else),
            "if" => Some(TokenKind::If),
            "nil" => Some(TokenKind::Nil),
            "or" => Some(TokenKind::Or),
            "print" => Some(TokenKind::Print),
            "return" => Some(TokenKind::Return),
            "super" => Some(TokenKind::Super),
            "var" => Some(TokenKind::Var),
            "while" => Some(TokenKind::While),
            "false" => Some(TokenKind::False),
            "for" => Some(TokenKind::For),
            "fun" => Some(TokenKind::Fun),
            "this" => Some(TokenKind::This),
            "true" => Some(TokenKind::True),
            _ => None,
        }
    }
}
