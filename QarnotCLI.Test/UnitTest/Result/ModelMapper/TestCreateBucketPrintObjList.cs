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
    public class TestCreateBucketPrintObjList
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new BucketModelMapper(new BytesFormatter());
            QarnotSDK.QBucket bucket = new QarnotSDK.QBucket(FakeSDKConnection.GetFakeConnection(), "name", false);
            var returnSetting = test.CreateObj(bucket);
            Assert.IsTrue(returnSetting is CommandValues.BucketCommandValue);
        }

        [Test]
        public void CreateObjVerifyTheValuesSet()
        {
            var bytesFormat = new BytesFormatter();
            var test = new BucketModelMapper(new BytesFormatter());
            QarnotSDK.QBucket bucket = new QarnotSDK.QBucket(FakeSDKConnection.GetFakeConnection(), "name", false);
            var returnSetting = test.CreateObj(bucket) as CommandValues.BucketCommandValue;
            Assert.AreEqual(returnSetting.Shortname, bucket.Shortname);
            Assert.AreEqual(returnSetting.FileCount, bucket.FileCount.ToString());
            Assert.AreEqual(returnSetting.UsedSpaceBytes, bytesFormat.ConvertToHumanReadable(bucket.UsedSpaceBytes));
        }
    }
}