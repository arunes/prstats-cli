open System.CommandLine.Invocation
open System.CommandLine.Help
open FSharp.SystemCommandLine
open System.CommandLine.Parsing

let checkPrequisites command =
    let hasSettings = Data.getSettings().IsSome
    let hasFetch = Data.getPullRequestCount() > 0

    Utils.checkPrequisites command hasSettings hasFetch

[<EntryPoint>]
let main argv =
    Dapper.FSharp.SQLite.OptionTypes.register ()

    let showHelp (ctx: InvocationContext) =
        let hc =
            HelpContext(ctx.HelpBuilder, ctx.Parser.Configuration.RootCommand, System.Console.Out)

        ctx.HelpBuilder.Write(hc)

    let ctx = Input.Context()

    let parser =
        rootCommandParser {
            description "PR Stats CLI - The CLI tool for pull request stats"
            inputs ctx
            setHandler showHelp
            addCommand Commands.Fetch.cmd
            addCommand Commands.Purge.cmd
            addCommand Commands.Reports.cmd
            addCommand Commands.Run.cmd
            addCommand Commands.Setup.cmd
        }

    let parsed = parser.Parse(argv)
    let commandName = parsed.CommandResult.Command.Name
    Utils.printCommandHeader commandName

    let prequisites = commandName |> checkPrequisites

    let resultCode =
        match prequisites with
        | None -> parsed.Invoke()
        | Some arr ->
            let neededCommands = arr |> Seq.map (fun c -> $"`{c}`") |> String.concat " and "

            Utils.printWarning
            <| $"You have to run {neededCommands} command(s) before running `{commandName}` command."
            1

    Utils.printCommandFooter commandName
    resultCode
