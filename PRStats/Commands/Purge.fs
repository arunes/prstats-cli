namespace Commands

open FSharp.SystemCommandLine

module Purge =
    let private run (dataOnly: bool) = 
        printfn "purging"

    let cmd =
        let dataOnly =
            Input.Option<bool>("--data-only", false, "Only deletes the data and not version controller settings.")

        command "purge" {
            description "Cleans up your version controller settings and pull requests data."
            inputs (dataOnly)
            setHandler run
        }
