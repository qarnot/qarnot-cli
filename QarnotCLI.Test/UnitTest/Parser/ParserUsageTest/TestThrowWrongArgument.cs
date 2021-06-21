namespace QarnotCLI.Test
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using CommandLine;


    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestParseUsageThrowArgumentException
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void PoolWithTagAndTagIntersectThrowAnExcption()
        {
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parserObj = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            string[] argv = new string[] { "pool info", "--tags", "t1", "t2", "--exclusive-tags", "t3", "t4" };
            ParseException ex = null;
            var converter = new OptionConverter(null);
            var usage = new ParserUsage();

            var parser = commandLineParser.ParseArguments<Options.InfoPoolOptions>(argv);
            ex = Assert.Throws<ParseException>(() => parser.MapResult(
                (Options.InfoPoolOptions o) => converter.ConvertGenericGetterOption(ConfigType.Pool, CommandApi.Info, o),
                err => throw new ParseException(usage.PrintHelp(parser, err, argv))));

            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void TaskWithTagAndTagIntersectThrowAnExcption()
        {
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parserObj = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            string[] argv = new string[] { "task info", "--tags", "t1", "t2", "--exclusive-tags", "t3", "t4" };
            ParseException ex = null;
            var converter = new OptionConverter(null);
            var usage = new ParserUsage();

            var parser = commandLineParser.ParseArguments<Options.InfoTaskOptions>(argv);
            ex = Assert.Throws<ParseException>(() => parser.MapResult(
                (Options.InfoTaskOptions o) => converter.ConvertGenericGetterOption(ConfigType.Task, CommandApi.Info, o),
                err => throw new ParseException(usage.PrintHelp(parser, err, argv))));

            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}