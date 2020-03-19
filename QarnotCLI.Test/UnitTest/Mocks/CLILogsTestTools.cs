namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotCLI;

    public static class CLILogsTest
    {
        public static void SurchargeLogs()
        {
            foreach (QarnotCLI.CLILogs.LogsLevel key in Enum.GetValues(typeof(QarnotCLI.CLILogs.LogsLevel)))
            {
                CLILogs.Logs[key] = new PrintSurchargeVoid();
            }

            CLILogs.LogSet = true;
        }

        public static void DeleteLogs()
        {
            CLILogs.Logs.Clear();
            CLILogs.LogSet = false;
            CLILogs.Verbose = CLILogs.LogsLevel.Info;
        }
    }

    public class PrintSurchargeVoid : Printer.APrint
    {
        public PrintSurchargeVoid()
        : base(false, new PrintVoid())
        {
        }

        public override void Print(string text)
        {
        }
    }

    public class PrintVoid : Printer.IMessagePrinter
    {
        public Task PrintMessageAsync(string message, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}