using Moq;
using System.CommandLine;

namespace QarnotCLI.Test;

/// <summary>
/// Wraps a <see cref="CommandLineSetup"/> to provide a test-friendly
/// Parse + Invoke workflow equivalent to the old Parser type.
/// </summary>
public class TestParser
{
    private readonly CommandLineSetup Setup;

    public TestParser(CommandLineSetup setup)
    {
        Setup = setup;

        // Override exception handling: swallow exceptions and return exit code 1
        // to reduce noise when testing inputs that intentionally fail parsing.
        Setup = setup with
        {
            InvocationConfig = new InvocationConfiguration
            {
                ProcessTerminationTimeout = setup.InvocationConfig.ProcessTerminationTimeout,
                EnableDefaultExceptionHandler = false,
            },
        };
    }

    public RootCommand RootCommand => (RootCommand)Setup.RootCommand;

    public async Task<int> InvokeAsync(string[] args)
    {
        var parseResult = Setup.RootCommand.Parse(args, Setup.ParserConfig);
        try
        {
            return await parseResult.InvokeAsync(Setup.InvocationConfig, CancellationToken.None);
        }
        catch
        {
            return 1;
        }
    }

    public async Task<int> InvokeAsync(string commandLine)
    {
        var parseResult = Setup.RootCommand.Parse(commandLine, Setup.ParserConfig);
        try
        {
            return await parseResult.InvokeAsync(Setup.InvocationConfig, CancellationToken.None);
        }
        catch
        {
            return 1;
        }
    }
}

public class MockParser
{
    public TestParser Parser { get; }

    public Mock<IAllUseCases> AllUseCases { get; }
    public Mock<IAccountUseCases> AccountUseCases { get; }
    public Mock<IBucketUseCases> BucketUseCases { get; }
    public Mock<IConfigUseCases> ConfigUseCases { get; }
    public Mock<ITaskUseCases> TaskUseCases { get; }
    public Mock<IPoolUseCases> PoolUseCases { get; }
    public Mock<IHardwareConstraintsUseCases> HardwareConstraintsUseCase { get; }
    public Mock<IJobUseCases> JobUseCases { get; }
    public Mock<ISecretsUseCases> SecretsUseCases { get; }

    public MockParser()
    {
        var globalOptions = new GlobalOptions(new());
        var releasesService = new ReleasesService();

        AllUseCases = new Mock<IAllUseCases>();
        AccountUseCases = new Mock<IAccountUseCases>();
        BucketUseCases = new Mock<IBucketUseCases>();
        ConfigUseCases = new Mock<IConfigUseCases>();
        TaskUseCases = new Mock<ITaskUseCases>();
        PoolUseCases = new Mock<IPoolUseCases>();
        HardwareConstraintsUseCase = new Mock<IHardwareConstraintsUseCases>();
        JobUseCases = new Mock<IJobUseCases>();
        SecretsUseCases = new Mock<ISecretsUseCases>();

        var setup = new CommandLineBuilderFactory(
            _ => TaskUseCases.Object,
            _ => PoolUseCases.Object,
            _ => HardwareConstraintsUseCase.Object,
            _ => JobUseCases.Object,
            _ => BucketUseCases.Object,
            _ => AllUseCases.Object,
            _ => SecretsUseCases.Object,
            _ => ConfigUseCases.Object,
            _ => AccountUseCases.Object
        ).Create(
            new(), releasesService, new Logger()
        );

        Parser = new TestParser(setup);
    }
}
