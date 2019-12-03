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
    public class TestParserArgTaskCheck
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CheckTaskWithAValidArgumentsList()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", Profile = "Bob", InstanceCount = 2 };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
            config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", JobUuid = "Bob", InstanceCount = 2 };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
            config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", PoolUuid = "Bob", InstanceCount = 2 };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
            config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", Profile = "Bob", Range = "2" };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
        }

        [Test]
        public void CheckTaskInvalidMissingInstanceNodesOrRange()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", Profile = "Bob" };
            Assert.IsFalse(CommandLineParser.CheckTask(config));
        }

        [Test]
        public void CheckTaskInvalidMissingProfileOrJobOrPool()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", InstanceCount = 2 };
            Assert.IsFalse(CommandLineParser.CheckTask(config));
        }

        [Test]
        public void CheckTaskInvalidMissingName()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Profile = "Bob", InstanceCount = 2 };
            Assert.IsFalse(CommandLineParser.CheckTask(config));
        }

        [Test]
        public void CheckTaskInvalidInstanceNodesAdRangeSetTogether()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create) { Type = ConfigType.Task, Name = "bob", Profile = "Bob", Range = "2", InstanceCount = 2 };
            Assert.IsFalse(CommandLineParser.CheckTask(config));
        }

        // TODO:
        [Test]
        public void CheckTaskCheckNotCreateConfigurationIsAlwayValid()
        {
            IConfiguration config = new AccountConfiguration(ConfigType.Account, CommandApi.Info) { };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
            config = new DefaultRunConfiguration(ConfigType.Pool, CommandApi.Info) { };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
            config = new DefaultRunConfiguration(ConfigType.Pool, CommandApi.Info) { Type = ConfigType.Pool };
            Assert.IsTrue(CommandLineParser.CheckTask(config));
        }
    }
}