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
    public class TestParserArgPoolCheck
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CheckPoolCreateConfigurationValidArgumentsList()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Pool, CommandApi.Create) { Type = ConfigType.Pool, Name = "bob", Profile = "Bob", InstanceCount = 2 };
            Assert.IsTrue(CommandLineParser.CheckPool(config));
        }

        [Test]
        public void CheckPoolInvalidMissingInstanceNodes()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Pool, CommandApi.Create) { Type = ConfigType.Pool, Name = "bob", Profile = "Bob" };
            Assert.IsFalse(CommandLineParser.CheckPool(config));
        }

        [Test]
        public void CheckPoolInvalidMissingProfile()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Pool, CommandApi.Create) { Type = ConfigType.Pool, Name = "bob", InstanceCount = 2 };
            Assert.IsFalse(CommandLineParser.CheckPool(config));
        }

        [Test]
        public void CheckPoolInvalidMissingName()
        {
            IConfiguration config = new CreateConfiguration(ConfigType.Pool, CommandApi.Create) { Type = ConfigType.Pool, Profile = "bob", InstanceCount = 2 };
            Assert.IsFalse(CommandLineParser.CheckPool(config));
        }

        [Test]
        public void CheckPoolAlwayValid()
        {
            IConfiguration config = new AccountConfiguration(ConfigType.Account, CommandApi.Info) { };
            Assert.IsTrue(CommandLineParser.CheckPool(config));
            config = new DefaultRunConfiguration(ConfigType.Pool, CommandApi.List) { };
            Assert.IsTrue(CommandLineParser.CheckPool(config));
            config = new DefaultRunConfiguration(ConfigType.Pool, CommandApi.List) { Type = ConfigType.Task };
            Assert.IsTrue(CommandLineParser.CheckPool(config));
        }
    }
}