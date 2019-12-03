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
    public class TestConvertOptionConfigReadFile
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CheckReadConfigurationFile()
        {
            OptionConverter convert = new OptionConverter(new UnitTestJsonDeserializer());
            string fileName = "fileName";
            int verboseLevel = 5;
            bool verbose = true;
            bool quiet = true;
            string line = "{}";
            UnitTestJsonDeserializer.GetFileGet = fileName;
            UnitTestJsonDeserializer.GetFileRetrun = line;
            UnitTestJsonDeserializer.DeserializeGet = line;
            Console.WriteLine(line);
            AccountConfiguration readFile = convert.ReadConfigurationFile<AccountConfiguration>(fileName, ConfigType.Account, CommandApi.Info);
            Assert.AreEqual(readFile.Type, ConfigType.Account);
            Assert.AreEqual(readFile.Command, CommandApi.Info);
        }

        [Test]
        public void CheckConfigReadFile()
        {
            OptionConverter convert = new OptionConverter(new UnitTestJsonDeserializer());
            string fileName = "fileName";
            int verboseLevel = 5;
            bool verbose = true;
            bool quiet = true;
            string line = "{\"VerboseLevel\" : " + verboseLevel + ", \"Verbose\" : " + verbose.ToString().ToLower() + ", \"Quiet\" : " + quiet.ToString().ToLower() + "}";
            UnitTestJsonDeserializer.GetFileGet = fileName;
            UnitTestJsonDeserializer.GetFileRetrun = line;
            UnitTestJsonDeserializer.DeserializeGet = line;
            Console.WriteLine(line);
            Options.AccountOptions readFile = convert.ConfigReadFile<Options.AccountOptions>(fileName);
            Assert.AreEqual(readFile.Verbose, verbose);
            Assert.AreEqual(readFile.Quiet, quiet);
        }
    }
}