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
    public class TestCommandGenericTask
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
        public async Task InfoTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TasksListBodies;
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.Info;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command) { Name = "task_name1" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "CreationDate\": \"2019-11-08T10:54:11Z";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task WaitTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TasksListBodies;
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.Wait;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new StdConfiguration(type, command) { Tags = new List<string>() { "hello", "world" } },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Task Wait end status : Success";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task StdoutTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessageList = new List<string>() {
                HttpTaskObject.TasksListBodies,
                "hello stdout"
                };
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.GetStdout;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new StdConfiguration(type, command) { Tags = new List<string>() { "hello", "world" } },
                FalsePrinter);

            string expected = "hello stdout";
            StringAssert.Contains(expected, returnString);
        }

        [Test]
        public async Task StderrTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessageList = new List<string>() {
                HttpTaskObject.TasksListBodies,
                "hello stderr"
                };
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.GetStderr;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new StdConfiguration(type, command) { Tags = new List<string>() { "hello", "world" } },
                FalsePrinter);

            string expected = "hello stderr";
            StringAssert.Contains(expected, returnString);
        }

        [Test]
        public async Task AbortTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.ActiveTasksListBodies;

            var commandLauncher = new CommandGeneric<QTask, CommandValues.GenericInfoCommandValue>(
                new QTasksRetriever(),
                new AbortTaskCommand(),
                FormatTable,
                FakeApi);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(ConfigType.Task, CommandApi.Abort),
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Task abort";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task DeleteTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TaskResponseBody;
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.Delete;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command) { Id = "TaskId" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Task delete";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task ListTaskFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TasksListBodies;

            var commandLauncher = new CommandGeneric<QTask, CommandValues.TaskCommandValue>(
                new QTasksRetriever(),
                new GenericCollectionCommand<QTask, CommandValues.TaskCommandValue>(new TaskModelMapper()),
                FormatTable,
                FakeApi);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(ConfigType.Task, CommandApi.List),
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "task_name1";
            string expected3 = "task_name2";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
            StringAssert.Contains(expected3, returnString);
        }

        [Test]
        public async Task UpdateTaskResourcesFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TasksListBodies;
            ConfigType type = ConfigType.Task;
            CommandApi command = CommandApi.UpdateResources;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command),
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            StringAssert.Contains(expected1, returnString);
        }
    }
}