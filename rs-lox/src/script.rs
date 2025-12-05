use std::{ error::Error, fs, ops::Deref };

use super::scanner::{ Scanner, token::Token };

pub struct Script(String);

impl Script {
    pub fn new(path: String) -> Self {
        Self(path)
    }

    pub fn tokens(self) -> Result<Vec<Token>, Box<dyn Error>> {
        let content = fs::read_to_string(self.0)?;
        let script: &'static String = Box::leak(Box::new(content));
        let scanner = Scanner::new(&script);
        let tokens = scanner.collect::<Vec<Token>>();
        Ok(tokens)
    }
}

impl Deref for Script {
    type Target = String;

    fn deref(&self) -> &Self::Target {
        &self.0
    }
}
