namespace Commands

module PullRequests =

    let run settings options = 
        Utils.printOk <| "running prs"
        printfn "settings: %A" settings
        printfn "options: %A" options
