namespace QarnotCLI
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class ParserUsage
    {
#if (DEBUG)
        private const int maxSize = 1200;
#else
        private const int maxSize = 120;
#endif
        public ParserUsage(IPackageDetails assemblyAttributes = null)
        {
            AssemblyAttributes = assemblyAttributes == null ? new PackageDetails() : assemblyAttributes;
        }

        private IPackageDetails AssemblyAttributes { get; }

        private string PrintHelpErrorUsage<T>(CommandLine.ParserResult<T> parser)
        {
            return CommandLine.Text.HelpText.AutoBuild(
                parser,
                h =>
                {
                    h.AddDashesToOption = true;
                    h.AddEnumValuesToHelpText = true;
                    return CommandLine.Text.HelpText.DefaultParsingErrorsHandler(parser, h);
                },
                e => e,
                false,
                ParserUsage.maxSize);
        }

        private string PrintHelpErrorVerbUsage<T>(CommandLine.ParserResult<T> parser)
        {
            return CommandLine.Text.HelpText.AutoBuild(parser, h => { return CommandLine.Text.HelpText.DefaultParsingErrorsHandler(parser, h); }, e => e, true, ParserUsage.maxSize);
        }

        private string PrintHelpUsage<T>(CommandLine.ParserResult<T> parser)
        {
            string help = CommandLine.Text.HelpText.AutoBuild(parser, h =>
                {
                    h.Heading = PrintHelpVersion(parser);
                    return CommandLine.Text.HelpText.DefaultParsingErrorsHandler(parser, h);
                },
                e => e, false, ParserUsage.maxSize);

            return help;
        }

        private string PrintHelpVerbUsage<T>(CommandLine.ParserResult<T> parser)
        {
            string help = CommandLine.Text.HelpText.AutoBuild(parser, h =>
                {
                    h.Heading = PrintHelpVersion(parser);
                    return CommandLine.Text.HelpText.DefaultParsingErrorsHandler(parser, h);
                },
                e => e, true, ParserUsage.maxSize);

            return help;
        }

        private string PrintHelpVersion<T>(CommandLine.ParserResult<T> parser)
        {
            string assemblyName = AssemblyAttributes.AssemblyName;
            string assemblyVersion = AssemblyAttributes.AssemblyVersion;
            string gitHash = AssemblyAttributes.CommitHash;
            string frameWorkBuilder = AssemblyAttributes.FrameworkName.Replace(" ", String.Empty);
            string date = AssemblyAttributes.BuildDate;
            // Remove milliseconds from the timestamp
            date = date.Substring(0, date.Length - 5) + "Z";

            return String.Format("{0}-{1}-{2}-{3}-{4}", assemblyName, assemblyVersion, gitHash, date, frameWorkBuilder);
        }

        /// <summary>
        /// Print the CommandLine help.
        /// </summary>
        /// <param name="parser">Parser used (CommandLine).</param>
        /// <param name="errs">Error list.</param>
        /// <param name="argv">argv list.</param>
        /// <typeparam name="T">The generic type parameter: Object to parser to get the usage.</typeparam>
        /// <returns>String usage.</returns>
        public string PrintHelp<T>(CommandLine.ParserResult<T> parser, System.Collections.Generic.IEnumerable<CommandLine.Error> errs, string[] argv)
        {
            // create logs if not already created
            CLILogs.CreateLoggers();

            string helpText = PrintHelpErrorUsage(parser);
            ParseException ex = new ParseException();

            // check if a "help" flag is used
            if (argv.Length > 1 && argv[1] == "help")
            {
                CLILogs.Usage(PrintHelpUsage(parser));
                throw new ParseHelpException();
            }

            if (errs == null)
            {
                CLILogs.Usage(helpText);
                throw ex;
            }

            if (errs.Any(x => x.Tag == CommandLine.ErrorType.VersionRequestedError))
            {
                helpText = PrintHelpVersion(parser);
                ex = new ParseVersionException();
            }
            else if (errs.Any(x => x.Tag == CommandLine.ErrorType.HelpRequestedError))
            {
                helpText = PrintHelpUsage(parser);
                ex = new ParseHelpException();
            }
            else if (errs.Any(x => x.Tag == CommandLine.ErrorType.HelpVerbRequestedError))
            {
                helpText = PrintHelpVerbUsage(parser);
                ex = new ParseHelpException();
            }
            else if (errs.Any(x => x.Tag == CommandLine.ErrorType.NoVerbSelectedError ||
                    x.Tag == CommandLine.ErrorType.BadVerbSelectedError))
            {
                helpText = PrintHelpErrorVerbUsage(parser);
                string errorList = string.Empty;
                errs.ToList().ForEach(e => errorList += e.ToString());
                ex = new ParseException(errorList);
                CLILogs.Debug("Parsing error : " + errorList);
            }
            else
            {
                string errorList = string.Empty;
                errs.ToList().ForEach(e => errorList += e.ToString());
                ex = new ParseException(errorList);
                CLILogs.Debug("Parsing error : " + errorList);
            }

            CLILogs.Usage(helpText);
            throw ex;
        }
    }
}