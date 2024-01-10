module Utils

open System.IO
open System
open System.Diagnostics

let appFolder = AppContext.BaseDirectory

let dataFolder =
    let dataRoot =
        (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "prstats-cli")
        |> Path.Combine

    if not <| Directory.Exists dataRoot then
        Directory.CreateDirectory dataRoot |> ignore

    dataRoot

let printCommandHeader command =
    let version =
        (appFolder, "prstats.exe") |> Path.Combine |> FileVersionInfo.GetVersionInfo

    printfn ""
    printfn "prstats-cli v%d.%d - running '%s' command." version.ProductMajorPart version.ProductMinorPart command
    printfn ""

let printCommandFooter command = printfn ""

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
