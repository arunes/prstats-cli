namespace Commands

open FSharp.SystemCommandLine
open System.IO
open Models.Common

module Reports =
    /// <summary>
    /// Gets a list of reports from the Reports folder
    /// </summary>
    /// <returns>A list of report records with Id and Name fields</returns>
    let reportList = 
        Directory.GetFiles "Reports/"
        |> Seq.filter (fun f -> f.EndsWith ".sql")
        |> Seq.map Path.GetFileNameWithoutExtension
        |> Seq.sort
        |> Seq.indexed
        |> Seq.map (fun (idx, f) -> { Id = idx + 1; Name = f })
        |> Seq.toList      

    /// <summary>
    /// Gets a report by report id
    /// </summary>
    /// <returns>A report record with Id and Name fields</returns>
    let getReportById id =
        let report = reportList |> Seq.tryFind (fun r -> r.Id = id)

        match report with
        | Some r -> r
        | None -> failwith $"Cannot find the report with id '{id}'"

    /// <summary>
    /// Gets a report's sql by report id
    /// </summary>
    /// <returns>A report record with Id and Name fields</returns>
    let getReportSql id =
        let report = reportList |> Seq.tryFind (fun r -> r.Id = id)

        match report with
        | Some r -> $"Reports/{r.Name}.sql" |> File.ReadAllText
        | None -> failwith $"Cannot find the report with id '{id}'"

    let private run () =
        Utils.printCommandHeader "reports"

        let reports = reportList

        let seperator =
            reports
            |> Seq.map (fun r -> r.Name.Length)
            |> Seq.max
            |> fun l -> "".PadLeft(l + 5, '-')

        printfn "Id   Name"
        printfn "%s" seperator
        reports |> Seq.iter (fun r -> printfn "%-5d%s" r.Id r.Name)

        Utils.printCommandFooter "reports"

    let cmd =
        command "reports" {
            description "Lists all the available reports."
            setHandler run
        }
