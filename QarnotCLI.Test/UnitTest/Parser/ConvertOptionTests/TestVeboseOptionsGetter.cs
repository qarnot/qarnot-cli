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
    public class TestVerboseOptionGetter
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CheckConfigGetLogOptionsSetNoColor()
        {
            Options.ILogOptions option = new Options.AccountOptions();
            bool noColor = true;
            option.NoColor = noColor;
            var convertOption = new OptionConverter(new JsonDeserializer());
            convertOption.ConfigGetLogOptions(option);
            Assert.AreEqual(CLILogs.NoColor, noColor);
        }

        [Test]
        public void CheckConfigGetVerboseOptionsToQuiet()
        {
            Options.IVerboseOptions option = new Options.AccountOptions();
            option.Quiet = true;
            var convertOption = new OptionConverter(new JsonDeserializer());
            convertOption.ConfigGetVerboseOptions(option);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.NoVerbose);
        }

        [Test]
        public void CheckConfigGetVerboseOptionsToVerbose()
        {
            Options.IVerboseOptions option = new Options.AccountOptions();
            option.Verbose = true;
            var convertOption = new OptionConverter(new JsonDeserializer());
            convertOption.ConfigGetVerboseOptions(option);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Debug);
            option.Verbose = true;
            option.Quiet = true;
            convertOption.ConfigGetVerboseOptions(option);
            Assert.AreEqual(CLILogs.Verbose, CLILogs.LogsLevel.Debug);
        }
    }
}