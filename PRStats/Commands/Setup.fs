namespace Commands

open Models.Data
open Sharprompt
open Flurl.Http
open FSharp.SystemCommandLine

module Setup =

    let rec private getAzureSettings () : Option<Settings> =
        let validators = [| Validators.Required() |]

        let organization =
            Prompt.Input<string>("Please enter your organization name", validators = validators)

        let project =
            Prompt.Input<string>("Please enter your project name", validators = validators)

        let repositoryId =
            Prompt.Input<string>("Please enter your repository name", validators = validators)

        let pat =
            Prompt.Password("Please enter your personal access token", validators = validators)

        printfn "Validating settings..."

        let isValid =
            task {
                try
                    let! response =
                        $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}"
                            .WithBasicAuth("", pat)
                            .GetAsync()

                    if response.StatusCode = 200 then
                        Utils.printOk <| $"Settings successfully validated!"
                        return true
                    else
                        let errorMessage =
                            match response.StatusCode with
                            | 203 ->
                                "Please make sure your personal access token is correct. (203 Non-Authoritative Information)"
                            | _ ->
                                "Cannot validate you settinggs. (Azure DevOps Api responded with: '{status}' status code)."

                        Utils.printError <| $"Validation failed! {errorMessage}"
                        return false
                with :? FlurlHttpException ->
                    return false

            }
            |> Async.AwaitTask
            |> Async.RunSynchronously

        if isValid then
            Some(
                { Type = VersionControllerType.AzureDevOps
                  Owner = organization
                  Project = Some project
                  Repo = repositoryId
                  Token = pat }
            )
        else
            Utils.printError
            <| "Validation failed, please check the information you entered."

            if Prompt.Confirm("Do you want to re-enter the information?") then
                getAzureSettings ()
            else
                None

    let private setup controllerType =
        let settings =
            match controllerType with
            | VersionControllerType.AzureDevOps -> getAzureSettings ()
            | _ -> failwith "Please select valid version controller"

        match settings with
        | Some s -> Data.saveSettings s
        | None -> ()

    let private run () =
        let settings = Data.getSettings ()

        match settings with
        | Some _ ->
            let confirmed =
                Prompt.Confirm
                    "Setup already completed, running the setup again will overwrite existing settings. Do you want to continue?"

            match confirmed with
            | true -> setup VersionControllerType.AzureDevOps
            | false -> ()
        | None -> setup VersionControllerType.AzureDevOps

    let cmd =
        let handler () =
            try
                run ()
            with ex ->
                Utils.printError <| ex.Message

        command "setup" {
            description "Runs the wizard to setup your version controller."
            setHandler handler
        }
