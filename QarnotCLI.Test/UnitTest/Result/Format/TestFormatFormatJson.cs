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
            string retString = "{" + Environment.NewLine + "  \"Var1\": \"test\"," + Environment.NewLine + "  \"Var2\": \"test\"" + Environment.NewLine + "}";
            TestJson test = new TestJson();

            Assert.AreEqual(retString, format.Format<TestJson>(test));
        }

        [Test]
        public void CheckBasicJsonFormatListReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("TABLE");
            string retString = "[" + Environment.NewLine + "  {" + Environment.NewLine + "    \"Var1\": \"test\"," + Environment.NewLine + "    \"Var2\": \"test\"" + Environment.NewLine + "  }" + Environment.NewLine + "]";
            List<TestJson> test = new List<TestJson>() { new TestJson() };

            Assert.AreEqual(retString, format.FormatCollection<TestJson>(test));
        }
    }
}