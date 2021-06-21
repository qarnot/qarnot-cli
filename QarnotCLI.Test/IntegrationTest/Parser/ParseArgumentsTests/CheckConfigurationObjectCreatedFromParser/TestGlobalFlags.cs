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
            using var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());

            var config = parser.Parse(new string[] { "account" });
            Assert.IsFalse(config.HumanReadable);
            config = parser.Parse(new string[] { "account", "-h" });
            Assert.IsTrue(config.HumanReadable);
            config = parser.Parse(new string[] { "account", "--human-readable" });
            Assert.IsTrue(config.HumanReadable);
        }

        [Test]
        public void TestCustomCertificateIsParsedAndSetInConfig()
        {
            using var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());

            var config = parser.Parse(new string[] { "account" });
            Assert.IsTrue(config.CustomSslCertificate == default(string));
            config = parser.Parse(new string[] { "account", "--api-ca-certificate", "path/tests" });
            Assert.IsTrue(config.CustomSslCertificate == "path/tests");
        }

        [Test]
        public void TestNoCheckCertificateIsParsedAndSetInConfig()
        {
            using var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());

            var config = parser.Parse(new string[] { "account" });
            Assert.IsFalse(config.UnsafeSslCertificate);
            config = parser.Parse(new string[] { "account", "--unsafe-ssl" });
            Assert.IsTrue(config.UnsafeSslCertificate);
        }
    }
}