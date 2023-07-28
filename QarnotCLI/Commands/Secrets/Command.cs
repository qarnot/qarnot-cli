using System.CommandLine;

namespace QarnotCLI;

public class SecretsCommands : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, ISecretsUseCases> Factory;

    public SecretsCommands(GlobalOptions globalOptions, Func<GlobalModel, ISecretsUseCases> factory)
        : base("secrets", "Secrets commands")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        AddCommand(BuildGetCommand());
        AddCommand(BuildCreateCommand());
        AddCommand(BuildUpdateCommand());
        AddCommand(BuildDeleteCommand());
        AddCommand(BuildListCommand());
    }

    private Command BuildGetCommand()
    {
        var example = new Example(
            Title: "Get the value of a secret",
            CommandLines: new[] {
                "qarnot secrets get path/to/secret"
            }
        );

        var keyArg = new Argument<string>(
            name: "key",
            description: "Key of the secret to retrieve"
        );

        var cmd = new CommandWithExamples("get", "Get the value of a secret")
        {
            example,
            keyArg,
        };

        cmd.SetHandler(
            model => Factory(model).Get(model),
            new GetSecretBinder(
                keyArg,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildCreateCommand()
    {
        var example = new Example(
            Title: "Create a new secret",
            CommandLines: new[] {
                "qarnot secrets create path/to/secret value"
            }
        );

        var keyArg = new Argument<string>(
            name: "key",
            description: "Key of the secret to create"
        );

        var valueArg = new Argument<string>(
            name: "value",
            description: "Value of the secret to create"
        );

        var cmd = new CommandWithExamples("create", "Create a new secret")
        {
            example,
            keyArg,
            valueArg,
        };

        cmd.SetHandler(
            model => Factory(model).Create(model),
            new WriteSecretBinder(
                keyArg,
                valueArg,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildUpdateCommand()
    {
        var example = new Example(
            Title: "Update an existing secret",
            CommandLines: new[] {
                "qarnot secrets update path/to/secret newValue"
            }
        );

        var keyArg = new Argument<string>(
            name: "key",
            description: "Key of the secret to update"
        );

        var newValueArg = new Argument<string>(
            name: "newValue",
            description: "New value for the secret to update"
        );

        var cmd = new CommandWithExamples("update", "Update an existing secret")
        {
            example,
            keyArg,
            newValueArg,
        };

        cmd.SetHandler(
            model => Factory(model).Update(model),
            new WriteSecretBinder(
                keyArg,
                newValueArg,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildDeleteCommand()
    {

        var example = new Example(
            Title: "Delete an existing secret",
            CommandLines: new[] {
                "qarnot secrets delete path/to/secret"
            }
        );

        var keyArg = new Argument<string>(
            name: "key",
            description: "Key of the secret to delete"
        );

        var cmd = new CommandWithExamples("delete", "Delete an existing secret")
        {
            example,
            keyArg,
        };

        cmd.SetHandler(
            model => Factory(model).Delete(model),
            new GetSecretBinder(
                keyArg,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildListCommand()
    {

        var examples = new[] {
            new Example(
                Title: "List all the root secret entries",
                CommandLines: new[] { "qarnot secrets list" }
            ),
            new Example(
                Title: "List all the secret keys",
                CommandLines: new[] { "qarnot secrets list -r" }
            ),
            new Example(
                Title: "List all the secret keys starting with a prefix",
                CommandLines: new[] { "qarnot secrets list -r pre/fix" }
            )
        };

        var prefixArg = new Argument<string>(
            name: "prefix",
            description: "Prefix of the secret to update",
            getDefaultValue: () => ""
        );

        var recursiveOpt = new Option<bool>(
            aliases: new[] { "--recursive", "-r" },
            description: "Perform a recursive listing. When performing a non-recursive listing, only entries right below `prefix` will be returned: `prefix/a` but not `prefix/a/b`. Subsequent prefixes can be identified by their trailing `/`."
        );

        var cmd = new CommandWithExamples("list", "Delete an existing secret")
        {
            examples,
            prefixArg,
            recursiveOpt,
        };

        cmd.SetHandler(
            model => Factory(model).List(model),
            new ListSecretBinder(
                prefixArg,
                recursiveOpt,
                GlobalOptions
            )
        );

        return cmd;
    }
}
