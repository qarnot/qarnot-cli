using System.CommandLine;
using System.CommandLine.Help;

namespace QarnotCLI;

public record CommandLineSetup(
    RootCommand RootCommand,
    ParserConfiguration ParserConfig,
    InvocationConfiguration InvocationConfig
);

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

    public CommandLineSetup Create(
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
        };
        rootCommand.AddGlobalOptions(globalOptions);

        var versionOpt = new Option<bool>("--version")
        {
            Description = "The current version",
        };
        rootCommand.SetAction(parseResult =>
        {
            if (parseResult.GetValue(versionOpt))
            {
                Console.WriteLine(assemblyDetails.ToString());
            }
            else
            {
                new HelpAction().Invoke(parseResult);
            }
        });
        rootCommand.Add(versionOpt);

        // Set up custom help on all commands
        CustomHelpLayout.ApplyCustomHelp(rootCommand, assemblyDetails);

        // Disable the default handling for `@` on the command line
        var parserConfig = new ParserConfiguration
        {
            ResponseFileTokenReplacer = (string _1, out IReadOnlyList<string>? _2, out string? _3) =>
            {
                _2 = null;
                _3 = null;
                return false;
            },
        };

        var invocationConfig = new InvocationConfiguration
        {
            ProcessTerminationTimeout = TimeSpan.FromSeconds(2),
            EnableDefaultExceptionHandler = false,
        };

        return new CommandLineSetup(rootCommand, parserConfig, invocationConfig);
    }
}
