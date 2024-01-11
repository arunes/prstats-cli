module Utils

open System.IO
open System
open System.Diagnostics

/// <summary>
/// Gets the application folder path
/// </summary>
let appFolder = AppContext.BaseDirectory

/// <summary>
/// Gets the data folder path
/// </summary>
let dataFolder =
    let dataRoot =
        (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "prstats-cli")
        |> Path.Combine

    if not <| Directory.Exists dataRoot then
        Directory.CreateDirectory dataRoot |> ignore

    dataRoot

/// <summary>
/// Prints the command header with the version and command name
/// </summary>
/// <param name="command">The command name</param>
let printCommandHeader command =
    let version =
        (appFolder, "prstats.exe") |> Path.Combine |> FileVersionInfo.GetVersionInfo

    printfn ""
    printfn "prstats-cli v%d.%d - running '%s' command." version.ProductMajorPart version.ProductMinorPart command
    printfn ""

/// <summary>
/// Prints the command footer with an empty line
/// </summary>
/// <param name="command">The command name</param>
let printCommandFooter command = printfn ""

/// <summary>
/// A private helper function to print a string with a specified color
/// </summary>
/// <param name="color">The color to use</param>
/// <param name="s">The string to print</param>
let private printColor =
    let lockObj = obj ()

    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printfn "%s" s
            Console.ResetColor())

/// <summary>
/// Prints an error message with red color
/// </summary>
/// <param name="s">The error message</param>
let printError = printColor ConsoleColor.Red

/// <summary>
/// Prints an ok message with green color
/// </summary>
/// <param name="s">The ok message</param>
let printOk = printColor ConsoleColor.Green

/// <summary>
/// Prints a warning message with yellow color
/// </summary>
/// <param name="s">The warning message</param>
let printWarning = printColor ConsoleColor.Yellow
