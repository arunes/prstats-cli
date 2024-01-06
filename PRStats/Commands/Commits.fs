namespace Commands

module Commits =
    let run settings options =
        Utils.printOk <| "running commits"
        printfn "settings: %A" settings
        printfn "options: %A" options
