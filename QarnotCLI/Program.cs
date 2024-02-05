using QarnotCLI;
using System.CommandLine;
using System.CommandLine.Parsing;
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
var parser = new CommandLineBuilderFactory(useCasesFactory)
    .Create(connectionConfiguration, releasesService, topLevelLogger)
    .Build();

try
{
    var exitCode = await parser.InvokeAsync(args);
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
