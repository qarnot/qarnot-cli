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
    public class TestPoolWrongArguments
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreatePoolArgvTestNoVerbParsArg()
        {
            string[] argv = new string[] { };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            var ex = Assert.Throws<ParseException>(() => parser.Parse(argv));

            // check usage
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolArgvTestInvalidFlagParsArg()
        {
            string[] argv = new string[] { "pool", "create", "--bob", "bobby" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            var ex = Assert.Throws<ParseException>(() => parser.Parse(argv));

            // check usage
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolMissArgTestParsArg()
        {
            string name = "NAME";
            string instance = "42";
            string profile = "PROFILE";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseException ex = null;

            argv = new string[] { "pool", "create", "--name", name, "--instanceNodes", instance };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "--name", name, "--profile", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "--instanceNodes", instance, "--profile", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "-n", name, "-i", instance };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "-n", name, "-p", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "-i", instance, "-p", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolArgNeededTestParsArg()
        {
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseException ex = null;
            string[] argv = null;
            argv = new string[] { "pool", "create", "-p", "profile", "-i", "5" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "--profile", "profile", "--instanceNodes", "5" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "-n", "name", "-i", "5" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "--name", "name", "--instanceNodes", "5" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "-n", "name", "-p", "profile" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "pool", "create", "--name", "name", "--profile", "profile" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}