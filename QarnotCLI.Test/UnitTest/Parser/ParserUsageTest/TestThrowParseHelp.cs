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
    public class TestParseUsageThrowHelp
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            CmdLineParser = new CommandLine.Parser();
            string[] argv = new string[] { };
            CommandLineParser parserObj = new CommandLineParser(new OptionConverter(new JsonDeserializer()), CmdLineParser, new ParserUsage(), new VerbFormater());
            Parser = CmdLineParser.ParseArguments<Options.DefaultJob>(argv);
        }

        [TearDown]
        public void TearDown()
        {
            CmdLineParser.Dispose();
        }

        private CommandLine.Parser CmdLineParser { get; set; }

        private CommandLine.ParserResult<QarnotCLI.Options.DefaultJob> Parser { get; set; }

        [Test]
        public void UsageThrowHelpRequestException()
        {
            ParserUsage helpUsage = new ParserUsage();
            List<CommandLine.Error> errs = new List<CommandLine.Error>()
            {
                // https://github.com/commandlineparser/commandline/blob/master/src/CommandLine/Error.cs
                new CommandLineWrapError(CommandLine.ErrorType.HelpRequestedError),
            };
            ParseHelpException ex = Assert.Throws<ParseHelpException>(() => helpUsage.PrintHelp(Parser, errs));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void UsageThrowVerbHelpException()
        {
            ParserUsage helpUsage = new ParserUsage();
            List<CommandLine.Error> errs = new List<CommandLine.Error>()
            {
                new CommandLineWrapError(CommandLine.ErrorType.HelpVerbRequestedError),
            };
            ParseHelpException ex = Assert.Throws<ParseHelpException>(() => helpUsage.PrintHelp(Parser, errs));
            Assert.IsNotNull(ex);
        }
    }
}