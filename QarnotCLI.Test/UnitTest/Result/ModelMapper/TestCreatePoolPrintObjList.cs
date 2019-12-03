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
    public class TestCreatePoolPrintObjList
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new PoolModelMapper();
            QarnotSDK.QPool pool = new QarnotSDK.QPool(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(pool);
            Assert.IsTrue(returnSetting is CommandValues.PoolCommandValue);
        }

        [Test]
        public void CreateObjVerifyTheValuesSet()
        {
            var test = new PoolModelMapper();
            QarnotSDK.QPool pool = new QarnotSDK.QPool(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(pool) as CommandValues.PoolCommandValue;
            Assert.AreEqual(returnSetting.Name, pool.Name);
            Assert.AreEqual(returnSetting.Shortname, pool.Shortname);
            Assert.AreEqual(returnSetting.Uuid, pool.Uuid.ToString());
            Assert.AreEqual(returnSetting.Profile, pool.Profile);
            Assert.AreEqual(returnSetting.State, pool.State);
            Assert.AreEqual(returnSetting.NodeCount, pool.NodeCount);
        }
    }
}