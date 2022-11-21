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
    public class TestFormatStatic
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void DefaultResultFormatReturnTable()
        {
           Assert.AreEqual(FormatterFactory.DefaultResultFormat(), "TABLE");
        }

        [Test]
        public void CheckFormatForTableReturnTrue()
        {
           Assert.IsTrue(FormatterFactory.CheckFormat("TABLE"));
        }

        [Test]
        public void CheckFormatForJsonReturnTrue()
        {
           Assert.IsTrue(FormatterFactory.CheckFormat("JSON"));
        }

        [Test]
        public void CheckFormatForPhpReturnFalse()
        {
           var ex = Assert.Throws<QarnotCLI.ParseException>(() => FormatterFactory.CheckFormat("PHP"));
           Assert.IsNotNull(ex);
        }

        [Test]
        public void CreateFormatForTableReturnFormatTable()
        {
           Assert.IsTrue(FormatterFactory.CreateFormat("TABLE") is FormatterFactory.TableFormatter);
        }

        [Test]
        public void CreateFormatForJsonReturnFormatJson()
        {
           Assert.IsTrue(FormatterFactory.CreateFormat("JSON") is FormatterFactory.JsonFormatter);
        }

        [Test]
        public void CreateFormatForPhpThrowError()
        {
           var ex = Assert.Throws<MissingMethodException>(() => FormatterFactory.CreateFormat("PHP"));
           Assert.IsNotNull(ex);
        }
    }
}