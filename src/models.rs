use thiserror::Error;
use serde::{Deserialize, Serialize};

#[derive(Error, Debug)]
pub enum AppError {
    #[error("Setup incomplete")]
    SetupIncomplete,

    #[error("Unknown error")]
    Unknown,
}

#[derive(Serialize, Deserialize, Default, Debug)]
#[serde(default)]
pub struct Settings {
    pub controller: Option<VersionController>,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum VersionController {
    AzureDevOps(AzureDevOpsSettings),
}

#[derive(Serialize, Deserialize, Default, Debug)]
pub struct AzureDevOpsSettings {
    pub organization: String,
    pub project: String,
    pub repository_id: String,
    pub pat: String,
}
