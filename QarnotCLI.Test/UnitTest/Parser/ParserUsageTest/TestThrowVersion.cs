namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestParseUsageThrowVersion
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestPrintHelpThrowParsVersionException()
        {
            // https://github.com/commandlineparser/commandline/blob/master/src/CommandLine/Error.cs
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parserObj = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseVersionException ex = null;
            ParserUsage helpUsage = new ParserUsage();
            string[] argv = new string[] { "--version" };

            var parser = commandLineParser.ParseArguments<Options.DefaultJob>(argv);

            List<CommandLine.Error> errs = new List<CommandLine.Error>();

            CommandLine.Error err = new CommandLineWrapError(CommandLine.ErrorType.VersionRequestedError);
            errs.Add(err);

            ex = Assert.Throws<ParseVersionException>(() => helpUsage.PrintHelp(parser, errs));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}