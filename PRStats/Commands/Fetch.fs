namespace Commands

module Fetch =

    let run settings =
        Utils.printOk <| "running fetch"
        printfn "settings: %A" settings

    let isFetchDone = false
