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
    public class TestAPrint
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        private class StockPrint : Printer.IMessagePrinter
        {
            public string StockPrintMessage { get; set; } = string.Empty;

            public Task PrintMessageAsync(string message, CancellationToken ct = default)
            {
                StockPrintMessage += message;
                return Task.CompletedTask;
            }
        }

        [Test]
        public void TestSendAnInfoStringNoColor()
        {
            var stockMessage = new StockPrint();
            Printer.APrint printer = new Printer.Info(false, stockMessage);
            string message = "test";
            printer.Print(message);
            Assert.AreEqual(stockMessage.StockPrintMessage, message + Environment.NewLine);
        }

        [Test]
        public void TestSendAnErrorStringNoColor()
        {
            var stockMessage = new StockPrint();
            Printer.APrint printer = new Printer.Error(false, stockMessage);
            string message = "test";
            printer.Print(message);
            Assert.AreEqual(stockMessage.StockPrintMessage, "[Error]: " + message + Environment.NewLine);
        }

        [Test]
        public void TestSendAnErrorStringColor()
        {
            var stockMessage = new StockPrint();
            Printer.APrint printer = new Printer.Error(true, stockMessage);
            string message = "test";
            printer.Print(message);
            Assert.AreEqual(stockMessage.StockPrintMessage, "[Error]: " + message + Environment.NewLine);
        }
    }
}