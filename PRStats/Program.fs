open System
open CommandLine.Text
open CommandLine

let fail errors = printfn "%A" errors

let displayHelp result =
    let helpText =
        HelpText.AutoBuild(
            result,
            fun (h: HelpText) ->
                h.AddEnumValuesToHelpText <- true
                h.AddOptions(result) |> ignore
                h
        )

    Console.WriteLine helpText


[<EntryPoint>]
let main args =
    let parser =
        new Parser(fun conf ->
            conf.CaseInsensitiveEnumValues <- true
            conf.HelpWriter <- null)

    let result =
        parser.ParseArguments<Models.PullRequestOptions, Models.CommitOptions, Models.SetupOptions, Models.FetchOptions, Models.PurgeOptions>
            args

    let rec getSettings() =
        let settings = Settings.getSettings()
        match settings with
        | Some s -> s
        | None ->
            Utils.printWarning <| "Before running any command you need to setup your version controller."
            Commands.Setup.run false
            getSettings ()

    match result with
    | :? CommandLine.Parsed<obj> as command ->
        let settings = getSettings()
        match command.Value with
        | :? Models.CommitOptions as opts -> Commands.Commits.run settings opts
        | :? Models.FetchOptions -> Commands.Fetch.run settings
        | :? Models.PullRequestOptions as opts -> Commands.PullRequests.run settings opts
        | :? Models.PurgeOptions as opts -> Commands.Purge.run settings opts
        | :? Models.SetupOptions -> Commands.Setup.run true
    | :? CommandLine.NotParsed<obj> as opts -> displayHelp opts

    1
