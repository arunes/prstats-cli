use crate::args::{GroupBy, OutputType};
use anyhow::Result;

pub fn run(output_type: OutputType, group_by: GroupBy) -> Result<()> {
    super::run_prerequisites(true, true)?;
    println!("running commits");

    Ok(())
}
