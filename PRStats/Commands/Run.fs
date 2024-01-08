namespace Commands

open System
open System.IO
open System.Text.Json
open Models.Common
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

    let private filterByStatus status prs =
        match status with
        | Status.Active -> prs |> Seq.filter (fun pr -> pr.Status = PullRequestStatus.active)
        | Status.Completed -> prs |> Seq.filter (fun pr -> pr.Status = PullRequestStatus.completed)
        | Status.Abandoned -> prs |> Seq.filter (fun pr -> pr.Status = PullRequestStatus.abandoned)
        | _ -> prs
        |> Seq.toList

    let private filterByBranch branch prs =
        match branch with
        | Some b -> prs |> Seq.filter (fun pr -> pr.TargetBranch = b)
        | None -> prs
        |> Seq.toList

    let private filterByBefore date dateType prs =
        match date with
        | Some d ->
            match dateType with
            | DateType.Completed -> prs |> Seq.filter (fun pr -> pr.ClosedOn <= Some d)
            | _ -> prs |> Seq.filter (fun pr -> pr.CreatedOn <= d)
        | None -> prs
        |> Seq.toList

    let private filterByAfter date dateType prs =
        match date with
        | Some d ->
            match dateType with
            | DateType.Completed -> prs |> Seq.filter (fun pr -> pr.ClosedOn >= Some d)
            | _ -> prs |> Seq.filter (fun pr -> pr.CreatedOn >= d)
        | None -> prs
        |> Seq.toList

    let private run
        (
            status: Status,
            branch: Option<string>,
            before: Option<DateTime>,
            after: Option<DateTime>,
            dateType: DateType
        ) =

        let prs =
            (Utils.getFilePath "prs.json")
            |> File.ReadAllText
            |> JsonSerializer.Deserialize<List<PullRequest>>
            |> filterByStatus status
            |> filterByBranch branch
            |> filterByBefore before dateType
            |> filterByAfter after dateType

        prs
        |> Seq.groupBy (fun pr -> pr.CreatedBy)
        |> Seq.map (fun (key, values) ->
            {| Name = key
               Count = values |> Seq.length |})
        |> Seq.sortByDescending (fun r -> r.Count)
        |> Seq.iter (fun r -> printfn "%s\t\t\t%d" r.Name r.Count)

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
