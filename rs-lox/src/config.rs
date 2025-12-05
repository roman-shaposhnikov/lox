use std::env::Args;

pub struct Config {
    pub path: String,
}

impl Config {
    pub fn build(args: Args) -> Result<Self, &'static str> {
        let args: Vec<String> = args.skip(1).collect();
        match args.len() {
            0 => Err("No path provided"),
            1 => Ok(Config { path: args[0].clone() }),
            _ => Err("Too much arguments"),
        }
    }
}
