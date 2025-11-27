use std::env;
use std::env::Args;
use std::process;
use rs_lox::scanner::Scanner;
use rs_lox::scanner::token::{ Token };
use rs_lox::script::Script;

fn main() {
    let args = env::args();
    let config = Config::build(args).unwrap_or_else(|err| {
        eprintln!("{err}");
        process::exit(64);
    });
    let script = Script::build(config.path).leak();
    let scanner = Scanner::new(&script);
    let tokens: Vec<Token> = scanner.collect();
    println!("{:?}", tokens);
}

struct Config {
    path: String,
}

impl Config {
    fn build(args: Args) -> Result<Self, &'static str> {
        let args: Vec<String> = args.skip(1).collect();
        match args.len() {
            0 => Err("No path provided"),
            1 => Ok(Config { path: args[0].clone() }),
            _ => Err("Too much arguments"),
        }
    }
}
