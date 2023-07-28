# QarnotCLI

The CLI uses `System.CommandLine` to handle the CLI part of the code. See [this page](https://learn.microsoft.com/en-us/dotnet/standard/commandline/define-commands) to have an overview of the library.

## Anatomy of a command

Most commands are composed of the following files:

* `./Commands/<Command>/Command.cs`: Where the command line arguments are
  defined and the command is bound to a function to be executed.

* `./Commands/<Command>/Options.cs`: Only found for commands that reuse the
  same options a lot. This is where we factor those options in single object
  to reduce the boilerplate of defining them and adding them to commands.

* `./Commands/<Command>/Binders.cs`: Responsible for binding the result of the
  parsing to business objects called "models".

* `./Commands/<Command>/Models.cs`: The result of the parsing of the command
  line.

* `./Commands/<Command>/UseCases.cs`: The business logic to execute for each
  command.

### `Global{Options, Binder, Model}`

Before diving into how a command is made, it's important to understand how
common settings are passed down:

* `GlobalOptions`: all the commands that interact with the API will need to
  take a `GlobalOptions` parameter. This `GlobalOptions` object encapsulate all
  the common settings (`--token`, etc.) and is needed for properly accessing
  the parsing of these settings in the business code. They are simply passed
  down to the binder of each command.

* `GlobalBinder`: must be the parent class of all the binders (except if you
  don't need to access the API at all). It takes care of binding all the
  global settings to any model that inherit from `GlobalModel`. All its
  children must implement a `GetBoundValueImpl` method _instead of_ the
  `GetBoundValue` normally required by `System.CommandLine`.

* `GlobalModel`: must be the parent record of all the models (except if you
  don't need to access the API at all). `GlobalModel` simply defines the global
  settings, to avoid having to duplicate them in every single model record.

### `Command.cs`

Inside the `Command.cs` file, you should find a sinle class definition,
inheriting from either `System.CommandLine.Command` or `CommandWithExamples`.
Its role is just to build a `System.CommandLine.Command` (the `task` in `qarnot
task create`) and its subcommands (the `create` in `qarnot task create`). Each
subcommand is defined in 3 broad steps, generally encapsulated in a
`Build<Command>Command` function.

* Defining `Example`, `Option` and `Argument`

* Instantiating the command itself and adding the `Example`, `Option` and
  `Argument`

* Setting the handler for the command by using the `SetHandler` function. This
  function takes a function and a binder. Binder are objects translating
  what is parsed by `System.CommandLine` to a business object.

For the command itself, we simply add the subcommand individually with
`AddCommand`.

### `Binder.cs`

The binding is mostly boilerplate code, it interprets the `BindingContext`
given to us by `System.CommandLine` and build business objects (we call them
"model" in this code base). Binders inherit from `GlobalBinder` in order to
save the boilerplate of binding the global settings.

For a few binders (`UpdatePoolsScalingBinder` for example), this is where is
located the parsing of various additional JSON files.

### `Model.cs`

Models are logicless records containing what the `UseCases` need to execute its
business logic. They inherit from `GlobalModel` in order to all have the global
settings (`Token` for the API) without having to re-define them all the time.

### `UseCases.cs`

The `UseCases` contain all the business logic. Each `UseCases` (for example
`TaskUseCases`) is a collection of function (`Create`, `Delete`, etc.).

Each function takes a model and is responsible for the printing of the result
using its `ILogger` and `IFormatter`.

Because `System.CommandLine` doesn't really offer a dependency injection system
we have made a really simple one ourselves. The idea is that we have a
`UseCasesFactory` that automates the instantiation of a `QarnotSDK.Connection`,
an `IFormatter` and an `ILogger` and build the `UseCases`. It means that
all the `UseCases` you want to instantiate with `UseCasesFactory` must have
a constructor that takes all three (and only those three) in that specific
order: `QarnotSDK.Connection`, `IFormatter`, `ILogger`.
