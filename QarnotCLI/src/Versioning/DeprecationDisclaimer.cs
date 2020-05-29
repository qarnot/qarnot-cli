namespace QarnotCLI
{
    using System;
    using System.Threading.Tasks;

    public class DeprecationDisclaimer
    {
        private const string DeprecationEnvironmentVariableName = "QARNOT_IGNORE_DEPRECATION";
        private readonly ReleasesHandler Handler;
        private readonly IPrinter printer;
        private readonly IEnvironmentVariableReader EnvironmentReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeprecationDisclaimer"/> class.
        /// </summary>
        /// <param name="handler">release handler that retrieve the actual releases. </param>
        /// <param name="printer">IPrinter to print the warnings. </param>
        /// <param name="envReader">environment variable to check for deprecation override. </param>
        public DeprecationDisclaimer(ReleasesHandler handler, IPrinter printer, IEnvironmentVariableReader envReader)
        {
            Handler = handler;
            EnvironmentReader = envReader;
        }

        private bool ShouldIgnoreDeprecation
        {
            get => EnvironmentReader.GetEnvironmentVariableBoolOrElse(DeprecationEnvironmentVariableName, false);
        }

        /// <summary>
        /// Check if the actual cli is deprecated or if a new version has been released.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CheckReleaseDeprecationsAsync()
        {
            try
            {
                if (!ShouldIgnoreDeprecation)
                {
                    var releaseLogger = Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Warning, true);
                    var isDeprecated = await Handler.IsActualReleaseDeprecated();
                    if (isDeprecated)
                    {
                        await releaseLogger.PrintAsync("This release version has been deprecated! Please update as soon as possible as we don't support this version anymore.");
                    }

                    var newReleaseExists = await Handler.DoesANewReleaseExists();
                    if (newReleaseExists)
                    {
                        await releaseLogger.PrintAsync("A release version exists! please upgrade the qarnot CLI to its last version to enjoy the last features.");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
