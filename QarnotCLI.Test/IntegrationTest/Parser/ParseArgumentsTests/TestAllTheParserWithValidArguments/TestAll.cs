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
    public class TestParseArgValidAll
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            CommandLineParser = new CommandLine.Parser();
            Parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), CommandLineParser, new ParserUsage(), new VerbFormater());
        }

        [TearDown]
        public void TearDown()
        {
            CommandLineParser.Dispose();
        }

        private CommandLineParser Parser { get; set; }

        private CommandLine.Parser CommandLineParser { get; set; }

        [Test]
        public void AllOptionsReturnDefaultRunConfiguration()
        {
            string[] argv = new string[] { "all", "--delete" };
            IConfiguration iConfSet = Parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }
        }

        [Test]
        public void AllOptionsCheckDelete()
        {
            string[] argv = new string[] { "all", "--delete" };

            DefaultRunConfiguration confset = Parser.Parse(argv) as DefaultRunConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.Delete);

            argv = new string[] { "all", "-d" };
            confset = Parser.Parse(argv) as DefaultRunConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
        }

        [Test]
        public void AllOptionsCheckAbort()
        {
            string[] argv = new string[] { "all", "--abort" };
            DefaultRunConfiguration confset = Parser.Parse(argv) as DefaultRunConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.Abort);

            argv = new string[] { "all", "-a" };
            confset = Parser.Parse(argv) as DefaultRunConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.Abort);
        }

        [Test]
        public void AllOptionsCheckList()
        {
            string[] argv = new string[] { "all", "--list" };
            DefaultRunConfiguration confset = Parser.Parse(argv) as DefaultRunConfiguration;

            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.List);

            argv = new string[] { "all", "-l" };
            confset = Parser.Parse(argv) as DefaultRunConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.List);
        }

        [Test]
        public void AllOptionsCheckNoArg()
        {
            string[] argv = new string[] { "all" };
            DefaultRunConfiguration confset = Parser.Parse(argv) as DefaultRunConfiguration;

            Assert.AreEqual(confset.Type, ConfigType.All);
            Assert.AreEqual(confset.Command, CommandApi.List);
        }
    }
}