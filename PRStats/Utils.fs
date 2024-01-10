module Utils

open System.IO
open System
open System.Reflection
open System.Diagnostics

let getFilePath fileName =
    let appDataRoot =
        (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "prstats-cli")
        |> Path.Combine

    if not <| Directory.Exists appDataRoot then
        Directory.CreateDirectory appDataRoot |> ignore

    Path.Combine(appDataRoot, fileName)

let printCommandHeader command =
    let version =
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)

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
