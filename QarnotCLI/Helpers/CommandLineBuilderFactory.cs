using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.IO;
using QarnotSDK;

namespace QarnotCLI;

public class CommandLineBuilderFactory
{
    private readonly Func<GlobalModel, ITaskUseCases> TaskUseCasesFactory;
    private readonly Func<GlobalModel, IPoolUseCases> PoolUseCasesFactory;
    private readonly Func<GlobalModel, IHardwareConstraintsUseCases> HardwareConstraintsUseCasesFactory;
    private readonly Func<GlobalModel, IJobUseCases> JobUseCasesFactory;
    private readonly Func<GlobalModel, IBucketUseCases> BucketUseCasesFactory;
    private readonly Func<GlobalModel, IAllUseCases> AllUseCasesFactory;
    private readonly Func<GlobalModel, ISecretsUseCases> SecretsUseCasesFactory;
    private readonly Func<GlobalModel, IConfigUseCases> ConfigUseCasesFactory;
    private readonly Func<GlobalModel, IAccountUseCases> AccountUseCasesFactory;

    public CommandLineBuilderFactory(UseCasesFactory useCasesFactory)
        : this(
            useCasesFactory.Create<TaskUseCases>,
            useCasesFactory.Create<PoolUseCases>,
            useCasesFactory.Create<HardwareConstraintsUseCases>,
            useCasesFactory.Create<JobUseCases>,
            useCasesFactory.Create<BucketUseCases>,
            options =>
                new AllUseCases(
                    useCasesFactory.Create<TaskUseCases>(options),
                    useCasesFactory.Create<PoolUseCases>(options),
                    useCasesFactory.Create<JobUseCases>(options),
                    useCasesFactory.Create<BucketUseCases>(options),
                    useCasesFactory.LoggerFactory.Create(options)
                ),
            useCasesFactory.Create<SecretsUseCases>,
            options => new ConfigUseCases(useCasesFactory.LoggerFactory.Create(options)),
            useCasesFactory.Create<AccountUseCases>
        )
    {
    }

    public CommandLineBuilderFactory(
        Func<GlobalModel, ITaskUseCases> taskUseCasesFactory,
        Func<GlobalModel, IPoolUseCases> poolUseCasesFactory,
        Func<GlobalModel, IHardwareConstraintsUseCases> hardwareConstraintsUseCasesFactory,
        Func<GlobalModel, IJobUseCases> jobUseCasesFactory,
        Func<GlobalModel, IBucketUseCases> bucketUseCasesFactory,
        Func<GlobalModel, IAllUseCases> allUseCasesFactory,
        Func<GlobalModel, ISecretsUseCases> secretsUseCasesFactory,
        Func<GlobalModel, IConfigUseCases> configUseCasesFactory,
        Func<GlobalModel, IAccountUseCases> accountUseCasesFactory
    )
    {
        TaskUseCasesFactory = taskUseCasesFactory;
        PoolUseCasesFactory = poolUseCasesFactory;
        HardwareConstraintsUseCasesFactory = hardwareConstraintsUseCasesFactory;
        JobUseCasesFactory = jobUseCasesFactory;
        BucketUseCasesFactory = bucketUseCasesFactory;
        AllUseCasesFactory = allUseCasesFactory;
        SecretsUseCasesFactory = secretsUseCasesFactory;
        ConfigUseCasesFactory = configUseCasesFactory;
        AccountUseCasesFactory = accountUseCasesFactory;
    }

    public CommandLineBuilder Create(
        ConnectionConfiguration config,
        IReleasesService releasesService,
        ILogger logger
    )
    {
        var globalOptions = new GlobalOptions(config);
        var assemblyDetails = releasesService.GetAssemblyDetails();

        var rootCommand = new RootCommand()
        {
            new TaskCommand(globalOptions, TaskUseCasesFactory),
            new PoolCommand(globalOptions, PoolUseCasesFactory),
            new HardwareConstraintsCommand(globalOptions, HardwareConstraintsUseCasesFactory),
            new JobCommand(globalOptions, JobUseCasesFactory),
            new BucketCommand(globalOptions, BucketUseCasesFactory),
            new AllCommand(globalOptions, AllUseCasesFactory),
            new SecretsCommands(globalOptions, SecretsUseCasesFactory),
            new ConfigCommand(globalOptions, ConfigUseCasesFactory),
            new AccountCommand(globalOptions, AccountUseCasesFactory),
            new VersionCommand(assemblyDetails, logger),
        }.AddGlobalOptions(globalOptions);

        var versionOpt = new Option<bool>(
            name: "--version",
            description: "The current version"
        );
        rootCommand.SetHandler(
            version => {
                if (version) {
                    Console.WriteLine(assemblyDetails.ToString());
                } else {
                    throw new DisplayHelpException(rootCommand);
                }
            },
            versionOpt
        );
        rootCommand.Add(versionOpt);

        return new CommandLineBuilder(rootCommand)
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .CancelOnProcessTermination()
            // Disable the default handling for `@` on the command line.
            .UseTokenReplacer((string _1, out IReadOnlyList<string>? _2, out string? _3) => {
                _2 = null;
                _3 = null;
                return false;
            })
            .UseCustomHelp(assemblyDetails)
            .UseExceptionHandler((exc, ctx) =>
            {
                if (exc is QarnotApiException)
                {
                    logger.Error(exc,"An error occurred while connecting to Qarnot API :");
                    ctx.ExitCode = 1;
                }
                else if (exc is DisplayHelpException displayHelpException)
                {
                    ctx.HelpBuilder.Write(
                        displayHelpException.Command,
                        ctx.Console.Out.CreateTextWriter()
                    );
                }
                else
                {
                    logger.Error(exc, "An error occured :");
                    ctx.ExitCode = 1;
                }
            })
            .UseParseErrorReporting(errorExitCode: 1);
    }
}
