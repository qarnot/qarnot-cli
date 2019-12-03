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
    public class TestChangeVerboseLevel
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestVerboseChangeToDebug()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Error;
            CLILogs.ChangeVerboseLevel(1);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Debug);
        }

        [Test]
        public void TestVerboseChangeToInfo()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.ChangeVerboseLevel(2);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Info);
        }

        [Test]
        public void TestVerboseChangeToError()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.ChangeVerboseLevel(3);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Error);
        }

        [Test]
        public void TestVerboseChangeToUsage()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.ChangeVerboseLevel(4);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Usage);
        }

        [Test]
        public void TestVerboseChangeToNoVerbose()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.ChangeVerboseLevel(5);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.NoVerbose);
        }
    }
}