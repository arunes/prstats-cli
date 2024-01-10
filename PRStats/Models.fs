namespace Models

open System
open Flurl.Http
open System.ComponentModel.DataAnnotations

module Common =
    type Report = { Id: int; Name: string }

module AzureDevOps =
    type PullRequestStatus =
        | Abandoned = 1
        | Active = 2
        | All = 3
        | Completed = 4
        | NotSet = 5

    type PullRequestVote =
        | Approved = 10
        | ApprovedWithSuggestions = 5
        | NoVote = 0
        | WaitingForAuthor = -5
        | Rejected = -10

    type User =
        { Id: Guid
          DisplayName: string
          UniqueName: string }

    type Reviewer =
        { Id: Guid
          DisplayName: string
          UniqueName: string
          Vote: PullRequestVote }

    type PullRequest =
        { Title: string
          PullRequestId: int
          Status: PullRequestStatus
          CreatedBy: User
          CreationDate: DateTime
          ClosedDate: Option<DateTime>
          SourceRefName: string
          TargetRefName: string
          IsDraft: bool
          Reviewers: Option<List<Reviewer>> }

    type ApiResponse<'T> = { Count: int; Value: List<'T> }

module Data =
    type VersionControllerType =
        | [<Display(Name = "Azure DevOps")>] AzureDevOps = 1
        | [<Display(Name = "Github")>] Github = 2

    type PullRequestStatus =
        | active = 1
        | completed = 2
        | abandoned = 3
        | other = 4

    type PullRequestVote =
        | approved = 1
        | rejected = 2
        | noVote = 3

    [<CLIMutable>]
    type Reviewer =
        { PullRequestId: string
          Name: string
          Vote: PullRequestVote }

    [<CLIMutable>]
    type PullRequest =
        { Id: string
          Title: string
          Status: PullRequestStatus
          CreatedBy: string
          CreatedOn: DateTime
          ClosedOn: Option<DateTime>
          SourceBranch: string
          TargetBranch: string
          IsDraft: bool }

    [<CLIMutable>]
    type Settings =
        { Type: VersionControllerType
          Owner: string
          Project: Option<string>
          Repo: string
          Token: string }

    type Settings with

        member self.azureApiUrl =
            $"https://dev.azure.com/{self.Owner}/{self.Project.Value}/_apis/git/repositories/{self.Repo}"
                .WithBasicAuth("", self.Token)
