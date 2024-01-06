module Models

open CommandLine
open System.ComponentModel.DataAnnotations

type OutputType =
    | number = 1
    | time = 2

type GroupBy =
    | user = 1
    | branch = 2

type Status =
    | all = 1
    | active = 2
    | completed = 3
    | abandoned = 4

type DateType =
    | completed = 1
    | created = 2

[<Verb("commits", HelpText = "Gets the pull request commit stats.")>]
type CommitOptions =
    { [<Option("type", HelpText = "Output type.", Default = OutputType.number)>]
      outputType: OutputType
      [<Option("group-by", HelpText = "Group records.", Default = GroupBy.user)>]
      groupBy: GroupBy }

[<Verb("fetch", HelpText = "Gets the latest data from your version controller.")>]
type FetchOptions() =
    class
    end

[<Verb("prs", HelpText = "Gets pull request stats.")>]
type PullRequestOptions =
    { [<Option("type", HelpText = "Output type.", Default = OutputType.number)>]
      outputType: OutputType
      [<Option("group-by", HelpText = "Group records.", Default = GroupBy.user)>]
      groupBy: GroupBy
      [<Option("status", HelpText = "Filter by status.", Default = Status.completed)>]
      status: Status
      [<Option("branch", HelpText = "Filter by target branch.")>]
      branch: string
      [<Option("before", HelpText = "Filter by date (shows pull requests before and on.")>]
      before: string
      [<Option("after", HelpText = "Filter by date (shows pull requests after and on.")>]
      after: string
      [<Option("date-type", HelpText = "Specify a date type.", Default = DateType.completed)>]
      dateType: DateType }

[<Verb("purge", HelpText = "Cleans up your version controller settings and pull requests data.")>]
type PurgeOptions =
    { [<Option("data-only", HelpText = "Only deletes the data and not version controller settings.")>]
      dataOnly: bool }

[<Verb("setup", HelpText = "Runs the wizard to setup your version controller.")>]
type SetupOptions() =
    class
    end

type VersionControllerType =
    | [<Display(Name = "Azure DevOps")>]AzureDevOps = 1
    | [<Display(Name = "Github")>]Github = 2

type AzureDevOpsSettings =
    { Type: VersionControllerType
      Organization: string
      Project: string
      RepositoryId: string
      PAT: string }

type GithubSettings =
    { Type: VersionControllerType }

type Settings = 
    | Azure of AzureDevOpsSettings
    | Github of GithubSettings
