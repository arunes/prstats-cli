namespace Models

open Flurl.Http
open System

module AzureDevOps =
    type Settings =
        { Type: Common.VersionControllerType
          Organization: string
          Project: string
          RepositoryId: string
          PAT: string }

    type Settings with

        member self.apiUrl =
            $"https://dev.azure.com/{self.Organization}/{self.Project}/_apis/git/repositories/{self.RepositoryId}"
                .WithBasicAuth("", self.PAT)

    type PullRequestStatus =
        | abandoned = 1
        | active = 2
        | all = 3
        | completed = 4
        | notSet = 5

    type PullRequestVote =
        | approved = 10
        | approvedWithSuggestions = 5
        | noVote = 0
        | waitingForAuthor = -5
        | rejected = -10

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
