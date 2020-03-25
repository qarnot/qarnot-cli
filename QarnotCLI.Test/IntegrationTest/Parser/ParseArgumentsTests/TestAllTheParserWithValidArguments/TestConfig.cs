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
    public class TestParseArgValidConfig
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void ConfigOptionsCheckTestParsArg()
        {
            string token = "TOKEN";
            string uri = "URI";
            string storage = "STORAGE";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            LocalSetUpConfiguration confset = null;

            argv = new string[] { "config", "--global", "--token", token, "--api-uri", uri, "--storage-uri", storage };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is LocalSetUpConfiguration))
            {
                throw new Exception("return value is not LocalSetUpConfiguration ");
            }

            confset = (LocalSetUpConfiguration)iConfSet;
            Assert.AreEqual(confset.GlobalPath, true);
            Assert.AreEqual(confset.ApiConnection.Token, token);
            Assert.AreEqual(confset.ApiConnection.ApiUri, uri);
            Assert.AreEqual(confset.ApiConnection.StorageUri, storage);

            argv = new string[] { "config", "-g", "-t", token, "-u", uri, "-s", storage };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is LocalSetUpConfiguration))
            {
                throw new Exception("return value is not LocalSetUpConfiguration ");
            }

            confset = (LocalSetUpConfiguration)iConfSet;
            Assert.AreEqual(confset.GlobalPath, true);
            Assert.AreEqual(confset.ApiConnection.Token, token);
            Assert.AreEqual(confset.ApiConnection.ApiUri, uri);
            Assert.AreEqual(confset.ApiConnection.StorageUri, storage);
            commandLineParser.Dispose();
        }
    }
}