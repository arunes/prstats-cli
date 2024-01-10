namespace Commands

open Models.Data
open Sharprompt
open Flurl.Http
open FSharp.SystemCommandLine

module Setup =

    let rec private getGithubSettings () : Option<Settings> = None

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
        else if Prompt.Confirm("Do you want to re-enter the information?") then
            getAzureSettings ()
        else
            None

    let private setup controllerType =
        let settings =
            match controllerType with
            | VersionControllerType.AzureDevOps -> getAzureSettings ()
            | VersionControllerType.Github -> getGithubSettings ()
            | _ -> failwith "Please select valid version controller"

        match settings with
        | Some s -> Data.saveSettings s
        | None -> ()

    let private run () =
        Utils.printCommandHeader "setup"

        let getControllerType () =
            Prompt.Select<VersionControllerType>("Select your source controller")

        let settings = Data.getSettings ()

        match settings with
        | Some _ ->
            let confirmed =
                Prompt.Confirm
                    "Setup already completed, running the setup again will overwrite existing settings. Do you want to continue?"

            match confirmed with
            | true -> setup (getControllerType ())
            | false -> ()
        | None -> setup (getControllerType ())

        Utils.printCommandFooter "setup"

    let cmd =
        command "setup" {
            description "Runs the wizard to setup your version controller."
            setHandler run
        }
