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
    public class TestCreateMessagePrintObjList
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new GenericInfoModelMapper();
            var tuple = ("uuid", "message");
            var returnSetting = test.CreateObj(tuple);
            Assert.IsTrue(returnSetting is CommandValues.GenericInfoCommandValue);
        }

        [Test]
        public void CreateObjVerifyTheValuesSet()
        {
            var test = new GenericInfoModelMapper();
            string message = "message";
            string uuid = "uuid";
            var returnSetting = test.CreateObj((uuid, message)) as CommandValues.GenericInfoCommandValue;
            Assert.AreEqual(returnSetting.Message, message);
            Assert.AreEqual(returnSetting.Uuid, uuid);
        }
    }
}