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
    using QarnotSDK;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestCommandOneGenericAccount
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            FakeHTTP = new FakeHTTPHandler();
            FormatTable = new FormatterFactory.TableFormatter();
            FalsePrinter = new PrintSurchargeVoid();
            FakeApi = new FakeConnectionWrapper(FakeHTTP);
            LaunchFactory = new LauncherFactory(FormatTable, FakeApi);
        }

        public FakeHTTPHandler FakeHTTP { get; set; }

        public FormatterFactory.TableFormatter FormatTable { get; set; }

        public PrintSurchargeVoid FalsePrinter { get; set; }

        public ICreateHelper Tool { get; set; }

        public IConnectionWrapper FakeApi { get; set; }

        public LauncherFactory LaunchFactory { get; set; }

        [Test]
        public async Task InfoAccountFromFakeHandlerReturnTheGoodValues()
        {
            FakeHTTP.ReturnMessage = HttpAccountObject.AccountResponseBody;
            ConfigType type = ConfigType.Account;
            CommandApi command = CommandApi.Info;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new AccountConfiguration(type, command),
                FalsePrinter);

            StringAssert.Contains("AccountInformation", returnString);
            StringAssert.Contains("Email : john.smith@qarnot-computing.com", returnString);
            StringAssert.Contains("MaxInstances : 50", returnString);
            StringAssert.Contains("Quota Bucket Number : 3/100", returnString);
            StringAssert.Contains("Quota Bytes Bucket : 368640/10737412742", returnString);
            StringAssert.Contains("Quota Task Count : 0/50", returnString);
            StringAssert.Contains("Quota Running Task : 1/50", returnString);
            StringAssert.Contains("Quota Total Pool : 0/50", returnString);
            StringAssert.Contains("Quota Running Pool : 11/50", returnString);
        }

        [Test]
        [SetCulture("en-US")]
        public async Task InfoAccountFromFakeHandlerReturnTheGoodHumanReadableValues()
        {
            FakeHTTP.ReturnMessage = HttpAccountObject.AccountResponseBody;
            ConfigType type = ConfigType.Account;
            CommandApi command = CommandApi.Info;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new AccountConfiguration(type, command) { HumanReadable = true },
                FalsePrinter);

            StringAssert.Contains("AccountInformation", returnString);
            StringAssert.Contains("Email : john.smith@qarnot-computing.com", returnString);
            StringAssert.Contains("MaxInstances : 50", returnString);
            StringAssert.Contains("Quota Bucket Number : 3/100", returnString);
            StringAssert.Contains("Quota Bytes Bucket : 360.0 KB/ 10.0 GB", returnString);
            StringAssert.Contains("Quota Task Count : 0/50", returnString);
            StringAssert.Contains("Quota Running Task : 1/50", returnString);
            StringAssert.Contains("Quota Total Pool : 0/50", returnString);
            StringAssert.Contains("Quota Running Pool : 11/50", returnString);
        }
    }
}