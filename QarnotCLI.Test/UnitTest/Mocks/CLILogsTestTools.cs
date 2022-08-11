using System.Collections.Generic;
namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotCLI;

    public static class CLILogsTest
    {
        public static void SurchargeLogs(bool resetVerbose = true)
        {
            CLILogs.Verbose = resetVerbose ? CLILogs.LogsLevel.Info : CLILogs.Verbose;
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
        public List<string> Logs { get; set; }
        public PrintSurchargeVoid()
        : base(false, new PrintVoid())
        {
            Logs = new();
        }

        public override Task PrintAsync(string text)
        {
            Logs.Add(text);
            return Task.CompletedTask;
        }
    }

    public class PrintVoid : Printer.IMessagePrinter
    {
        public Task PrintMessageAsync(string message, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }

    public static class CLILogsCheckValues
    {
        public static Dictionary<QarnotCLI.CLILogs.LogsLevel, string> Messages = new Dictionary<QarnotCLI.CLILogs.LogsLevel, string>();

        public static string AllMessages;

        public static void SurchargeLogs(bool resetVerbose = true)
        {
            CLILogs.Verbose = resetVerbose ? CLILogs.LogsLevel.Info : CLILogs.Verbose;
            foreach (QarnotCLI.CLILogs.LogsLevel key in Enum.GetValues(typeof(QarnotCLI.CLILogs.LogsLevel)))
            {
                CLILogs.Logs[key] = new PrintSurchargeGetMessages(key);
                CLILogsCheckValues.Messages[key] = string.Empty;
            }
            CLILogsCheckValues.AllMessages = string.Empty;

            CLILogs.LogSet = true;
        }

        public static void DeleteLogs()
        {
            CLILogs.Logs.Clear();
            CLILogs.LogSet = false;
            CLILogs.Verbose = CLILogs.LogsLevel.Info;
        }
    }

    public class PrintSurchargeGetMessages : Printer.APrint
    {
        QarnotCLI.CLILogs.LogsLevel Key;

        public PrintSurchargeGetMessages(QarnotCLI.CLILogs.LogsLevel key)
        : base(false, new PrintVoid())
        {
            Key = key;
        }

        public override Task PrintAsync(string text)
        {
            CLILogsCheckValues.AllMessages += text;
            CLILogsCheckValues.Messages[Key] += text;
            return Task.CompletedTask;
        }
    }
}
