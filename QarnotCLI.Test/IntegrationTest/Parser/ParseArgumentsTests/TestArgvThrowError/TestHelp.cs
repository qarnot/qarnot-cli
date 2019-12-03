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
    public class TestCheckHelpAndReturnParseHelpException
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void HelpFlagParsArgReturnParseHelpException()
        {
            string[] argv = new string[] { "--help" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            var ex = Assert.Throws<QarnotCLI.ParseHelpException>(() => parser.Parse(argv));

            // check usage
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolArgvTestHelpFlagParsArgReturnParseHelpException()
        {
            string[] argv = new string[] { "pool", "create", "--help" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            var ex = Assert.Throws<QarnotCLI.ParseHelpException>(() => parser.Parse(argv));

            // check usage
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}