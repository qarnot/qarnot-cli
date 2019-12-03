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
    public class TestCreateLoggers
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.DeleteLogs();
        }

        [Test]
        public void CheckWhenAllTheLogsAreCreate()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.CreateLoggers();
            Assert.IsTrue(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Debug));
            Assert.IsTrue(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Info));
            Assert.IsTrue(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Error));
            Assert.IsTrue(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Usage));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.NoVerbose));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Result));
        }

        [Test]
        public void CheckAllTheLogsType()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.Debug;
            CLILogs.CreateLoggers();
            Assert.IsTrue(CLILogs.Logs[CLILogs.LogsLevel.Debug] is Printer.Debug);
            Assert.IsTrue(CLILogs.Logs[CLILogs.LogsLevel.Info] is Printer.Info);
            Assert.IsTrue(CLILogs.Logs[CLILogs.LogsLevel.Error] is Printer.Error);
            Assert.IsTrue(CLILogs.Logs[CLILogs.LogsLevel.Usage] is Printer.Usage);
        }

        [Test]
        public void CheckWhenNoLogMustBeCreate()
        {
            CLILogs.Verbose = CLILogs.LogsLevel.NoVerbose;
            CLILogs.CreateLoggers();
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Debug));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Info));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Error));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Usage));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.NoVerbose));
            Assert.IsFalse(CLILogs.Logs.ContainsKey(CLILogs.LogsLevel.Result));
        }
    }
}