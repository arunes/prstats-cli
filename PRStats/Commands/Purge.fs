namespace Commands

open FSharp.SystemCommandLine

module Purge =
    let private run (dataOnly: bool) = Utils.printError <| "Not implemented"

    let cmd =
        let dataOnly =
            Input.Option<bool>("--data-only", false, "Only deletes the data and not version controller settings.")

        let handler (dataOnly: bool) =
            try
                run (dataOnly)
            with ex ->
                Utils.printError <| ex.Message

        command "purge" {
            description "Cleans up your version controller settings and pull requests data."
            inputs (dataOnly)
            setHandler handler
        }
