use clap::{Parser, Subcommand};

#[derive(Debug, Parser)]
#[clap(name = "prstats-cli", version)]
pub struct App {
    #[clap(subcommand)]
    pub command: Command,
}

#[derive(Debug, clap::ValueEnum, Clone)]
pub enum OutputType {
    Time,
    Number,
}

#[derive(Debug, clap::ValueEnum, Clone)]
pub enum GroupBy {
    User,
    Branch,
}

#[derive(Debug, clap::ValueEnum, Clone)]
pub enum Status {
    All,
    Open,
    Completed,
    Abandoned,
}

#[derive(Debug, clap::ValueEnum, Clone)]
pub enum DateType {
    Closed,
    Created,
}

#[derive(Debug, Subcommand)]
pub enum Command {
    /// Gets pull request stats
    #[command(name = "prs")]
    PullRequests {
        /// Output type
        #[arg(long = "type", default_value = "number")]
        #[clap(value_enum)]
        output_type: OutputType,

        /// Group records
        #[arg(long, default_value = "user")]
        #[clap(value_enum)]
        group_by: GroupBy,

        /// Filter by status
        #[arg(long, default_value = "completed")]
        #[clap(value_enum)]
        status: Status,

        /// Filter by target branch
        #[arg(long, default_value = "")]
        branch: String,

        /// Filter by date (shows pull requests before and on)
        #[arg(long, default_value = "")]
        before: String,

        /// Filter by date (shows pull requests after and on)
        #[arg(long, default_value = "")]
        after: String,

        /// Specify a date type
        #[arg(long, default_value = "created")]
        #[clap(value_enum)]
        date_type: DateType,
    },

    /// Gets the pull request commit stats
    Commits {
        /// Output type
        #[arg(long = "type", default_value = "number")]
        #[clap(value_enum)]
        output_type: OutputType,

        /// Group records
        #[arg(long, default_value = "user")]
        #[clap(value_enum)]
        group_by: GroupBy,
    },

    /// Runs the wizard to setup your version controller
    Setup,

    /// Gets the latest data from your version controller
    Fetch,

    /// Cleans up your version controller settings and pull requests data
    Purge {
        /// Only deletes the data and not version controller settings
        #[arg(long, default_value_t = false)]
        data_only: bool,
    },
}
