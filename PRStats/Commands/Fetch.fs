namespace Commands

open Models.Settings
open Models.Common
open System.Text.Json
open System.IO
open Extensions
open FSharp.SystemCommandLine

module AZ = Models.AzureDevOps

module Fetch =
    open Flurl.Http

    let private downloadAzureDevOpsPRs (settings: AZ.Settings) =
        let prs =
            settings.apiUrl
                .AppendPathSegment("pullrequests")
                .AppendQueryParam("$top", "500")
                .AppendQueryParam("searchCriteria.status", "all")
                .GetJson<AZ.ApiResponse<AZ.PullRequest>>()

        // if pr.count > limit pull next

        let convertReviewers (reviewers: Option<List<AZ.Reviewer>>) =
            match reviewers with
            | None
            | Some [] -> None
            | Some az ->
                az
                |> List.map (fun rv ->
                    let result: Reviewer =
                        { Name = rv.DisplayName
                          Vote =
                            match rv.Vote with
                            | AZ.PullRequestVote.approved
                            | AZ.PullRequestVote.approvedWithSuggestions -> PullRequestVote.approved
                            | AZ.PullRequestVote.rejected -> PullRequestVote.rejected
                            | _ -> PullRequestVote.noVote }

                    result)
                |> Some

        prs.Value
        |> List.map (fun pr ->
            { Id = pr.PullRequestId.ToString()
              Title = pr.Title
              Status =
                match pr.Status with
                | AZ.PullRequestStatus.abandoned -> PullRequestStatus.abandoned
                | AZ.PullRequestStatus.active -> PullRequestStatus.active
                | AZ.PullRequestStatus.completed -> PullRequestStatus.completed
                | _ -> PullRequestStatus.other
              CreatedBy = pr.CreatedBy.DisplayName
              CreatedOn = pr.CreationDate
              ClosedOn = pr.ClosedDate
              SourceBranch = pr.SourceRefName.Replace("refs/heads/", "")
              TargetBranch = pr.TargetRefName.Replace("refs/heads/", "")
              IsDraft = pr.IsDraft
              Reviewers = convertReviewers pr.Reviewers })

    let private downloadGithubPRs settings = failwith "not implemented"

    let private downloadPullRequests (settings: ControllerSettings) =
        match settings with
        | AzureDevOps az -> downloadAzureDevOpsPRs az
        | Github gh -> downloadGithubPRs gh

    let private run() =
        printfn "Fetching pull requests..."

        let settings = Settings.getSettings()
        let prs = downloadPullRequests settings.Value

        (Utils.getFilePath "prs.json", JsonSerializer.Serialize(prs))
        |> File.WriteAllText

        Utils.printOk <| "Fetch completed!"

    /// <summary>
    /// Checks if the fetch operation is done
    /// </summary>
    let isFetchDone () = 
        (Utils.getFilePath "prs.json") |> File.Exists

    let cmd =
        command "fetch" {
            description "Gets the latest data from your version controller."
            setHandler run
        }