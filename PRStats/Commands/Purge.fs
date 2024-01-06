namespace Commands

module Purge =
    let run settings options =
        Utils.printOk <| "running purge"
        printfn "settings: %A" settings
        printfn "options: %A" options
