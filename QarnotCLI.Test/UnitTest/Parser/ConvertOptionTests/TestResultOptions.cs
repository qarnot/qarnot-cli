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
    public class TestResultFormatGetter
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            Option = new Options.AccountOptions();
            Config = new DefaultRunConfiguration(ConfigType.Task, CommandApi.List);
            Convert = new OptionConverter(new UnitTestJsonDeserializer());
        }

        private Options.IOptions Option { get; set; }

        private IConfiguration Config { get; set; }

        private OptionConverter Convert { get; set; }

        [Test]
        public void CheckGetResultOptionsForJSON()
        {
            string format = "JSON";
            Option.ResultFormat = format;
            Convert.GetDefaultOptions(Config, Option);
            Assert.AreEqual(Config.ResultFormat, format);
        }

        [Test]
        public void CheckGetResultOptionsForTABLE()
        {
            string format = "TABLE";
            Option.ResultFormat = format;
            Convert.GetDefaultOptions(Config, Option);
            Assert.AreEqual(Config.ResultFormat, format);
        }

        [Test]
        public void CheckGetResultOptionsMatchAlsoLowerCaseFormat()
        {
            string format = "json";
            Option.ResultFormat = format;
            Convert.GetDefaultOptions(Config, Option);
            Assert.AreEqual(Config.ResultFormat, format.ToUpper());
        }

        [Test]
        public void GetResultOptionsUnknowTypeMustThrowAnError()
        {
            string format = "PHP";
            Option.ResultFormat = format;
            var ex = Assert.Throws<ParseException>(() => Convert.GetDefaultOptions(Config, Option));
            Assert.IsNotNull(ex);
        }
    }
}