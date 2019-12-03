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
    public class TestCreateTaskPrintObjList
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new TaskModelMapper();
            QarnotSDK.QTask task = new QarnotSDK.QTask(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(task);
            Assert.IsTrue(returnSetting is CommandValues.TaskCommandValue);
        }

        [Test]
        public void CreateObjVerifyTheValuesSet()
        {
            var test = new TaskModelMapper();
            QarnotSDK.QTask task = new QarnotSDK.QTask(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(task) as CommandValues.TaskCommandValue;
            Assert.AreEqual(returnSetting.Name, task.Name);
            Assert.AreEqual(returnSetting.Shortname, task.Shortname);
            Assert.AreEqual(returnSetting.Uuid, task.Uuid.ToString());
            Assert.AreEqual(returnSetting.Profile, task.Profile);
            Assert.AreEqual(returnSetting.State, task.State);
            Assert.AreEqual(returnSetting.InstanceCount, task.InstanceCount);
        }
    }
}