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
    public class TestFormatXml
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        private class TestXML
        {
            public string Var1 { get; set; } = "test";

            public string Var2 { get; set; } = "test";
        }

        [Test]
        public void CheckBasicXmlFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("XML");
            string retString = "<Information>" + Environment.NewLine + "  <Var1>test</Var1>" + Environment.NewLine + "  <Var2>test</Var2>" + Environment.NewLine + "</Information>";
            TestXML test = new TestXML();

            Assert.AreEqual(retString, format.Format<TestXML>(test));
        }

        [Test]
        public void CheckBasicListFormatReturnTheGoodSring()
        {
            var format = FormatterFactory.CreateFormat("XML");
            string retString = "<Information>" + Environment.NewLine +
            "  <Values>"  + Environment.NewLine +
            "    <Var1>test</Var1>"  + Environment.NewLine +
            "    <Var2>test</Var2>"  + Environment.NewLine +
            "  </Values>"  + Environment.NewLine +
            "</Information>";
            List<TestXML> test = new List<TestXML>() { new TestXML() };

            Assert.AreEqual(retString, format.FormatCollection<TestXML>(test));
        }
    }
}