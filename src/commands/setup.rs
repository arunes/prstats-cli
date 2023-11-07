use crate::{
    models::{AppError, AzureDevOpsSettings, Settings, VersionController},
    settings,
};
use anyhow::{Error, Result};
use colored::Colorize;
use dialoguer::{Confirm, Input, Password};

pub fn run() -> Result<()> {
    println!("running setup");

    let mut settings = settings::get_settings();
    if settings.controller.is_some() {
        let confirmed = Confirm::new()
            .with_prompt("Setup already completed, running the setup again will overwrite existing settings. Do you want to continue?")
            .interact()
            .unwrap();

        if !confirmed {
            return Ok(());
        }
    }

    setup(&mut settings)
}

fn setup(settings: &mut Settings) -> Result<()> {
    // TODO: options for other version controllers
    let info = ask_azure_devops_info(&settings.controller);
    if let Some(azure_settings) = info {
        settings.controller = Some(VersionController::AzureDevOps(azure_settings));
        settings.save_settings()?;
        return Ok(());
    }

    Err(Error::new(AppError::SetupIncomplete))
}

fn ask_azure_devops_info(
    version_controller: &Option<VersionController>,
) -> Option<AzureDevOpsSettings> {
    let mut current: Option<AzureDevOpsSettings> = None;
    if let Some(controller) = version_controller {
        if let VersionController::AzureDevOps(azure_devops) = controller {
            current = Some(AzureDevOpsSettings {
                organization: azure_devops.organization.to_owned(),
                project: azure_devops.project.to_owned(),
                repository_id: azure_devops.repository_id.to_owned(),
                pat: azure_devops.pat.to_owned(),
            });
        }
    }

    let default = current.unwrap_or_default();
    let organization = get_input(
        "Please enter your organization name".to_owned(),
        &default.organization,
    );
    let project = get_input(
        "Please enter your project name".to_owned(),
        &default.project,
    );
    let repository_id = get_input(
        "Please enter your repository name".to_owned(),
        &default.repository_id,
    );
    let pat = Password::new()
        .with_prompt("Please enter your personal access token")
        .interact()
        .unwrap();

    println!("Validating settings...");

    let is_valid =
        validate_azure_devops(&organization, &project, &repository_id, &pat).unwrap_or(false);
    if is_valid {
        return Some(AzureDevOpsSettings {
            organization,
            project,
            repository_id,
            pat,
        });
    } else {
        if Confirm::new()
            .with_prompt("Do you want to re-enter the information?")
            .interact()
            .unwrap()
        {
            ask_azure_devops_info(version_controller);
        }
    }

    None
}

fn get_input(question: String, default: &String) -> String {
    Input::<String>::new()
        .with_prompt(question)
        .default(default.to_owned())
        .validate_with(|input: &String| -> Result<(), &str> {
            if input.is_empty() {
                Err("Please enter a value")
            } else {
                Ok(())
            }
        })
        .interact_text()
        .unwrap()
}

fn validate_azure_devops(
    organization: &String,
    project: &String,
    repository_id: &String,
    pat: &String,
) -> Result<bool> {
    let url = format!(
        "https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repository_id}"
    );

    let client = reqwest::blocking::Client::new();
    let response = client.get(&url).basic_auth("user", Some(pat)).send()?;
    let status = response.status().as_u16();

    match status {
        200 => {
            println!("{}", "Settings are successfully validated.".green());
            Ok(true)
        }
        _ => {
            let error_message = match status {
                203 => "Please make sure your personal access token is correct. (203 Non-Authoritative Information)",
                401 => "Please make sure your personal access token is correct. (401 Unauthorized)",
                403 => "Please make sure your personal access token has 'Read' access to 'Code' scope. (403 Forbidden)",
                _ => "Cannot validate you settinggs. (Azure DevOps Api responded with: '{status}' status code)."
            };

            println!("{}", error_message.red());

            Ok(false)
        }
    }
}
