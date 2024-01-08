﻿open System.CommandLine.Invocation
open System.CommandLine.Help
open FSharp.SystemCommandLine


[<EntryPoint>]
let main argv =
    let showHelp (ctx: InvocationContext) =
        let hc =
            HelpContext(ctx.HelpBuilder, ctx.Parser.Configuration.RootCommand, System.Console.Out)

        ctx.HelpBuilder.Write(hc)

    let ctx = Input.Context()

    rootCommand argv {
        description "PR Stats CLI - The CLI tool for pull request stats"
        inputs ctx
        setHandler showHelp
        addCommand Commands.Run.cmd
        addCommand Commands.Setup.cmd
        addCommand Commands.Fetch.cmd
        addCommand Commands.Purge.cmd
    }