use crate::{models::Settings, utils};
use anyhow::Result;
use std::{
    fs::{self, File},
    io::Write,
};

impl Settings {
    pub fn save_settings(&self) -> Result<()> {
        let path = utils::get_file_path(Some("settings.toml"));
        fs::create_dir_all(path.clone().parent().unwrap())?;

        let toml_content = toml::to_string(self)?;
        let mut file = File::create(path)?;
        file.write_all(toml_content.as_bytes())?;

        Ok(())
    }
}

pub fn get_settings() -> Settings {
    let path = utils::get_file_path(Some("settings.toml"));

    match fs::read_to_string(path) {
        Ok(content) => toml::from_str(&content).unwrap(),
        Err(_) => Settings::default(),
    }
}
