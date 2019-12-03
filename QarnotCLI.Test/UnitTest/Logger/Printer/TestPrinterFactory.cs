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
    public class TestPrinterFactory
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestReturnDebugRetrunTheGoodType()
        {
            var printerDebug = Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Debug, true);
            Assert.IsTrue(printerDebug is Printer.Debug);
        }

        [Test]
        public void TestReturnInfoRetrunTheGoodType()
        {
            var printer = Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Info, true);
            Assert.IsTrue(printer is Printer.Info);
        }

        [Test]
        public void TestReturnErrorRetrunTheGoodType()
        {
            var printer = Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Error, true);
            Assert.IsTrue(printer is Printer.Error);
        }

        [Test]
        public void TestReturnUsageRetrunTheGoodType()
        {
            var printer = Printer.PrinterFactory.Factory(CLILogs.LogsLevel.Usage, true);
            Assert.IsTrue(printer is Printer.Usage);
        }
    }
}