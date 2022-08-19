namespace QarnotCLI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class launcher, can get a specific set of command and run it.
    /// Catch And Print Async exceptions throws.
    /// </summary>
    public interface ICommandManager
    {
        Task StartAsync(IConfiguration config, CancellationToken ct = default);
    }

    public class CommandManager : ICommandManager
    {
        private readonly ILauncherFactory Factory;

        private readonly IPrinter PrintResult;

        public CommandManager(ILauncherFactory factory, IPrinter printResult)
        {
            this.Factory = factory;
            this.PrintResult = printResult;
        }

        private async Task StartAsyncThrowError(IConfiguration config, CancellationToken ct = default(CancellationToken))
        {
            var commandToLaunch = this.Factory.CreateLauncher(config.Type, config.Command);
            await commandToLaunch.RunAndPrintCommandAsync(config, this.PrintResult, ct);
        }

        public async Task StartAsync(IConfiguration config, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                await this.StartAsyncThrowError(config, ct);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    PrintInfoDebug.DebugException(ex);
                }

                throw new ErrorPrintException();
            }
            catch (QarnotSDK.QarnotApiException qe)
            {
                // if no info in error message, display the inner exception's message which should contain at least the status code
                if (qe.Message == "Exception of type 'QarnotSDK.QarnotApiException' was thrown." && !String.IsNullOrWhiteSpace(qe.InnerException?.Message))
                {
                    PrintInfoDebug.DebugException(qe.InnerException);
                    throw new ErrorPrintException();
                }
                PrintInfoDebug.DebugException(qe);
                throw new ErrorPrintException();
            }
            catch (Exception ex)
            {
                PrintInfoDebug.DebugException(ex);
                throw new ErrorPrintException();
            }
        }

        /// <summary>
        /// The Exceptions send by the Launcher to ask for a return error in the main.
        /// </summary>
        public class ErrorPrintException : Exception
        {
            public ErrorPrintException()
                : base()
            {
            }

            public ErrorPrintException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public ErrorPrintException(string message)
                : base(message)
            {
            }
        }
    }
}
