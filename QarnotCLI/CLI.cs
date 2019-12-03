﻿[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("QarnotCLI.Test")]

namespace QarnotCLI
{
    using System;

    /// <summary>
    /// Program Start.
    /// </summary>
    public static class CLI
    {
        /// <summary>
        /// Main function, launch the program and return.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the program.</param>
        /// <returns>0 if success or exit if fail.</returns>
        public static int Main(string[] args)
        {
            int returnValue = 0;

            try
            {
                CLI.StartCli(args);
            }
            catch (NotImplementedException ex)
            {
                CLILogs.Error("NotImplementedException found");
                CLILogs.Error(ex.ToString());
                returnValue = 1;
            }
            catch (ParseVersionException)
            {
                CLILogs.Debug("version ask");
            }
            catch (ParseHelpException)
            {
                CLILogs.Debug("help ask");
            }
            catch (ParseException)
            {
                CLILogs.Debug("Parse error found");
                returnValue = 1;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                CLILogs.Error(ex.Message);
                returnValue = 1;
            }
            catch (CommandManager.ErrorPrintException)
            {
                CLILogs.Debug("Error catch in the launcher");
                returnValue = 1;
            }

            return returnValue;
        }

        /// <summary>
        /// get the configuration from the arguments
        /// and launch the command.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        private static void StartCli(string[] argv)
        {
            var deprecationDisclaimer = new DeprecationDisclaimer(
                new ReleasesHandler(),
                Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Warning, true),
                new EnvironmentVariableReader());
            deprecationDisclaimer.CheckReleaseDeprecations().Wait();

            var parser = CreateCommandLineParser();
            IConfiguration configAndcommandValues = parser.Parse(argv);
            PrintInfoDebug.IConfiguration(configAndcommandValues);

            IApiDataManager apiDataManager = CreateApiDataManager();

            if (apiDataManager.Start(configAndcommandValues))
            {
                ICommandManager commandManager = CreateCommandManager(configAndcommandValues.ResultFormat);
                commandManager.Start(configAndcommandValues);
            }
        }

        private static ICommandManager CreateCommandManager(string format)
        {
            return new CommandManager(
                new LauncherFactory(FormatterFactory.CreateFormat(format), new ConnectionWrapper()),
                Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Result, false));
        }

        private static IApiDataManager CreateApiDataManager()
        {
            var getEnvPath = new ConfigurationFileGetter();
            var getFileInformation = new ConfigurationFileReader();

            return new ApiDataManager(
                new ApiConnectionConfigurationRetriever(getEnvPath, new EnvironmentVariableReader(), getFileInformation),
                new ApiConnectionConfigurationWritter(getEnvPath, getFileInformation));
        }

        private static IArgumentsParser CreateCommandLineParser()
        {
            return new CommandLineParser(
                new OptionConverter(new JsonDeserializer()),
                new CommandLine.Parser(with =>
                {
                    with.HelpWriter = null;
                    with.EnableDashDash = true;
                }),
                new ParserUsage(),
                new VerbFormater());
        }
    }
}
