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
    public class TestParseUsageThrowVerbException
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestPrintHelpThrowParsExceptionIfBadVerb()
        {
            // https://github.com/commandlineparser/commandline/blob/master/src/CommandLine/Error.cs
            var commandLineParser = new CommandLine.Parser();
            var parser = commandLineParser.ParseArguments<Options.DefaultJob>(new string[] { });
            CommandLineParser parserObj = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParserUsage helpUsage = new ParserUsage();

            List<CommandLine.Error> errs = new List<CommandLine.Error>();

            CommandLine.Error err = new CommandLineWrapError(CommandLine.ErrorType.BadVerbSelectedError);
            errs.Add(err);

            ParseException ex = Assert.Throws<ParseException>(() => helpUsage.PrintHelp(parser, errs));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}