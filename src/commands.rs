use anyhow::Result;

use crate::settings;

pub mod commits;
pub mod fetch;
pub mod pull_requests;
pub mod purge;
pub mod setup;

fn run_prerequisites(run_setup: bool, run_fetch: bool) -> Result<()> {
    let settings = settings::get_settings();
    if run_setup && settings.azure_dev_ops.is_none() {
        setup::run()?;
    }

    if run_fetch && !fetch::is_fetch_done() {
        fetch::run()?;
    }

    Ok(())
}
