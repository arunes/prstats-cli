use anyhow::Result;

pub fn run() -> Result<()> {
    super::run_prerequisites(true, false)?;
    println!("running fetch");

    Ok(())
}

pub fn is_fetch_done() -> bool {
    false
}
