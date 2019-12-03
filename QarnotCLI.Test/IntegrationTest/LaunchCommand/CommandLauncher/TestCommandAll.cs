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
    public class TestCommandAll
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
        public void TestForCommandAll()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TasksListBodies;
            ConfigType type = ConfigType.All;
            CommandApi command = CommandApi.List;

            Dictionary<string, ICommandLauncher> dict = new Dictionary<string, ICommandLauncher>()
            {
               { "first", LaunchFactory.CreateLauncher(ConfigType.Task, command) },
               { "second", LaunchFactory.CreateLauncher(ConfigType.Pool, command) },
            };

            var allConfig = new DefaultRunConfiguration(type, command);
            var all = new CommandAll(dict);
            var ret = all.RunAndPrintCommandAsync(allConfig, FalsePrinter);
        }
    }
}