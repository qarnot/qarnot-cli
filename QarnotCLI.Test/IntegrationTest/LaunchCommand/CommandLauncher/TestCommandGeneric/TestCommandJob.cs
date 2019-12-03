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
    public class TestCommandGenericJob
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
        public async Task InfoJobFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.JobsListBodies;
            ConfigType type = ConfigType.Job;
            CommandApi command = CommandApi.Info;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command),
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "2019-11-08T10:54:11Z";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task DeleteJobFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.JobResponseBody;
            ConfigType type = ConfigType.Job;
            CommandApi command = CommandApi.Delete;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command) { Id = "JobId" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Job delete";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task AbortJobFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.ActiveJobsListBodies;

            var commandLauncher = new CommandGeneric<QJob, CommandValues.GenericInfoCommandValue>(
                new QJobsRetriever(),
                new AbortJobCommand(null),
                FormatTable,
                FakeApi);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(ConfigType.Job, CommandApi.Abort),
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Job terminate";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task ListJobFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.ActiveJobsListBodies;

            var commandLauncher = new CommandGeneric<QJob, CommandValues.JobCommandValue>(
                new QJobsRetriever(),
                new GenericCollectionCommand<QJob, CommandValues.JobCommandValue>(new JobModelMapper()),
                FormatTable,
                FakeApi);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(ConfigType.Job, CommandApi.List) { Name = "name" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "job_name";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }
    }
}