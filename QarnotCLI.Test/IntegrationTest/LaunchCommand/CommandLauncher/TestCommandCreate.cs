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
    public class TestCommandCreate
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            FakeHTTP = new FakeHTTPHandler();
            FormatTable = new FormatterFactory.TableFormatter();
            FalsePrinter = new PrintSurchargeVoid();
            Tool = new CreateHelper();
            FakeApi = new FakeConnectionWrapper(FakeHTTP);
        }

        public FakeHTTPHandler FakeHTTP { get; set; }

        public FormatterFactory.TableFormatter FormatTable { get; set; }

        public PrintSurchargeVoid FalsePrinter { get; set; }

        public ICreateHelper Tool { get; set; }

        public IConnectionWrapper FakeApi { get; set; }

        [Test]
        public async Task CreatePoolFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolResponseBody;
            CreateConfiguration config = new CreateConfiguration(ConfigType.Pool, CommandApi.Create);
            config.Constants = new List<string>() { "key1=val1", "key2=val2" };
            config.Constraints = new List<string>() { "key1=val1", "key2=val2" };
            config.TasksDefaultWaitForPoolResourcesSynchronization = true;
            var createPool = new ApiObjectCreator.CreateNewPool(Tool);
            var commandLauncher = new CreateCommandLauncher(createPool, FormatTable, FakeApi);
            string ret = await commandLauncher.RunAndPrintCommandAsync(config, FalsePrinter);
            string expected = "Uuid : f78fdff8-7081-46e1-bb2f-d9cd4e185ece" + Environment.NewLine + "Message : New pool created, state : Success" + Environment.NewLine;
            Assert.AreEqual(ret, expected);
        }

        [Test]
        public async Task CreateJobFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.JobResponseBody;
            CreateConfiguration config = new CreateConfiguration(ConfigType.Job, CommandApi.Create);
            var createJob = new ApiObjectCreator.CreateNewJob(Tool);
            var commandLauncher = new CreateCommandLauncher(createJob, FormatTable, FakeApi);
            string ret = await commandLauncher.RunAndPrintCommandAsync(config, FalsePrinter);
            string expected = "Uuid : f78fdff8-7081-46e1-bb2f-d9cd4e185ece" + Environment.NewLine + "Message : New job created, state : Success" + Environment.NewLine;
            Assert.AreEqual(ret, expected);
        }

        [Test]
        public async Task CreateJobWithPoolFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpJobObject.JobResponseBody;
            CreateConfiguration config = new CreateConfiguration(ConfigType.Job, CommandApi.Create);
            config.PoolUuidOrShortname = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            var createJob = new ApiObjectCreator.CreateNewJob(Tool);
            var commandLauncher = new CreateCommandLauncher(createJob, FormatTable, FakeApi);
            string ret = await commandLauncher.RunAndPrintCommandAsync(config, FalsePrinter);
            string expected = "Uuid : f78fdff8-7081-46e1-bb2f-d9cd4e185ece" + Environment.NewLine + "Message : New job created, state : Success" + Environment.NewLine;
            Assert.AreEqual(ret, expected);
        }

        [Test]
        public async Task CreateTaskFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.WaitForPoolResourcesSynchronization = true;
            config.InstanceCount = 1;
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithPoolFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.PoolUuidOrShortname = "f78fdff8-0000-0000-0000-d9cd4e185ece";
            config.InstanceCount = 1;
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithPoolRangeFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.PoolUuidOrShortname = "f78fdff8-0000-0000-0000-d9cd4e185ece";
            config.Range = "1-2";
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithJobFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.JobUuidOrShortname = "f78fdff8-0000-0000-0000-d9cd4e185ece";
            config.InstanceCount = 1;
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithJobRangeFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.JobUuidOrShortname = "f78fdff8-0000-0000-0000-d9cd4e185ece";
            config.Range = "1-2";
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithRangeFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.Range = "1-2";
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithConstantAndContraintFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.Range = "1-2";
            config.Tags = new List<string>() { "tag1", "tag2" };
            config.Constants = new List<string>() { "key1=val1", "key2=val2" };
            config.Constraints = new List<string>() { "key1=val1", "key2=val2" };
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithDependenciesFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.Range = "1-2";
            config.JobUuidOrShortname = "f78fdff8-0000-0000-0000-d9cd4e185ece";
            config.Dependents = new List<string>() { "f78fdff8-0000-0000-0000-d9cd4e185ece", "f78fdff8-0000-0000-0000-d9cd4e185ece" };
            await LaunchCreateTaskFromFakeHandler(config);
        }

        [Test]
        public async Task CreateTaskWithWhitelistBlacklistAndPeriodicFromFakeHandlerReturnTheGoodUuid()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.InstanceCount = 1;
            config.SnapshotPeriodicSec = 5;
            config.Whitelist = "white_a";
            config.Blacklist = "black_a";
            await LaunchCreateTaskFromFakeHandler(config);
        }


        [Test]
        public async Task CreateTask_WithMaxRetriesPerInstance_PrintsSomethingOnStdout()
        {
            CreateConfiguration config = new CreateConfiguration(ConfigType.Task, CommandApi.Create);
            config.InstanceCount = 1;
            config.SnapshotPeriodicSec = 5;
            config.MaxRetriesPerInstance = 19;
            await LaunchCreateTaskFromFakeHandler(config);
        }

        public async Task LaunchCreateTaskFromFakeHandler(CreateConfiguration config)
        {
            FakeHTTP.ReturnMessage = HttpTaskObject.TaskResponseBody;
            var createTask = new ApiObjectCreator.CreateNewTask(Tool);
            var commandLauncher = new CreateCommandLauncher(createTask, FormatTable, FakeApi);
            string ret = await commandLauncher.RunAndPrintCommandAsync(config, FalsePrinter);
            string expected = "Uuid : f78fdff8-7081-46e1-bb2f-d9cd4e185ece" + Environment.NewLine + "Message : New task created, state : Success" + Environment.NewLine;
            Assert.AreEqual(ret, expected);
        }
    }
}