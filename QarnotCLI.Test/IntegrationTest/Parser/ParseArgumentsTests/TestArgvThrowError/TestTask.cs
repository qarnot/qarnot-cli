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
    public class TestTaskWrongArguments
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateTaskMissArgTestParsArg()
        {
            string name = "NAME";
            string instance = "42";
            string profile = "PROFILE";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseException ex = null;

            argv = new string[] { "task", "create", "--name", name, "--instance", instance };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "task", "create", "--name", name, "--profile", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "task", "create", "--instance", instance, "--profile", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "task", "create", "-n", name, "-i", instance };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "task", "create", "-n", name, "-p", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            argv = new string[] { "task", "create", "-i", instance, "-p", profile };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }
    }
}