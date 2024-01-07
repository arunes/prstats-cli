namespace Models

open System
open System.ComponentModel.DataAnnotations

module Common =
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

    type Reviewer = { Name: string; Vote: PullRequestVote }

    type PullRequest =
        { Id: string
          Title: string
          Status: PullRequestStatus
          CreatedBy: string
          CreatedOn: DateTime
          ClosedOn: Option<DateTime>
          SourceBranch: string
          TargetBranch: string
          IsDraft: bool
          Reviewers: Option<List<Reviewer>> }
