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
    public class TestApiDataManager
    {
        private class FakeRetriveApiConnectionConfiguration : IApiConnectionConfigurationRetriever
        {
            public bool GetIsCall { get; set; } = false;

            public APIConnectionInformation RetrieveConfigurationInformation(APIConnectionInformation config)
            {
                GetIsCall = true;
                return config;
            }
        }

        private class FakeApiConnectionConfigurationSetter : IApiConnectionConfigurationWritter
        {
            public bool SetIsCall { get; set; } = false;

            public void SaveSettings(IConfiguration iconfig)
            {
                SetIsCall = true;
            }
        }

        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void RetrunFalseWhenConfigIsSet()
        {
            var conf = new LocalSetUpConfiguration(ConfigType.Config, CommandApi.Set);
            var get = new FakeRetriveApiConnectionConfiguration();
            var set = new FakeApiConnectionConfigurationSetter();
            IApiDataManager manager = new ApiDataManager(get, set);
            Assert.IsFalse(manager.Start(conf));
            Assert.IsFalse(get.GetIsCall);
            Assert.IsTrue(set.SetIsCall);
        }

        [Test]
        public void RetrunTrueWhenNotConfigSet()
        {
            var conf = new LocalSetUpConfiguration(ConfigType.Pool, CommandApi.Set);
            var get = new FakeRetriveApiConnectionConfiguration();
            var set = new FakeApiConnectionConfigurationSetter();
            IApiDataManager manager = new ApiDataManager(get, set);
            Assert.IsTrue(manager.Start(conf));
            Assert.IsTrue(get.GetIsCall);
            Assert.IsFalse(set.SetIsCall);
        }

        [Test]
        public void RetrunTrueWhenConfigIsNotSet()
        {
            var conf = new LocalSetUpConfiguration(ConfigType.Config, CommandApi.Create);
            var get = new FakeRetriveApiConnectionConfiguration();
            var set = new FakeApiConnectionConfigurationSetter();
            IApiDataManager manager = new ApiDataManager(get, set);
            Assert.IsTrue(manager.Start(conf));
            Assert.IsTrue(get.GetIsCall);
            Assert.IsFalse(set.SetIsCall);
        }
    }
}