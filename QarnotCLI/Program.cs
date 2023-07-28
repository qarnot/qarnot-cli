using QarnotCLI;
using System.CommandLine;
using System.CommandLine.Parsing;
using QarnotSDK;

var topLevelLogger = new Logger();
var loggerFactory = new LoggerFactory();
var useCasesFactory = new UseCasesFactory(
    new QarnotAPIFactory(),
    new FormatterFactory(),
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
    await parser.InvokeAsync(args);
}
catch (QarnotApiException e)
{
    topLevelLogger.Error(e, "An error occurred while connection to Qarnot API");
    Environment.Exit(1);
}
catch (Exception e)
{
    topLevelLogger.Error(e, "An error occured:");
    Environment.Exit(1);
}
