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
        public void CheckBasicJsonFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("JSON");
            string retString = "{\n  \"Var1\": \"test\",\n  \"Var2\": \"test\"\n}";
            TestJson test = new TestJson();

            Assert.AreEqual(retString, format.Format<TestJson>(test));
        }

        [Test]
        public void CheckBasicJsonFormatListReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "[\n  {\n    \"Var1\": \"test\",\n    \"Var2\": \"test\"\n  }\n]";
            List<TestJson> test = new List<TestJson>() { new TestJson() };

            Assert.AreEqual(retString, format.FormatCollection<TestJson>(test));
        }
    }
}