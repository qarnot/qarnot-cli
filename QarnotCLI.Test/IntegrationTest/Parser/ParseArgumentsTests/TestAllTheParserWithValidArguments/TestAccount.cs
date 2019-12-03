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
    public class TestParseArgValidAccount
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void AccountConfigurationCheckTestParsArg()
        {
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            AccountConfiguration confset = null;

            argv = new string[] { "account" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is AccountConfiguration))
            {
                throw new Exception("return value is not AccountConfiguration ");
            }

            confset = (AccountConfiguration)iConfSet;

            commandLineParser.Dispose();
        }
    }
}