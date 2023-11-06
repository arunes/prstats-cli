use thiserror::Error;
use serde::{Deserialize, Serialize};

#[derive(Error, Debug)]
pub enum AppError {
    #[error("Unknown error")]
    Unknown,
}

#[derive(Serialize, Deserialize, Default, Debug)]
#[serde(default)]
pub struct Settings {
    pub azure_dev_ops: Option<AzureDevOps>,
}

#[derive(Serialize, Deserialize, Default, Debug)]
pub struct AzureDevOps {
    pub organization: String,
    pub project: String,
    pub repository_id: String,
    pub pat: String,
}
