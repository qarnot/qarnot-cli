using Moq;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;

namespace QarnotCLI.Test;

public class MockParser
{
    public Parser Parser { get; }

    public Mock<IAllUseCases> AllUseCases { get; }
    public Mock<IAccountUseCases> AccountUseCases { get; }
    public Mock<IBucketUseCases> BucketUseCases { get; }
    public Mock<IConfigUseCases> ConfigUseCases { get; }
    public Mock<ITaskUseCases> TaskUseCases { get; }
    public Mock<IPoolUseCases> PoolUseCases { get; }
    public Mock<IJobUseCases> JobUseCases { get; }
    public Mock<ISecretsUseCases> SecretsUseCases { get; }

    public MockParser()
    {
        var globalOptions = new GlobalOptions(new());
        var releasesService = new ReleasesService();
        var assemblyDetails = releasesService.GetAssemblyDetails();

        AllUseCases = new Mock<IAllUseCases>();
        AccountUseCases = new Mock<IAccountUseCases>();
        BucketUseCases = new Mock<IBucketUseCases>();
        ConfigUseCases = new Mock<IConfigUseCases>();
        TaskUseCases = new Mock<ITaskUseCases>();
        PoolUseCases = new Mock<IPoolUseCases>();
        JobUseCases = new Mock<IJobUseCases>();
        SecretsUseCases = new Mock<ISecretsUseCases>();

        Parser = new CommandLineBuilderFactory(
            _ => TaskUseCases.Object,
            _ => PoolUseCases.Object,
            _ => JobUseCases.Object,
            _ => BucketUseCases.Object,
            _ => AllUseCases.Object,
            _ => SecretsUseCases.Object,
            _ => ConfigUseCases.Object,
            _ => AccountUseCases.Object
        ).Create(
            new(), releasesService, new Logger()
        ).UseExceptionHandler((exc, ctx) => {
            // Limit noise for when testing inputs failing parsing (on purpose).
            ctx.ExitCode = 1;
        }).Build();
    }
}
