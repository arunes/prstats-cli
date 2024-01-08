namespace Commands

open FSharp.SystemCommandLine

module Purge =
    let cmd =
        let handler (dataOnly: bool) = printfn $"Purge called."

        let dataOnly =
            Input.Option<bool>("--data-only", false, "Only deletes the data and not version controller settings.")

        command "purge" {
            description "Cleans up your version controller settings and pull requests data."
            inputs (dataOnly)
            setHandler handler
        }
