module Settings

open System.Text.Json.Nodes
open System.Text.Json
open System.IO

let getSettings () : Option<Models.Settings> =
    let settingsFile = Utils.getFilePath "settings.json"

    if not <| (settingsFile |> File.Exists) then
        None
    else
        let settings = settingsFile |> File.ReadAllText |> JsonValue.Parse

        let controllerType = settings["Type"].Deserialize<Models.VersionControllerType>()

        match controllerType with
        | Models.VersionControllerType.AzureDevOps ->
            Some(Models.Azure(JsonSerializer.Deserialize<Models.AzureDevOpsSettings> settings))
        | Models.VersionControllerType.Github ->
            Some(Models.Github(JsonSerializer.Deserialize<Models.GithubSettings> settings))
        | _ -> None

let saveSettings settings =
    let json =
        match settings with
        | Models.Azure az -> JsonSerializer.Serialize az
        | Models.Github gh -> JsonSerializer.Serialize gh

    (Utils.getFilePath "settings.json", json) |> File.WriteAllText
