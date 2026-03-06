# DESIGN.md

This file provides general code design information, and guidance to AI coding agents when working with code in this repository.

## Project Overview

QarnotCLI is a .NET 6 CLI for the Qarnot Computing platform, built with `System.CommandLine` (beta4) and `QarnotSDK`. Assembly name: `qarnot`.

## Build & Test Commands

```bash
# Build
dotnet build

# Run tests (NUnit)
dotnet test

# Run a single test by name
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Run tests in a specific file/class
dotnet test --filter "FullyQualifiedName~TestTask"

# Publish (self-contained binary)
cd QarnotCLI && dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true -o ./dest/bin
```

If you want to use a local development version of the QarnotSDK (in ../csharp/), then set this environment:

```bash
export UseLocalDevelopmentQarnotSDK=true
```

## Architecture

The CLI uses a **command pattern** with `System.CommandLine`. Each resource (task, pool, job, bucket, secrets, etc.) lives under `QarnotCLI/Commands/<Resource>/` with these files:

- **Command.cs** - Defines CLI arguments/options, subcommands, and binds handlers via `SetHandler`
- **Binders.cs** - Translates `System.CommandLine` `BindingContext` into business model objects
- **Models.cs** - Logicless records consumed by use cases; inherit from `GlobalModel`
- **UseCases.cs** - Business logic; each function takes a model and uses `ILogger`/`IFormatter` for output
- **Options.cs** (optional) - Shared option definitions to reduce boilerplate

### Global infrastructure (`Commands/Global/`)

- **GlobalOptions/GlobalBinder/GlobalModel** - Common settings (token, API URL, etc.) inherited by all commands that interact with the API
- **UseCasesFactory** - Simple DI: auto-instantiates `Connection`, `IFormatter`, `ILogger` and constructs use cases. All use cases must have a constructor taking `(Connection, IFormatter, ILogger)` in that order.
- **CommandLineBuilderFactory** - Assembles the full command tree

### Entry point

`QarnotCLI/Program.cs` - Top-level statements: creates factories, parses connection config, builds the command tree, invokes the parser.

### Key supporting files

- `QarnotCLI/Formatter.cs` - Output formatting (JSON, table, etc.)
- `QarnotCLI/Logger.cs` - Logging abstraction
- `QarnotCLI/StateManager.cs` - State management
- `QarnotCLI/ConnectionConfiguration.cs` - API connection config parsing
- `QarnotCLI/Helpers/` - Shared utilities (JSON parsing, help layout, examples)

### Tests

- `QarnotCLI.Test/Parser/` - Command-line parsing tests per resource (NUnit + Moq + AutoFixture)
- `QarnotCLI.Test/Mocks/MockParser.cs` - Shared test mock for the parser

## Adding a New Command

See `QarnotCLI/README.md` for the full guide. In short: create `Command.cs`, `Binders.cs`, `Models.cs`, `UseCases.cs` under `Commands/<NewResource>/`, inherit from `GlobalModel`/`GlobalBinder`, and register the command in `CommandLineBuilderFactory`.

## Coding style

### Language Settings
- C# 14 as long as compatible with the build targets
- 4 spaces indentation (no tabs)
- File-scoped namespaces: `namespace Qarnot.Module;`

### Naming Conventions
- **PascalCase**: Classes, interfaces, methods, properties, enums, constants
- **camelCase**: Local variables, parameters
- **_camelCase**: Private fields
- **I prefix**: Interfaces (e.g., `ILeaderInfoService`)
- **A prefix**: Abstract base classes (e.g., `ABaseClass`)

### Import Organization
- Place `using` statements outside namespace
- Order: .NET libraries (alphabetical) → blank line → third-party → blank line → internal
- when you need a new namespace in scope, add a `using`, don't use fully qualified names inline in code


### Bracing, indentation, and other coding style

First of all, this is for new or edited code. **Do not modify code just for bracing or indentation style**.

You'll follow mostly an Allman style:
- braces are alone on a new line, at the indentation level of the above line
- **DO NOT** omit braces for one-line bodies of loops or conditions
- every function declaration with 3 arguments or more have their arguments wrapped and aligned. Same for call sites, unless arguments are very short (like 3 integers or something like that)
- be agressive on extracting methods with meaningful names, even if they're called only once
- also use enough intermediate variables and use explicit names. It's OK if the names are a bit long. Not ridiculously long, but don't shorten them agressively.

As a general rule, let code breathe. Group lines that go together but feel free to insert a blank like to separate logical groups of lines.
