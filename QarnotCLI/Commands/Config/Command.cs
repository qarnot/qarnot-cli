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

        Add(BuildSetConfigCommand());
        Add(BuildShowConfigCommand());
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
        GlobalOptions.TokenOpt.Aliases.Add("-t");

        var localOpt = new Option<bool>("--local", "-l")
        {
            Description = "Set the configuration file in the local folder ($PWD/.Qarnot/) to use it when inside the current folder.",
        };

        var showOpt = new Option<bool>("--show", "-w")
        {
            Description = "Display the connection information that will be used in the connection (check also the environment variables).",
        };

        var apiUriOpt = new Option<string>("--api-uri", "-u")
        {
            Description = "The API URI to use",
        };

        var storageUriOpt = new Option<string>("--storage-uri", "-s")
        {
            Description = "The bucket API URI to use",
        };

        var accountEmailOpt = new Option<string>("--account-email", "-e")
        {
            Description = "The use account email address",
        };

        var forceStoragePathStyleOpt = new Option<bool?>("--force-storage-path-style", "-f")
        {
            Description = "Force storage path style",
        };

        var noSanitizeBucketPathOpt = new Option<bool?>("--no-sanitize-bucket-path")
        {
            Description = "Disable automatic sanitization of bucket paths",
        };

        var storageUnsafeSslOpt = new Option<bool?>("--storage-unsafe--url")
        {
            Description = "Bypass SSL check for storage connection",
        };

        var cmd = new CommandWithExamples("set", "Set configuration options")
        {
            examples
        };

        cmd.Add(localOpt);
        cmd.Add(showOpt);
        cmd.Add(apiUriOpt);
        cmd.Add(storageUriOpt);
        cmd.Add(accountEmailOpt);
        cmd.Add(forceStoragePathStyleOpt);
        cmd.Add(noSanitizeBucketPathOpt);
        cmd.Add(storageUnsafeSslOpt);

        cmd.SetModelAction(
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

        var showGlobalOpt = new Option<bool>("--global", "-g")
        {
            Description = "Show global configuration file",
        };

        var withoutEnvOpt = new Option<bool>("--without-env")
        {
            Description = "Show the raw configuration file without options passed down by environment variables",
        };

        var cmd = new CommandWithExamples("show", "Show configuration file")
        {
            examples
        };
        cmd.Add(showGlobalOpt);
        cmd.Add(withoutEnvOpt);

        cmd.SetModelAction(
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
