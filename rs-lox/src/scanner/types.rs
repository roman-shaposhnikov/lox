use std::iter::{ Enumerate, Peekable };

use crate::shared::types::CharIter;

pub type Source = Peekable<Enumerate<CharIter>>;
