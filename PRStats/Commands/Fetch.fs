namespace Commands

open Models.Data
open System.IO
open Extensions
open FSharp.SystemCommandLine

module AZ = Models.AzureDevOps

module Fetch =
    open Flurl.Http

    let private downloadAzureDevOpsPRs (settings: Settings) =
        let response =
            settings.azureApiUrl
                .AppendPathSegment("pullrequests")
                .AppendQueryParam("$top", "500")
                .AppendQueryParam("searchCriteria.status", "all")
                .GetJson<AZ.ApiResponse<AZ.PullRequest>>()

        // if pr.count > limit pull next

        let convertReviewers (pullRequestId: string, reviewers: Option<List<AZ.Reviewer>>) =
            match reviewers with
            | None
            | Some [] -> List.empty
            | Some az ->
                az
                |> List.map (fun rv ->
                    let result: Reviewer =
                        { PullRequestId = pullRequestId
                          Name = rv.DisplayName
                          Vote =
                            match rv.Vote with
                            | AZ.PullRequestVote.approved
                            | AZ.PullRequestVote.approvedWithSuggestions -> PullRequestVote.approved
                            | AZ.PullRequestVote.rejected -> PullRequestVote.rejected
                            | _ -> PullRequestVote.noVote }

                    result)

        let prs =
            response.Value
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
                  IsDraft = pr.IsDraft })

        let reviewers =
            response.Value
            |> List.map (fun pr -> convertReviewers (pr.PullRequestId.ToString(), pr.Reviewers))
            |> List.collect (fun rw -> rw)

        prs, reviewers

    let private downloadGithubPRs settings = failwith "not implemented"

    let private downloadPullRequests (settings: Settings) =
        match settings.Type with
        | VersionControllerType.AzureDevOps -> downloadAzureDevOpsPRs settings
        | VersionControllerType.Github -> downloadGithubPRs settings
        | _ -> failwith "Cannot determine the settings type."

    let private run () =
        printfn "Fetching pull requests..."

        let settings = Data.getSettings ()
        let prs, reviewers = downloadPullRequests settings.Value

        Data.createPullRequests prs
        Data.createPullRequestReviewers reviewers

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
