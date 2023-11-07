mod args;
mod commands;
mod models;
mod settings;
mod utils;
use anyhow::Result;
use args::{App, Command};
use clap::Parser;
use colored::Colorize;

fn main() {
    let app = App::parse();

    if let Err(error) = run_command(app.command) {
        println!("{}", format!("Error happened!, {:?}", error).red());
    }
}

fn run_command(command: Command) -> Result<()> {
    match command {
        Command::PullRequests {
            output_type,
            group_by,
            status,
            branch,
            before,
            after,
            date_type,
        } => commands::pull_requests::run(
            output_type,
            group_by,
            status,
            branch,
            before,
            after,
            date_type,
        ),
        Command::Commits {
            output_type,
            group_by,
        } => commands::commits::run(output_type, group_by),
        Command::Setup => commands::setup::run(),
        Command::Fetch => commands::fetch::run(),
        Command::Purge { data_only } => commands::purge::run(data_only),
    }
}
