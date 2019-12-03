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
    public class TestCommandManager
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        public class CommandLauncherHandler : ICommandLauncher
        {
            public Exception Ex { get; set; } = null;

            public CommandLauncherHandler(Exception ex)
            {
                Ex = ex;
            }

            public async Task<string> RunAndPrintCommandAsync(IConfiguration config, IPrinter printer, CancellationToken ct = default(CancellationToken))
            {
                if (Ex != null)
                {
                    throw Ex;
                }

                return await Task.Run(() => "hello");
            }
        }

        public class LauncherFactoryHandler : ILauncherFactory
        {
            public Exception Ex { get; set; } = null;

            public LauncherFactoryHandler(Exception ex)
            {
                Ex = ex;
            }

            public ICommandLauncher CreateLauncher(ConfigType type, CommandApi command)
            {
                return new CommandLauncherHandler(Ex);
            }
        }

        [Test]
        public void TestNoExceptionSendReturnNothing()
        {
            var fakeFactory = new LauncherFactoryHandler(null);
            var manager = new CommandManager(fakeFactory, null);
            manager.Start(new CreateConfiguration(ConfigType.Pool, CommandApi.Create));
        }

        [Test]
        public void TestSimpleExceptionThrowsErrorPrintException()
        {
            var fakeFactory = new LauncherFactoryHandler(new Exception());
            var manager = new CommandManager(fakeFactory, null);
            var ex = Assert.Throws<CommandManager.ErrorPrintException>(() => manager.Start(new CreateConfiguration(ConfigType.Pool, CommandApi.Create)));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void TestAggregateExceptionThrowsErrorPrintException()
        {
            var fakeFactory = new LauncherFactoryHandler(new AggregateException(new Exception[2] { new AggregateException(), new FileNotFoundException() }));
            var manager = new CommandManager(fakeFactory, null);
            var ex = Assert.Throws<CommandManager.ErrorPrintException>(() => manager.Start(new CreateConfiguration(ConfigType.Pool, CommandApi.Create)));
            Assert.IsNotNull(ex);
        }
    }
}