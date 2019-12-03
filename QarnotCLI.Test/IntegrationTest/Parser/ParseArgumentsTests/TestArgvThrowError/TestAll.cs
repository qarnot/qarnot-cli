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
    public class TestAllWrongArguments
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void ConfigOptionsAllCanOnlHaveOneRuleParsArg()
        {
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseException ex = null;

            argv = new string[] { "all", "-lad" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "all", "--list --abort --delete" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}