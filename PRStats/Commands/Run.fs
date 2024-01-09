namespace Commands

open System
open System.IO
open Sharprompt
open FSharp.SystemCommandLine

module Run =

    type Status =
        | All = 1
        | Active = 2
        | Completed = 3
        | Abandoned = 4

    type DateType =
        | Completed = 1
        | Created = 2

    let private printPretty data =
        let columnsLengths =
            { 0 .. (data |> Seq.head |> Seq.length) - 1 }
            |> Seq.map (fun idx ->
                data
                |> Seq.map (fun r -> r |> Seq.item idx)
                |> Seq.map (fun c -> c |> String.length |> (fun l -> l + 5))
                |> Seq.max)

        let totalLength = columnsLengths |> Seq.map (fun c -> c) |> Seq.sum
        let separator = "".PadLeft(totalLength, '-')

        data
        |> Seq.indexed
        |> Seq.iter (fun (rix, row) ->
            row
            |> Seq.indexed
            |> Seq.iter (fun (cix, col) ->
                let length = columnsLengths |> Seq.item cix
                let value = col.PadRight length
                printf "%s" value)


            match rix with
            | 0 -> printfn "\n%s" separator
            | _ -> printfn "")

    let private run
        (
            status: Status,
            branch: Option<string>,
            before: Option<DateTime>,
            after: Option<DateTime>,
            dateType: DateType
        ) =

        let reportList =
            Directory.GetFiles "Reports/"
            |> Seq.filter (fun f -> f.EndsWith ".sql")
            |> Seq.map Path.GetFileNameWithoutExtension

        let report = Prompt.Select("Select report to run", reportList)
        let sql = $"Reports/{report}.sql" |> File.ReadAllText

        let dbStatus =
            match status with
            | Status.Active -> Some Models.Data.PullRequestStatus.active
            | Status.Completed -> Some Models.Data.PullRequestStatus.completed
            | Status.Abandoned -> Some Models.Data.PullRequestStatus.abandoned
            | _ -> None

        let dateTypeColumn =
            match dateType with
            | DateType.Completed -> "ClosedOn"
            | _ -> "CreatedOn"

        printfn "Running '%s' report." report
        Data.getPullRequests (dbStatus, branch, before, after, dateTypeColumn, sql)
        |> Async.RunSynchronously
        |> Seq.toList
        |> printPretty

    let cmd =
        let status = Input.Option<Status>("--status", Status.Completed, "Filter by status.")
        let branch = Input.OptionMaybe<string>("--branch", "Filter by status.")

        let before =
            Input.OptionMaybe<DateTime>(
                "--before",
                "Filter by date (shows pull requests before and on. Date format must be in YYYY-MM-DD."
            )

        let after =
            Input.OptionMaybe<DateTime>(
                "--after",
                "Filter by date (shows pull requests after and on. Date format must be in YYYY-MM-DD."
            )

        let dateType =
            Input.Option<DateType>("--date-type", DateType.Completed, "Specify a date type.")

        command "run" {
            description "Gets pull request stats."
            inputs (status, branch, before, after, dateType)
            setHandler run
        }
