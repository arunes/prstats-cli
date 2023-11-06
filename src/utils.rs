use std::{env, path::PathBuf};

pub fn get_file_path(file: Option<&str>) -> PathBuf {
    let root = dirs::config_dir();
    let mut path = match root {
        Some(mut value) => {
            value.push("prstats");
            value
        }
        None => {
            let mut temp = env::temp_dir();
            temp.push("prstats");
            temp
        }
    };

    if let Some(file_path) = file {
        path.push(file_path);
    }

    path
}
