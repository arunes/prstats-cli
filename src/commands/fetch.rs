use std::{fs::{File, self}, io::Write};

use anyhow::{Result, Error};

use crate::{models::{VersionController, AzureDevOpsSettings}, settings, utils};

pub fn run() -> Result<()> {
    super::run_prerequisites(true, false)?;
    println!("running fetch");
    let settings = settings::get_settings();

    match settings.controller.unwrap() {
        VersionController::AzureDevOps(settings) => {
            let json = get_azure_devops_prs(settings)?;
            let path = utils::get_file_path(Some("prs.json"));
            fs::create_dir_all(path.clone().parent().unwrap())?;
   
            let mut file = File::create(path)?;
            file.write_all(json.as_bytes())?;
        }
    }

    Ok(())
}

pub fn is_fetch_done() -> bool {
    false
}

fn get_azure_devops_prs(settings: AzureDevOpsSettings) -> Result<String> {
    let url = format!("https://dev.azure.com/{}/{}/_apis/git/repositories/{}/pullrequests?api-version=7.1-preview.1&$top=500&searchCriteria.status=all",
        settings.organization,
        settings.project,
        settings.repository_id
    );

    let response = reqwest::blocking::Client::new()
        .get(url)
        .basic_auth("user", Some(settings.pat))
        .send()?;

    match response.text() {
        Ok(text) => Ok(text),
        Err(error) => Err(Error::new(error))
    }
}
