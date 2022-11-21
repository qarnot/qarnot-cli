namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;
    using System.Reflection;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestFormatJson
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        private class TestJson
        {
            public string Var1 { get; set; } = "test";

            public string Var2 { get; set; } = "test";
        }

        [Test]
        public void CheckBasicJsonFormatReturnTheGoodString()
        {
            var format = FormatterFactory.CreateFormat("JSON");
            string retString = "{" + Environment.NewLine + "  \"Var1\": \"test\"," + Environment.NewLine + "  \"Var2\": \"test\"" + Environment.NewLine + "}";
            TestJson test = new TestJson();

            Assert.AreEqual(retString, format.Format<TestJson>(test));
        }

        [Test]
        public void CheckBasicJsonFormatListReturnTheGoodString()
        {
            var format = FormatterFactory.CreateFormat("JSON");
            string retString = "[" + Environment.NewLine + "  {" + Environment.NewLine + "    \"Var1\": \"test\"," + Environment.NewLine + "    \"Var2\": \"test\"" + Environment.NewLine + "  }" + Environment.NewLine + "]";
            List<TestJson> test = new List<TestJson>() { new TestJson() };

            Assert.AreEqual(retString, format.FormatCollection<TestJson>(test));
        }

        private static TestCaseData[] ObjectsWithConnection =
        {
            new TestCaseData(
                new QarnotSDK.QTask(new("someToken"), "ConnectionOutputTaskTest")
            ),
            new TestCaseData(
                new QarnotSDK.QPool(new("someToken"), "ConnectionOutputPoolTest")
            ),
            new TestCaseData(
                new QarnotSDK.QJob(new("someToken"), "ConnectionOutputJobTest")
            ),
            new TestCaseData(
                new FakeTask(new("someToken"), "ConnectionOutputTaskInPoolTest", new QarnotSDK.QPool(new("someToken"), Guid.NewGuid()))
            )
        };

        [TestCaseSource(nameof(ObjectsWithConnection))]
        public void CheckJsonFormatHidesConnectionInfo(object objectWithConnection)
        {
            var format = FormatterFactory.CreateFormat("JSON");
            var connection = (QarnotSDK.Connection) objectWithConnection
                .GetType()
                .GetProperty("Connection")
                .GetGetMethod(nonPublic: true)
                .Invoke(objectWithConnection, null);
            var name = (string) objectWithConnection
                .GetType()
                .GetProperty("Name")
                .GetGetMethod(nonPublic: true)
                .Invoke(objectWithConnection, null);
            var formattedObject = format.Format(objectWithConnection);
            //TestContext.Progress.WriteLine(formattedObject);
            Assert.IsNotNull(formattedObject);
            StringAssert.Contains($"\"Name\": \"{name}\"", formattedObject);
            StringAssert.DoesNotContain("\"Connection\": ", formattedObject);
            StringAssert.DoesNotContain(connection.Token, formattedObject);
        }

        internal class FakeTask: QarnotSDK.QTask
        {
            private QarnotSDK.QPool _pool { get; set; }
            public override QarnotSDK.QPool Pool => _pool;
            private Guid _fakeUuid { get; set; }
            public override Guid Uuid => _fakeUuid;
            public FakeTask(QarnotSDK.Connection connection, string name, QarnotSDK.QPool pool, uint instanceCount = 0, string shortname = default(string), bool? waitForPoolResourcesSynchronization=null)
                    : base (connection, name, pool, instanceCount, shortname, waitForPoolResourcesSynchronization)
            {
                _pool = pool;
                _fakeUuid = Guid.NewGuid();
            }
        }
    }
}