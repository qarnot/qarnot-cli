namespace QarnotCLI.Test
{
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestGlobalFlagsAreParsed
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestHumanReadableFlagIsParsedAndSetInConfig()
        {
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());

            var config = parser.Parse(new string[] { "account" });
            Assert.IsFalse(config.HumanReadable);
            config = parser.Parse(new string[] { "account", "-h" });
            Assert.IsTrue(config.HumanReadable);
            config = parser.Parse(new string[] { "account", "--human-readable" });
            Assert.IsTrue(config.HumanReadable);

            commandLineParser.Dispose();
        }
    }
}