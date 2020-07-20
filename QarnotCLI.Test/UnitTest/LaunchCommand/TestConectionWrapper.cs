namespace QarnotCLI.Test
{
    using System;
    using NUnit.Framework;
    using QarnotCLI;
    using QarnotSDK;
    using Moq;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestConnectionWrapper
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestConnectionWrapperStoredNull()
        {
            var wrapper = new ConnectionWrapper();
            var ex = Assert.Throws<System.ArgumentException>(() => wrapper.CreateConnection(null));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void TestConnectionWrapperCheckValuesAre()
        {
            var wrapper = new ConnectionWrapper();
            Mock<IConfiguration> mockAStorage = new Mock<IConfiguration>();
            APIConnectionInformation info = new APIConnectionInformation();
            info.Token = "Token";
            info.ApiUri = "https://apiuri.com";
            info.StorageUri = "https://storageuri.com";
            info.AccountEmail = "accountemail@test.com";
            info.ForcePathStyle = true;
            mockAStorage.Setup(foo => foo.ApiConnection).Returns(info);

            IConfiguration config = mockAStorage.Object;
            QarnotSDK.Connection connection = wrapper.CreateConnection(config);

            Assert.AreEqual("Token", connection.Token);
            Assert.AreEqual(new Uri("https://apiuri.com/"), connection.Uri);
            Assert.AreEqual(new Uri("https://storageuri.com/"), connection.StorageUri);
            Assert.AreEqual(true, connection.ForceStoragePathStyle);
            Assert.AreEqual("accountemail@test.com", connection.StorageAccessKey);
        }
    }
}