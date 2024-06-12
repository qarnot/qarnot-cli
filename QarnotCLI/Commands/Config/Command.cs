using System.CommandLine;

namespace QarnotCLI;

public class ConfigCommand : CommandWithExamples
{
    private readonly Func<GlobalModel, IConfigUseCases> Factory;
    private readonly GlobalOptions GlobalOptions;
    public ConfigCommand(GlobalOptions globalOptions, Func<GlobalModel, IConfigUseCases> factory)
        : base("config", "Configure the CLI options")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        AddCommand(BuildSetConfigCommand());
        AddCommand(BuildShowConfigCommand());
    }

    private Command BuildSetConfigCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot config set --token ___TOKEN___"
                }
            ),
            new Example(
                Title: "Regular usage with a personal API URI",
                CommandLines: new[] {
                    "qarnot config set --storage-uri=https://storage.qarnot.com --token=___TOKEN___ --api-uri=https://api.qarnot.com"
                }
            ),
            new Example(
                Title: "Set configuration in the local configuration file",
                CommandLines: new[] {
                    "qarnot config set --local -t ___TOKEN___"
                }
            )
        };

        // It's not possible to add `-t` as an alias for all the commands because it's also
        // used for tags when searching for tasks and pools. We add it here.
        GlobalOptions.TokenOpt.AddAlias("-t");

        var localOpt = new Option<bool>(
            aliases: new[] { "--local", "-l" },
            description: "Set the configuration file in the local folder ($PWD/.Qarnot/) to use it when inside the current folder."
        );

        var showOpt = new Option<bool>(
            aliases: new[] { "--show", "-w" },
            description: "Display the connection information that will be used in the connection (check also the environment variables)."
        );

        var apiUriOpt = new Option<string>(
            aliases: new[] { "--api-uri", "-u" },
            description: "The API URI to use"
        );

        var storageUriOpt = new Option<string>(
            aliases: new[] { "--storage-uri", "-s" },
            description: "The bucket API URI to use"
        );

        var accountEmailOpt = new Option<string>(
            aliases: new[] { "--account-email", "-e" },
            description: "The use account email address"
        );

        var forceStoragePathStyleOpt = new Option<bool?>(
            aliases: new[] { "--force-storage-path-style", "-f" },
            description: "Force storage path style"
        );

        var noSanitizeBucketPathOpt = new Option<bool?>(
            name: "--no-sanitize-bucket-path",
            description: "Disable automatic sanitization of bucket paths"
        );

        var storageUnsafeSslOpt = new Option<bool?>(
            name: "--storage-unsafe--url",
            description: "Bypass SSL check for storage connection"
        );

        var cmd = new CommandWithExamples("set", "Set configuration options")
        {
            examples
        };

        cmd.AddOption(localOpt);
        cmd.AddOption(showOpt);
        cmd.AddOption(apiUriOpt);
        cmd.AddOption(storageUriOpt);
        cmd.AddOption(accountEmailOpt);
        cmd.AddOption(forceStoragePathStyleOpt);
        cmd.AddOption(noSanitizeBucketPathOpt);
        cmd.AddOption(storageUnsafeSslOpt);

        cmd.SetHandler(
            model => Factory(model).SetConfig(model),
            new SetConfigBinder(
                localOpt,
                showOpt,
                apiUriOpt,
                storageUriOpt,
                accountEmailOpt,
                forceStoragePathStyleOpt,
                noSanitizeBucketPathOpt,
                storageUnsafeSslOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildShowConfigCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot config show"
                }
            ),
            new Example(
                Title: "Show global configuration file",
                CommandLines: new[] {
                    "qarnot config show --global"
                }
            )
        };

        var showGlobalOpt = new Option<bool>(
            aliases: new[] { "--global", "-g" },
            description: "Show global configuration file"
        );

        var withoutEnvOpt = new Option<bool>(
            aliases: new[] { "--without-env" },
            description: "Show the raw configuration file without options passed down by environment variables"
        );

        var cmd = new CommandWithExamples("show", "Show configuration file")
        {
            examples
        };
        cmd.AddOption(showGlobalOpt);
        cmd.AddOption(withoutEnvOpt);

        cmd.SetHandler(
            model => Factory(model).ShowConfig(model),
            new ShowConfigBinder(
                showGlobalOpt,
                withoutEnvOpt,
                GlobalOptions
            )
        );

        return cmd;
    }
}
