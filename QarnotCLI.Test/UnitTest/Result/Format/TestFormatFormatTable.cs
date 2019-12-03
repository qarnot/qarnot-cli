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
            string retString = "{\n  \"Var1\": \"test\",\n  \"Var2\": \"test\"\n}";
            TestTable test = new TestTable();

            Assert.AreEqual(retString, format.Format<TestTable>(test));
        }

        [Test]
        public void CheckBasicTableFormatListMustBeJson()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "[\n  {\n    \"Var1\": \"test\",\n    \"Var2\": \"test\"\n  }\n]";
            List<TestTable> test = new List<TestTable>() { new TestTable() };

            Assert.AreEqual(retString, format.FormatCollection<TestTable>(test));
        }

        [Test]
        public void CheckICommandValueTableFormatSimpleReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test\nVar2 : test\n";
            TestTableCommandValues test = new TestTableCommandValues();

            Assert.AreEqual(retString, format.Format<TestTableCommandValues>(test));
        }

        [Test]
        public void CheckICommandValueTableListFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test\nVar2 : \n    * test1\n    * test2\n";
            TestTableCommandValuesList test = new TestTableCommandValuesList();

            Assert.AreEqual(retString, format.Format<TestTableCommandValuesList>(test));
        }

        [Test]
        public void CheckICommandValueTableDictionaryFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "Var1 : test\nVar2 : \n    key1 : value1\n    key2 : value2\n";
            TestTableCommandValuesDictionary test = new TestTableCommandValuesDictionary();

            Assert.AreEqual(retString, format.Format<TestTableCommandValuesDictionary>(test));
        }

        [Test]
        public void CheckICommandValueTableFormatListReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = " --------------- \n | Var1 | Var2 |\n --------------- \n | test | test |\n --------------- \n | test | test |\n --------------- \n\n Count: 2";
            List<TestTableCommandValues> test = new List<TestTableCommandValues>() { new TestTableCommandValues(), new TestTableCommandValues() };

            Assert.AreEqual(retString, format.FormatCollection<TestTableCommandValues>(test));
        }
    }
}