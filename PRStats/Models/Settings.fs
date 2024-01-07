namespace Models

module Settings =
    type ControllerSettings =
        | AzureDevOps of AzureDevOps.Settings
        | Github of Github.Settings
