
# PR Stats CLI - The CLI tool for pull request stats

![PR Stats CLI logo](terminal.png)

*The PR Stats CLI is a command-line interface tool that you use to get stats about pull requests directly from a command shell.*

### Supported version control platforms

 - [x] Azure DevOps Git
 - [ ] Github

## Getting started
You can download pre compiled cli tool from below, or you can build it from source.

### Download 

| Target | OS | Download |
|--|--|--|
| x86_64-apple-darwin | 64-bit macOS (10.12+, Sierra+) | - |
| x86_64-pc-windows-msvc | 64-bit MSVC (Windows 7+) | - |
| x86_64-unknown-linux-gnu | 64-bit Linux (kernel 3.2+, glibc 2.17+) | - |

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

### Pull requests `prs`

Gets pull request stats.

```bash
Command:
  prstats prs

Options:
  --type        Output type             [time, number]                         defaults to number
  --group-by    Group records           [user, branch]                         default to user
  --status      Filter by status        [all, open, completed, abandoned]      defaults to completed
  --branch      Filter by branch        [string]                               --branch main, only shows pull requests going into main branch
  --before      Filter by date          [string]                               --before 2023-11-04, shows pull requests before and on 11/04/2023
  --after       Filter by date          [string]                               --after 2023-11-04, shows pull requests after and on 11/04/2023
  --date-type   Specifys a date type    [closed, created]                      defaults to created
```

#### Examples

Get number of pull requests by user:

```bash
prstats prs --type count --group-by user
```

Get number of open pull requests by branch:

```bash
prstats prs --type count --group-by branch --status open
```

Get average pull request life by user:

```bash
prstats prs --type time --group-by user
```

### Commits `commits`

Gets the pull request commit stats.

#### Examples

Get average number of commits by user:

```bash
prstats commits --type count --group-by user
```

### Setup `setup`

Runs the wizard to setup your version controller.

```bash
prstats setup
```

### Fetch `fetch`

Gets the latest data from your version controller.

```bash
prstats fetch
```

### Purge `purge`

Cleans up your version controller settings and pull requests data.

```bash
Command:
  prstats purge

Options:
  --data-only   Only deletes the data and not version controller settings   [boolean]   defaults to false
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
