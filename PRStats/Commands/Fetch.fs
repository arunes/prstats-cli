﻿namespace Commands

open Models.Data
open Extensions
open FSharp.SystemCommandLine
open Flurl.Http

module AZ = Models.AzureDevOps

module Fetch =
    let private downloadAzureDevOpsPRs (settings: Settings) =
        let downloadLimit = 500

        let rec downloadPRs (skip: int) =
            let response =
                settings.azureApiUrl
                    .AppendPathSegment("pullrequests")
                    .AppendQueryParam("$top", downloadLimit)
                    .AppendQueryParam("$skip", skip)
                    .AppendQueryParam("searchCriteria.status", "all")
                    .GetJson<AZ.ApiResponse<AZ.PullRequest>>()

            seq {
                yield! response.Value

                if response.Count = downloadLimit then
                    yield! downloadPRs (skip + downloadLimit)
            }

        let apiResponse = downloadPRs 0 |> Seq.toList

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
                            | AZ.PullRequestVote.Approved
                            | AZ.PullRequestVote.ApprovedWithSuggestions -> PullRequestVote.approved
                            | AZ.PullRequestVote.Rejected -> PullRequestVote.rejected
                            | _ -> PullRequestVote.noVote }

                    result)

        let prs =
            apiResponse
            |> List.map (fun pr ->
                { Id = pr.PullRequestId.ToString()
                  Title = pr.Title
                  Status =
                    match pr.Status with
                    | AZ.PullRequestStatus.Abandoned -> PullRequestStatus.abandoned
                    | AZ.PullRequestStatus.Active -> PullRequestStatus.active
                    | AZ.PullRequestStatus.Completed -> PullRequestStatus.completed
                    | _ -> PullRequestStatus.other
                  CreatedBy = pr.CreatedBy.DisplayName
                  CreatedOn = pr.CreationDate
                  ClosedOn = pr.ClosedDate
                  SourceBranch = pr.SourceRefName.Replace("refs/heads/", "")
                  TargetBranch = pr.TargetRefName.Replace("refs/heads/", "")
                  IsDraft = pr.IsDraft })

        let reviewers =
            apiResponse
            |> List.map (fun pr -> convertReviewers (pr.PullRequestId.ToString(), pr.Reviewers))
            |> List.collect (fun rw -> rw)

        prs, reviewers

    let private downloadPullRequests (settings: Settings) =
        match settings.Type with
        | VersionControllerType.AzureDevOps -> downloadAzureDevOpsPRs settings
        | _ -> failwith "Cannot determine the settings type."

    let private run () =
        printfn "Fetching pull requests..."

        let settings = Data.getSettings ()
        let prs, reviewers = downloadPullRequests settings.Value

        printfn "Saving pull requests to database..."
        Data.createPullRequests prs
        Data.createPullRequestReviewers reviewers

        Utils.printOk
        <| $"Fetch completed! Fetched {prs.Length} pull requests and {reviewers.Length} reviewers records."

    let cmd =
        let handler () =
            try
                run ()
            with ex ->
                Utils.printError <| ex.Message

        command "fetch" {
            description "Gets the latest data from your version controller."
            setHandler handler
        }
