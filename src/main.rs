mod args;
use args::App;
use clap::Parser;

fn main() {
    let app = App::parse();

    println!("{app:?}");
}
