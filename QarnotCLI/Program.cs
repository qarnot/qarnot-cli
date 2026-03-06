using QarnotCLI;
using System.CommandLine;
using QarnotSDK;

var topLevelLogger = new Logger();
var loggerFactory = new LoggerFactory();
var useCasesFactory = new UseCasesFactory(
    new QarnotAPIFactory(),
    new FormatterFactory(),
    new StateManagerFactory(),
    loggerFactory
);

var releasesService = new ReleasesService();
if (!DeprecationDisclaimer.ShouldIgnoreDeprecation)
{
    await DeprecationDisclaimer.Display(releasesService, topLevelLogger);
}

var connectionConfiguration = new ConnectionConfigurationParser(topLevelLogger).Parse();
var setup = new CommandLineBuilderFactory(useCasesFactory)
    .Create(connectionConfiguration, releasesService, topLevelLogger);

try
{
    var parseResult = setup.RootCommand.Parse(args, setup.ParserConfig);
    var exitCode = await parseResult.InvokeAsync(setup.InvocationConfig);
    Environment.Exit(exitCode);
}
catch (QarnotApiException e)
{
    topLevelLogger.Error(e, "An error occurred while connecting to Qarnot API");
    Environment.Exit(1);
}
catch (Exception e)
{
    topLevelLogger.Error(e, "An error occured:");
    Environment.Exit(1);
}
