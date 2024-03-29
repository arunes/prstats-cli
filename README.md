# PR Stats CLI - The CLI tool for pull request stats

![PR Stats CLI logo](https://raw.githubusercontent.com/arunes/prstats-cli/main/terminal.png)

_The PR Stats CLI is a command-line interface tool that you use to get stats about pull requests directly from a command shell._

### Supported version control platforms

- Azure DevOps Git

### Roadmap
- Support GitHub
- Ability to use multiple accounts
- Ability to use multiple repos


## Getting started

You can install the PR Stats CLI tool from Chocolatey, as a dotnet tool or you can install manually by downloading pre complied version from [Releases](https://github.com/arunes/prstats-cli/releases) page.

### Installing with Chocolatey
To install PR Stats CLI using [Chocolatey](https://chocolatey.org/), run the following command from your command line / PowerShell:
```powershell
choco install prstats-cli
```

To upgrade PR Stats CLI, run:
```powershell
choco upgrade prstats-cli
```

To verify PR Stats CLI was successfully installed:
```bash
prstats --version
```

### Installing as a NET Tool
To install PR Stats CLI as .NET tool:
```bash
dotnet tool install --global prstats-cli
```

To verify PR Stats CLI was successfully installed:
```bash
prstats --version
```

## Usage

The tool downloads your pull request data and runs commands on the offline data. If you need to update your data see [fetch](#fetch-fetch) command.

```bash
Description:
  PR Stats CLI - The CLI tool for pull request stats

Usage:
  prstats [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  fetch    Gets the latest data from your version controller.
  purge    Cleans up your version controller settings and pull requests data.
  reports  Lists all the available reports.
  run      Gets pull request stats.
  setup    Runs the wizard to setup your version controller.
```

### Pull requests `run`

```bash
Description:
  Runs pull request reports.

Usage:
  prstats run [options]

Options:
  --status <Abandoned|Active|All|Completed>  Filter by status. [default: Completed]
  --branch <branch>                          Filter by status. []
  --before <before>                          Filter by date (shows pull requests before and on. Date format must be in
                                             YYYY-MM-DD. []
  --after <after>                            Filter by date (shows pull requests after and on. Date format must be in
                                             YYYY-MM-DD. []
  --date-type <Completed|Created>            Specify a date type. [default: Completed]
  --report-id <report-id>                    Specify a report id. You can get list of reports by running `reports`
                                             command. []
  -?, -h, --help                             Show help and usage information
```

#### Examples

Shows results only for active pull requests:

```bash
prstats run --status active
```

Shows results for completed pull requests that are created after Jan 1st, 2024:

```bash
prstats run --status completed --date-type created
```

### Setup `setup`

```bash
Description:
  Runs the wizard to setup your version controller.

Usage:
  prstats setup [options]

Options:
  -?, -h, --help  Show help and usage information
```

### Fetch `fetch`

```bash
Description:
  Gets the latest data from your version controller.

Usage:
  prstats fetch [options]

Options:
  -?, -h, --help  Show help and usage information
```

### Reports `reports`

```bash
Description:
  Lists all the available reports.

Usage:
  prstats reports [options]

Options:
  -?, -h, --help  Show help and usage information
```

### Purge `purge`

```bash
Description:
  Cleans up your version controller settings and pull requests data.

Usage:
  prstats purge [options]

Options:
  --data-only     Only deletes the data and not version controller settings. [default: False]
  -?, -h, --help  Show help and usage information
```

#### Examples

Purge pull request data and version controller settings:

```bash
prstats purge
```

Purge pull request data:

```bash
prstats purge --data-only
```
