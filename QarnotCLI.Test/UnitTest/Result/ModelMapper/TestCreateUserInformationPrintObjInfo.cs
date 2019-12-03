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
    public class TestCreateUserInformationPrintObjInfo
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateObjTestTheGoodRetrunValueType()
        {
            var test = new UserInformationModelMapper();
            QarnotSDK.UserInformation userInformation = null;
            var returnSetting = test.CreateObj(userInformation);
            Assert.IsTrue(returnSetting is CommandValues.UserInformationCommandValue);
        }
    }
}