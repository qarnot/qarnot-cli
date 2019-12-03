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
    public class TestCreateOneLogger
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.DeleteLogs();
        }

        [Test]
        public void ALevelLowerThanVerboseMustNotBeCreate()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Error;
            var key = CLILogs.LogsLevel.Debug;
            CLILogs.CreateOneLogger(key);
            Assert.IsFalse(CLILogs.Logs.ContainsKey(key));
        }

        [Test]
        public void ALevelEqualOrUpperThanVerboseMustBeCreate()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Error;
            var key = CLILogs.LogsLevel.Error;
            CLILogs.CreateOneLogger(key);
            Assert.IsTrue(CLILogs.Logs.ContainsKey(key));
            key = CLILogs.LogsLevel.Usage;
            CLILogs.CreateOneLogger(key);
            Assert.IsTrue(CLILogs.Logs.ContainsKey(key));
        }

        [Test]
        public void ALevelEqualOrUpperThanLogsLevelNoVerboseIsNotSet()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Error;
            var key = CLILogs.LogsLevel.NoVerbose;
            CLILogs.CreateOneLogger(key);
            Assert.IsFalse(CLILogs.Logs.ContainsKey(key));
        }
    }
}