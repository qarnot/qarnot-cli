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
    public class TestCreateTools
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestCreateConstants()
        {
            List<string> constant = new List<string>()
            {
                "constant1=value1=2",
                "constant2=value3",
            };
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstants(constant);
            Assert.AreEqual(ret["constant1"], "value1=2");
            Assert.AreEqual(ret["constant2"], "value3");
        }

        [Test]
        public void TestCreateConstantsNullReturnReturnEmptyDict()
        {
            List<string> constant = null;
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstants(constant);
            Assert.AreEqual(ret.Count, 0);
        }

        [Test]
        public void TestCreateConstantsEmptyReturnEmptyDict()
        {
            List<string> constant = new List<string>();
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstants(constant);
            Assert.AreEqual(ret.Count, 0);
        }

        [Test]
        public void TestCreateConstantsWrongLinesReturnNull()
        {
            List<string> constant = new List<string>()
            {
                "testLineNotTake",
                "testanotherLineNotTake&é\"'(-è_çà)+¹~#{[|`\\^@]}¤─./§%µ,;:!ù*",
            };
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstants(constant);
            Assert.AreEqual(ret.Count, 0);
        }

        [Test]
        public void TestCreateConstraints()
        {
            List<string> constraint = new List<string>()
            {
                "constant1=value1=2",
                "constant2=value3",
            };
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstraints(constraint);
            Assert.AreEqual(ret["constant1"], "value1=2");
            Assert.AreEqual(ret["constant2"], "value3");
        }

        [Test]
        public void TestCreateConstraintsNullReturnReturnEmptyDict()
        {
            List<string> constraint = null;
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstraints(constraint);
            Assert.AreEqual(ret.Count, 0);
        }

        [Test]
        public void TestCreateConstraintsEmptyReturnEmptyDict()
        {
            List<string> constraint = new List<string>();
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstraints(constraint);
            Assert.AreEqual(ret.Count, 0);
        }

        [Test]
        public void TestCreateConstraintsWrongLinesReturnNull()
        {
            List<string> constraint = new List<string>()
            {
                "testLineNotTake",
                "testanotherLineNotTake&é\"'(-è_çà)+¹~#{[|`\\^@]}¤─./§%µ,;:!ù*",
            };
            var tool = new CreateHelper();
            Dictionary<string, string> ret = tool.CreateConstraints(constraint);
            Assert.AreEqual(ret.Count, 0);
        }
    }
}