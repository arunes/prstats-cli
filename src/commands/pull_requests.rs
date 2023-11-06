use crate::args::{DateType, GroupBy, OutputType, Status};
use anyhow::Result;

pub fn run(
    output_type: OutputType,
    group_by: GroupBy,
    status: Status,
    branch: String,
    before: String,
    after: String,
    date_type: DateType,
) -> Result<()> {
    super::run_prerequisites(true, true)?;
    println!("running prs");
    
    Ok(())
}
