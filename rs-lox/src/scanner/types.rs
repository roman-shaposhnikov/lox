use std::{ iter::Peekable };

use crate::shared::types::CharIter;

pub type Source = Peekable<CharIter>;
