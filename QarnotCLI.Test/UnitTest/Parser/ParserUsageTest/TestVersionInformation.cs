namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestVersionInformation
    {
        public class AssemblyAttributesTester : IPackageDetails
        {
            public string CommitHash { get => "0129abf"; }
            public string AssemblyName { get => "qarnot"; }
            public string AssemblyVersion { get => "12.43.5.54"; }
            public string FrameworkName { get => "NET.Core"; }
            public string BuildDate { get => "2020-04-29T17:22:47:234Z"; }
        }

        [Test]
        public void TestPrintHelReturnTheGoodVersionInformation()
        {
            CLILogsCheckValues.SurchargeLogs();
            var argv = new string[] { "--version" };
            using var commandLineParser = new CommandLine.Parser();
            ParserUsage helpUsage = new ParserUsage(new AssemblyAttributesTester());

            var parser = commandLineParser.ParseArguments<Options.DefaultJob>(argv);

            List<CommandLine.Error> errs = new List<CommandLine.Error>()
            {
                new CommandLineWrapError(CommandLine.ErrorType.VersionRequestedError)
            };

            try
            {
                helpUsage.PrintHelp(parser, errs, argv);
            }
            catch
            {
                string input = CLILogsCheckValues.AllMessages;
                string versionWaitted = "qarnot-12.43.5.54-0129abf-2020-04-29T17:22:47Z-NET.Core";
                Assert.AreEqual(input, versionWaitted);
                return;
            }
            throw new Exception("A version must be throw");
        }

        [Test]
        public void TestPrintHelReturnTheGoodVersionFormat()
        {
            CLILogsCheckValues.SurchargeLogs();
            using var commandLineParser = new CommandLine.Parser();
            ParserUsage helpUsage = new ParserUsage();
            var argv = new string[] { "--version" };

            var parser = commandLineParser.ParseArguments<Options.DefaultJob>(argv);

            List<CommandLine.Error> errs = new List<CommandLine.Error>()
            {
                new CommandLineWrapError(CommandLine.ErrorType.VersionRequestedError)
            };

            try
            {
                helpUsage.PrintHelp(parser, errs, argv);
            }
            catch
            {
                string input = CLILogsCheckValues.AllMessages;
                string datePattern = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z";
                string pattern = @"\w+-(\d+\.){3}\d+-[0-9a-f]{7}-" + datePattern + @"-[A-Za-z0-9\.]+";
                Console.WriteLine(input);
                Console.WriteLine(pattern);
                Assert.IsTrue(Regex.Match(input, pattern, RegexOptions.IgnoreCase).Success);
                return;
            }
            throw new Exception("A version must be throw");
        }
    }
}