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
    public class TestParseArgCheckJob
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CheckJobValidArgumentsList()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Job, CommandApi.Create) { Type = ConfigType.Job, Name = "bob" };
            Assert.IsTrue(CommandLineParser.CheckJob(config));
        }

        [Test]
        public void CheckJobInvalidMissingName()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Job, CommandApi.Create) { Type = ConfigType.Job };
            Assert.IsFalse(CommandLineParser.CheckJob(config));
        }

        [Test]
        public void CheckJobAlwayValid()
        {
            IConfiguration config = new AccountConfiguration(ConfigType.Account, CommandApi.Info) { };
            Assert.IsTrue(CommandLineParser.CheckJob(config));
            config = new DefaultRunConfiguration(ConfigType.Job, CommandApi.List) { };
            Assert.IsTrue(CommandLineParser.CheckJob(config));
            config = new DefaultRunConfiguration(ConfigType.Job, CommandApi.List) { Type = ConfigType.Task };
            Assert.IsTrue(CommandLineParser.CheckJob(config));
        }
    }
}