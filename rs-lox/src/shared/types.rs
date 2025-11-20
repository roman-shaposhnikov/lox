pub type AnyIter<T> = Box<dyn Iterator<Item = T>>;
pub type CharIter = AnyIter<char>;
