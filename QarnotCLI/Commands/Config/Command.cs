using System.CommandLine;

namespace QarnotCLI;

public class ConfigCommand : CommandWithExamples
{
    public ConfigCommand(GlobalOptions globalOptions, Func<GlobalModel, IConfigUseCases> factory)
        : base("config", "Configure the CLI options")
    {
        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot config --token ___TOKEN___"
                }
            ),
            new Example(
                Title: "Regular usage with a personal API URI",
                CommandLines: new[] {
                    "qarnot config --storage-uri=https://storage.qarnot.com --token=___TOKEN___ --api-uri=https://api.qarnot.com"
                }
            ),
        };

        var globalOpt = new Option<bool>(
            aliases: new[] { "--global", "-g" },
            description: "Set the configuration in the global default file ($HOME/.Qarnot/) to use it outside the binary path."
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

        AddExamples(examples);
        AddOption(globalOpt);
        AddOption(showOpt);
        AddOption(apiUriOpt);
        AddOption(storageUriOpt);
        AddOption(accountEmailOpt);
        AddOption(forceStoragePathStyleOpt);
        AddOption(noSanitizeBucketPathOpt);
        AddOption(storageUnsafeSslOpt);

        this.SetHandler(
            model => factory(model).Run(model),
            new RunConfigBinder(
                globalOpt,
                showOpt,
                apiUriOpt,
                storageUriOpt,
                accountEmailOpt,
                forceStoragePathStyleOpt,
                noSanitizeBucketPathOpt,
                storageUnsafeSslOpt,
                globalOptions
            )
        );
    }
}
