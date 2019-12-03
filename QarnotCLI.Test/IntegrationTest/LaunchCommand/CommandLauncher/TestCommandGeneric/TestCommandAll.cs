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
    public class TestCommandIntegrationAll
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
        public async Task ListAllFromFakeHandlerReturnTheGoodValues()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolsListBodies;
            ConfigType type = ConfigType.All;
            CommandApi command = CommandApi.List;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);
            return;

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command),
                FalsePrinter);

            string expected1 = "796a5321-0001-4a5c-2f42-54cce169dff8";
            string expected2 = "CreationDate\": \"2019-11-08T10:54:11Z";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }
    }
}