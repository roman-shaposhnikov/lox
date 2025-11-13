pub mod types {
    pub type AnyIter<T> = Box<dyn Iterator<Item = T>>;
}
