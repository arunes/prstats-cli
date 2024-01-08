
# PR Stats CLI - The CLI tool for pull request stats

![PR Stats CLI logo](terminal.png)

*The PR Stats CLI is a command-line interface tool that you use to get stats about pull requests directly from a command shell.*

### Supported version control platforms

 - [x] Azure DevOps Git
 - [ ] Github

## Getting started
You can download pre compiled cli tool from below, or you can build it from source.

### Download 

| Target                   | OS                                      | Download |
| ------------------------ | --------------------------------------- | -------- |
| x86_64-apple-darwin      | 64-bit macOS (10.12+, Sierra+)          | -        |
| x86_64-pc-windows-msvc   | 64-bit MSVC (Windows 7+)                | -        |
| x86_64-unknown-linux-gnu | 64-bit Linux (kernel 3.2+, glibc 2.17+) | -        |

> If your OS is not listed here, you can build the project from the source code.


### Building from source
You will need [rustup] to build the tool from the source.

Clone the repository:

```bash
git clone https://github.com/arunes/prstats-cli.git
```

Go to the source code folder in your terminal, and build:

```
cargo build
```

## Usage

The tool downloads your pull request data and runs commands on the offline data. If you need to update your data see [fetch](#fetch-fetch) command.

> You can see the date and time the your data was downloaded after running any stat command.

```bash
Description:
  PR Stats CLI - The CLI tool for pull request stats

Usage:
  PRStats [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  run    Gets pull request stats.
  setup  Runs the wizard to setup your version controller.
  fetch  Gets the latest data from your version controller.
  purge  Cleans up your version controller settings and pull requests data.
```

### Pull requests `run`

```bash
Description:
  Gets pull request stats.

Usage:
  PRStats run [options]

Options:
  --status <Abandoned|Active|All|Completed>  Filter by status. [default: Completed]
  --branch <branch>                          Filter by status. []
  --before <before>                          Filter by date (shows pull requests before and on. Date format must be in
                                             YYYY-MM-DD. []
  --after <after>                            Filter by date (shows pull requests after and on. Date format must be in
                                             YYYY-MM-DD. []
  --date-type <Completed|Created>            Specify a date type. [default: Completed]
  -?, -h, --help                             Show help and usage information
```

#### Examples

Get number of pull requests by user:

```bash
prstats run --type count --group-by user
```

Get number of open pull requests by branch:

```bash
prstats run --type count --group-by branch --status open
```

Get average pull request life by user:

```bash
prstats run --type time --group-by user
```

### Setup `setup`

```bash
Description:
  Runs the wizard to setup your version controller.

Usage:
  PRStats setup [options]

Options:
  -?, -h, --help  Show help and usage information
```

### Fetch `fetch`

```bash
Description:
  Gets the latest data from your version controller.

Usage:
  PRStats fetch [options]

Options:
  -?, -h, --help  Show help and usage information
```

### Purge `purge`

```bash
Description:
  Cleans up your version controller settings and pull requests data.

Usage:
  PRStats purge [options]

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

[rustup]: https://www.rust-lang.org/tools/install
