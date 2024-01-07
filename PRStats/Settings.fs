module Settings

open System.Text.Json.Nodes
open Models.Settings
open Models.Common
open System.IO
open System.Text.Json

let getSettings () : Option<ControllerSettings> =
    let settingsFile = Utils.getFilePath "settings.json"

    if not <| (settingsFile |> File.Exists) then
        None
    else
        let settings = settingsFile |> File.ReadAllText |> JsonValue.Parse

        let controllerType = settings["Type"].Deserialize<VersionControllerType>()

        match controllerType with
        | VersionControllerType.AzureDevOps ->
            Some(AzureDevOps(JsonSerializer.Deserialize<Models.AzureDevOps.Settings> settings))
        | VersionControllerType.Github -> Some(Github(JsonSerializer.Deserialize<Models.Github.Settings> settings))
        | _ -> None

let saveSettings settings =
    let json =
        match settings with
        | AzureDevOps az -> JsonSerializer.Serialize az
        | Github gh -> JsonSerializer.Serialize gh

    (Utils.getFilePath "settings.json", json) |> File.WriteAllText
