mod config;

use std::env;
use std::process;

use rs_lox::script::Script;

use config::Config;

fn main() {
    let args = env::args();
    let config = Config::build(args).unwrap_or_else(|err| {
        eprintln!("{err}");
        process::exit(64);
    });
    let tokens = Script::new(config.path)
        .tokens()
        .unwrap_or_else(|err| {
            eprintln!("{err}");
            process::exit(65);
        });
    println!("{:?}", tokens);
}
