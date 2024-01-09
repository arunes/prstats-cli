﻿module Data

open System
open System.Text
open System.IO
open System.Data.SQLite
open Dapper
open Dapper.FSharp.SQLite
open Models.Data
open System.Data

let private settingsTable = table<Settings>
let private pullRequestTable = table<PullRequest>
let private pullRequestReviewerTable = table'<Reviewer> "PullRequestReviewer"

let private createTables (conn: SQLiteConnection) =
    let sql =
        "CREATE TABLE IF NOT EXISTS PullRequest (
             Id TEXT PRIMARY KEY,
             Title TEXT NOT NULL,
             Status INTEGER NOT NULL,
             CreatedBy TEXT NOT NULL,
             CreatedOn TEXT NOT NULL,
             ClosedOn TEXT NULL,
             SourceBranch TEXT NOT NULL,
             TargetBranch TEXT NOT NULL,
             IsDraft INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS PullRequestReviewer (
             Id INTEGER PRIMARY KEY,
             PullRequestId TEXT NOT NULL,
             Name TEXT NOT NULL,
             Vote INTEGER NOT NULL,
             FOREIGN KEY(PullRequestId) REFERENCES PullRequest(Id)
        );

        CREATE TABLE IF NOT EXISTS Settings (
             Type INTEGER PRIMARY KEY,
             Owner TEXT NOT NULL,
             Project TEXT NULL,
             Repo TEXT NOT NULL,
             Token TEXT NOT NULL
        );"

    conn.Execute sql |> ignore

let private getConnection () =
    let dbFile = Utils.getFilePath "data.sqlite"

    let dbExists =
        match dbFile |> File.Exists with
        | true -> true
        | false ->
            SQLiteConnection.CreateFile dbFile
            false

    let dbConnectionStr = $"Data Source={dbFile};Version=3;"
    let conn = new SQLiteConnection(dbConnectionStr)
    conn.Open()

    if not <| dbExists then
        createTables conn

    conn

let getSettings () : Option<Settings> =
    use db = getConnection ()

    let result =
        select {
            for p in settingsTable do
                skipTake 0 1
        }
        |> db.SelectAsync<Settings>
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> Seq.toList

    match result with
    | [ s ] -> Some s
    | _ -> None

let saveSettings (settings: Settings) =
    use db = getConnection ()

    delete {
        for s in settingsTable do
            deleteAll
    }
    |> db.DeleteAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    insert {
        into settingsTable
        value settings
    }
    |> db.InsertAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    ()

let createPullRequests prs =
    use db = getConnection ()

    delete {
        for p in pullRequestTable do
            deleteAll
    }
    |> db.DeleteAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    insert {
        into pullRequestTable
        values prs
    }
    |> db.InsertAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

let createPullRequestReviewers reviewers =
    use db = getConnection ()

    delete {
        for p in pullRequestReviewerTable do
            deleteAll
    }
    |> db.DeleteAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    insert {
        into pullRequestReviewerTable
        values reviewers
    }
    |> db.InsertAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

let getPullRequests
    (
        status: Option<PullRequestStatus>,
        branch: Option<string>,
        before: Option<DateTime>,
        after: Option<DateTime>,
        dateTypeColumn: string,
        sql: string
    ) =
    let addSelect (sb: StringBuilder) =
        sb.AppendLine "SELECT * FROM PullRequest WHERE 1=1 "

    let addBranch (sb: StringBuilder) =
        match branch with
        | Some b -> sb.AppendLine $"AND TargetBranch = '{b}' "
        | None -> sb

    let addStatus (sb: StringBuilder) =
        match status with
        | Some s -> sb.AppendLine $"AND Status = {(int) s} "
        | None -> sb

    let addBefore (sb: StringBuilder) =
        match before with
        | Some d -> sb.AppendLine $"AND {dateTypeColumn} <= date('{d:``yyyy-MM-dd``}') "
        | None -> sb

    let addAfter (sb: StringBuilder) =
        match after with
        | Some d -> sb.AppendLine $"AND {dateTypeColumn} >= date('{d:``yyyy-MM-dd``}') "
        | None -> sb

    let subSql =
        new StringBuilder()
        |> addSelect
        |> addBranch
        |> addStatus
        |> addBefore
        |> addAfter

    let fullSql = sql.Replace("[DataSource]", $"({subSql.ToString()})")

    let convert (reader: IDataReader) =
        let getValue (rdr: IDataReader, column: obj, index: int) =
            let ct = column.GetType()

            let value: obj =
                if ct = typeof<String> then rdr.GetString index
                elif ct = typeof<Int16> then rdr.GetInt16 index
                elif ct = typeof<Int32> then rdr.GetInt32 index
                elif ct = typeof<Int64> then rdr.GetInt64 index
                elif ct = typeof<Double> then rdr.GetDouble index
                else ct

            value.ToString()

        let mutable headerSet = false

        [ while reader.Read() do
              let colSeq = { 0 .. reader.FieldCount - 1 }

              if not <| headerSet then
                  yield colSeq |> Seq.map (fun idx -> reader.GetName idx) |> Seq.toList
                  headerSet <- true

              let response =
                  colSeq
                  |> Seq.map (fun idx -> getValue (reader, reader.Item idx, idx))
                  |> Seq.toList

              yield response ]


    async {
        use db = getConnection ()
        use! reader = db.ExecuteReaderAsync fullSql |> Async.AwaitTask

        return convert reader
    }
