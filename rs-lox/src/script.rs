use std::{ fs, ops::Deref };

pub struct Script(String);

impl Script {
    pub fn build(path: String) -> Self {
        // TODO: handle file read errors
        let content = fs::read_to_string(path).unwrap();
        Self(content)
    }

    pub fn leak(self) -> &'static String {
        Box::leak(Box::new(self))
    }
}

impl Deref for Script {
    type Target = String;

    fn deref(&self) -> &Self::Target {
        &self.0
    }
}
