namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;
    using QarnotSDK;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestCommandGenericPool
    {
        [SetUp]
        public void Init()
        {
            CLILogsCheckValues.SurchargeLogs();
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
        public async Task InfoPoolFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolResponseBody;
            ConfigType type = ConfigType.Pool;
            CommandApi command = CommandApi.Info;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command) { Name = "pool_name" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "CreationDate\": \"2019-11-08T10:54:11Z";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task DeletePoolFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolResponseBody;
            ConfigType type = ConfigType.Pool;
            CommandApi command = CommandApi.Delete;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command) { Id = "PoolId" },
                FalsePrinter);

            string expected1 = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";
            string expected2 = "Pool delete";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
        }

        [Test]
        public async Task ListPoolFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolsListBodiesWithPaging;

            var commandLauncher = new CommandGeneric<QPool, CommandValues.PoolCommandValue>(
                new QPoolsRetriever(),
                new GenericCollectionCommand<QPool, CommandValues.PoolCommandValue>(new PoolModelMapper()),
                FormatTable,
                FakeApi);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(ConfigType.Pool, CommandApi.List) { Tags = new List<string>() { "hello", "world" } },
                FalsePrinter);

            string expected1 = "796a5321-0001-4a5c-2f42-54cce169dff8";
            string expected2 = "Pool Name Test List1";
            string expected3 = "Pool Name Test List3";
            StringAssert.Contains(expected1, returnString);
            StringAssert.Contains(expected2, returnString);
            StringAssert.Contains(expected3, returnString);
        }

        [Test]
        public async Task SetPoolElasticSettingsFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolsListBodiesWithPaging;
            ConfigType type = ConfigType.Pool;
            CommandApi command = CommandApi.Set;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new PoolSetElasticSettingsConfiguration(type, command),
                FalsePrinter);

            string expected1 = "796a5321-0001-4a5c-2f42-54cce169dff8";
            StringAssert.Contains(expected1, returnString);
        }

        [Test]
        public async Task UpdatePoolResourcesFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolsListBodiesWithPaging;
            ConfigType type = ConfigType.Pool;
            CommandApi command = CommandApi.UpdateResources;

            var commandLauncher = LaunchFactory.CreateLauncher(type, command);

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new DefaultRunConfiguration(type, command),
                FalsePrinter);

            string expected1 = "796a5321-0001-4a5c-2f42-54cce169dff8";
            StringAssert.Contains(expected1, returnString);
        }

        [Test]
        public async Task UpdatePoolConstantFromFakeHandlerReturnTheGoodUuid()
        {
            FakeHTTP.ReturnMessage = HttpPoolObject.PoolsListBodiesWithPaging;
            ConfigType type = ConfigType.Pool;
            CommandApi command = CommandApi.UpdateConstant;

            var commandLauncher = new CommandGeneric<QPool, CommandValues.GenericInfoCommandValue>(
                new QPoolsRetriever(),
                new UpdatePoolConstantCommand(),
                FormatTable,
                FakeApi
            );

            string returnString = await commandLauncher.RunAndPrintCommandAsync(
                new ConstantUpdateConfiguration(type, command) {ConstantName = "SOME_CONSTANT", ConstantValue = "some-new-value"},
                FalsePrinter);

            string expected1 = "796a5321-0001-4a5c-2f42-54cce169dff8";
            StringAssert.Contains(expected1, returnString);
        }

        [TestCase(CommandApi.UpdateResources)]
        [TestCase(CommandApi.UpdateConstant)]
        [TestCase(CommandApi.Delete)]
        [TestCase(CommandApi.Set)]
        public void TestBasicCommandOnPoolNotFoundReturnTheErrorMessage(CommandApi command)
        {
            FakeHTTP.ReturnStatusCode = System.Net.HttpStatusCode.NotFound;
            FakeHTTP.ReturnMessage = "{\"Message\": \"No such pool\"}";
            ConfigType type = ConfigType.Pool;

            var commandManager = new CommandManager(LaunchFactory, FalsePrinter);

            var exception = Assert.ThrowsAsync<CommandManager.ErrorPrintException>(
                async () => await commandManager.StartAsync(
                    new DefaultRunConfiguration(type, command) { Name="Invalid pool", TagsIntersect = true }),
                "Pool not found should throw from sdk");

            string expectedMessage = "No such pool";
            Assert.True(CLILogsCheckValues.Messages[CLILogs.LogsLevel.Error].Contains(expectedMessage));
        }
    }
}