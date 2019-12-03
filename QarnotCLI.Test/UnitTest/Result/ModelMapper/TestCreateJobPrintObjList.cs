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
    public class TestCreateJobPrintObjList
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new JobModelMapper();
            QarnotSDK.QJob job = new QarnotSDK.QJob(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(job);
            Assert.IsTrue(returnSetting is CommandValues.JobCommandValue);
        }

        [Test]
        public void CreateObjVerifyTheValuesSet()
        {
            var test = new JobModelMapper();
            QarnotSDK.QJob job = new QarnotSDK.QJob(FakeSDKConnection.GetFakeConnection(), "name");
            var returnSetting = test.CreateObj(job) as CommandValues.JobCommandValue;
            Assert.AreEqual(returnSetting.Name, job.Name);
            Assert.AreEqual(returnSetting.Shortname, job.Shortname);
            Assert.AreEqual(returnSetting.Uuid, job.Uuid.ToString());
            Assert.AreEqual(returnSetting.State, job.State);
        }
    }
}