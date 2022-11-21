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
    public class TestFormatTable
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        private class TestTable
        {
            public string Var1 { get; set; } = "test";

            public string Var2 { get; set; } = "test";
        }

        private class TestTableCommandValues : ICommandValue
        {
            public string Var1 { get; set; } = "test";

            public string Var2 { get; set; } = "test";
        }

        private class TestTableCommandValuesList : ICommandValue
        {
            public string Var1 { get; set; } = "test";

            public List<string> Var2 { get; set; } = new List<string>() { "test1", "test2" };
        }

        private class TestTableCommandValuesDictionary : ICommandValue
        {
            public string Var1 { get; set; } = "test";

            public Dictionary<string, string> Var2 { get; set; } = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" } };
        }

        [Test]
        public void CheckBasicTableFormatMustBeJson()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "{" + Environment.NewLine + "  \"Var1\": \"test\"," + Environment.NewLine + "  \"Var2\": \"test\"" + Environment.NewLine + "}";
            TestTable test = new TestTable();

            Assert.AreEqual(retString, format.Format<TestTable>(test));
        }

        [Test]
        public void CheckBasicTableFormatListMustBeJson()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "[" + Environment.NewLine + "  {" + Environment.NewLine + "    \"Var1\": \"test\"," + Environment.NewLine + "    \"Var2\": \"test\"" + Environment.NewLine + "  }" + Environment.NewLine + "]";
            List<TestTable> test = new List<TestTable>() { new TestTable() };

            Assert.AreEqual(retString, format.FormatCollection<TestTable>(test));
        }

        [Test]
        public void CheckICommandValueTableFormatSimpleReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test" + Environment.NewLine + "Var2 : test" + Environment.NewLine;
            TestTableCommandValues test = new TestTableCommandValues();

            Assert.AreEqual(retString, format.Format<TestTableCommandValues>(test));
        }

        [Test]
        public void CheckICommandValueTableListFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test" + Environment.NewLine + "Var2 : " + Environment.NewLine + "    * test1" + Environment.NewLine + "    * test2" + Environment.NewLine;
            TestTableCommandValuesList test = new TestTableCommandValuesList();

            Assert.AreEqual(retString, format.Format<TestTableCommandValuesList>(test));
        }

        [Test]
        public void CheckICommandValueTableDictionaryFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test" + Environment.NewLine + "Var2 : " + Environment.NewLine + "    key1 : value1" + Environment.NewLine + "    key2 : value2" + Environment.NewLine;
            TestTableCommandValuesDictionary test = new TestTableCommandValuesDictionary();

            Assert.AreEqual(retString, format.Format<TestTableCommandValuesDictionary>(test));
        }

        [Test]
        public void CheckICommandValueTableFormatListReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = " --------------- " + Environment.NewLine + " | Var1 | Var2 |" + Environment.NewLine + " --------------- " + Environment.NewLine + " | test | test |" + Environment.NewLine + " --------------- " + Environment.NewLine + " | test | test |" + Environment.NewLine + " --------------- " + Environment.NewLine + Environment.NewLine + " Count: 2";
            List<TestTableCommandValues> test = new List<TestTableCommandValues>() { new TestTableCommandValues(), new TestTableCommandValues() };

            Assert.AreEqual(retString, format.FormatCollection<TestTableCommandValues>(test));
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
        };

        [TestCaseSource(nameof(ObjectsWithConnection))]
        public void CheckTableFormatHidesConnectionInfo(object objectWithConnection)
        {
            var format = FormatterFactory.CreateFormat("TABLE");
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
    }
}