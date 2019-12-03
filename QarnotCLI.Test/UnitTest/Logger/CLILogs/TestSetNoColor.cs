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
    public class TestSetNoColor
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void SetNoColorTrue()
        {
            bool noColor = true;
            CLILogs.SetNoColor(noColor);
            Assert.AreEqual(CLILogs.NoColor, noColor);
        }

        [Test]
        public void SetNoColorFalse()
        {
            bool noColor = false;
            CLILogs.SetNoColor(noColor);
            Assert.AreEqual(CLILogs.NoColor, noColor);
        }
    }
}