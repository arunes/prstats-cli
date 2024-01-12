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
/// Checks the prerequisites for a given command
/// </summary>
/// <param name="command">The command to check</param>
/// <param name="hasSettings">A flag indicating whether the settings are available</param>
/// <param name="hasFetch">A flag indicating whether the fetch operation is done</param>
/// <returns>An option value containing an array of required commands that are missing, or None if all prerequisites are met</returns>
let checkPrequisites command hasSettings hasFetch =
    let check requiredCommand current =
        if requiredCommand = "setup" && not <| hasSettings then
            current |> Array.append [| "setup" |]
        elif requiredCommand = "fetch" && not <| hasFetch then
            current |> Array.append [| "fetch" |]
        else
            current

    match command with
    | "fetch" -> Array.empty |> check "setup"
    | "purge" -> Array.empty |> check "setup" |> check "fetch"
    | "run" -> Array.empty |> check "setup" |> check "fetch"
    | _ -> Array.empty
    |> Array.rev
    |> fun arr -> if arr.Length = 0 then None else Some arr

/// <summary>
/// Prints the command header with the version and command name
/// </summary>
/// <param name="command">The command name</param>
let printCommandHeader command =
    let version =
        (appFolder, "prstats.dll") |> Path.Combine |> FileVersionInfo.GetVersionInfo

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
