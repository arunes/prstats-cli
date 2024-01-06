module Utils

open System.IO
open System

let getFilePath fileName =
    let appDataRoot =
        (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "prstats-cli")
        |> Path.Combine

    if not <| Directory.Exists appDataRoot then
        Directory.CreateDirectory appDataRoot |> ignore

    Path.Combine(appDataRoot, fileName)

let private printColor =
    let lockObj = obj ()

    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printfn "%s" s
            Console.ResetColor())

let printError = printColor ConsoleColor.Red
let printOk = printColor ConsoleColor.Green
let printWarning = printColor ConsoleColor.Yellow
