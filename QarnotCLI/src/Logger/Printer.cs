namespace QarnotCLI
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPrinter
    {
        Task PrintAsync(string message);
    }

    public class Printer
    {
        public interface IMessagePrinter
        {
            Task PrintMessageAsync(string message, CancellationToken ct = default);
        }

        public class MessagePrinter : IMessagePrinter
        {
            private readonly TextWriter Writer;

            public MessagePrinter(TextWriter tw)
            {
                Writer = tw;
            }

            public async Task PrintMessageAsync(string message, CancellationToken ct = default)
            {
                await Writer.WriteAsync(message.AsMemory(), cancellationToken: ct);
            }
        }

        public class PrinterFactory
        {
            public static APrint Factory(CLILogs.LogsLevel level, bool color)
            {
                switch (level)
                {
                    case CLILogs.LogsLevel.Debug:
                        return new Printer.Debug(color, new MessagePrinter(Console.Out));
                    case CLILogs.LogsLevel.Info:
                        return new Printer.Info(color, new MessagePrinter(Console.Out));
                    case CLILogs.LogsLevel.Error:
                        return new Printer.Error(color, new MessagePrinter(Console.Error));
                    case CLILogs.LogsLevel.Warning:
                        return new Printer.Warning(color, new MessagePrinter(Console.Error));
                    case CLILogs.LogsLevel.Usage:
                        return new Printer.Usage(color, new MessagePrinter(Console.Error));
                    case CLILogs.LogsLevel.Result:
                        return new Printer.Result(color, new MessagePrinter(Console.Out));
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public abstract class APrint : IPrinter
        {
            protected APrint(bool color, IMessagePrinter printer)
            {
                Color = color;
                Printer = printer;
            }

            private IMessagePrinter Printer { get; set; }

            protected bool Color { get; set; } = true;

            protected virtual string PrefixStdColor { get; } = string.Empty;

            protected virtual string PrefixStdNoColor { get; } = string.Empty;

            protected virtual ConsoleColor TextColorPrefix { get; } = Console.ForegroundColor;

            protected virtual ConsoleColor BackgroundColorPrefix { get; } = Console.BackgroundColor;

            protected virtual ConsoleColor TextColorMessage { get; } = Console.ForegroundColor;

            protected virtual ConsoleColor BackgroundColorMessage { get; } = Console.BackgroundColor;

            protected virtual string SuffixStdColor { get; } = string.Empty;

            protected virtual string SuffixStdNoColor { get; } = string.Empty;

            protected virtual ConsoleColor TextColorSuffix { get; } = Console.ForegroundColor;

            protected virtual ConsoleColor BackgroundColorSuffix { get; } = Console.BackgroundColor;

            protected async Task PrintInColorAsync(string message, ConsoleColor colorForground, ConsoleColor colorBackground)
            {
                Console.ForegroundColor = colorForground;
                Console.BackgroundColor = colorBackground;
                await Printer.PrintMessageAsync(message);
                Console.ResetColor();
            }

            private async Task PrintMessageWithColorAsync(string message)
            {
                await PrintInColorAsync(this.PrefixStdColor, this.TextColorPrefix, this.BackgroundColorPrefix);
                await PrintInColorAsync(message, this.TextColorMessage, this.BackgroundColorMessage);
                await PrintInColorAsync(this.SuffixStdColor, this.TextColorSuffix, this.BackgroundColorSuffix);
                await Printer.PrintMessageAsync(Environment.NewLine);
            }

            protected virtual string ConcatText(string prefix, string text, string suffix)
            {
                return prefix + text + suffix + Environment.NewLine;
            }

            protected virtual string ConcatStdNoColorText(string text)
            {
                return this.ConcatText(this.PrefixStdNoColor, text, this.SuffixStdNoColor);
            }

            public virtual async Task PrintAsync(string message)
            {
                if (this.Color)
                {
                    await PrintMessageWithColorAsync(message);
                }
                else
                {
                    await Printer.PrintMessageAsync(ConcatStdNoColorText(message));
                }
            }
        }

        public class Debug : APrint
        {
            public Debug(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }

            protected override string PrefixStdColor { get; } = "[Trace]: ";

            protected override ConsoleColor TextColorPrefix { get; } = ConsoleColor.Green;
        }

        public class Info : APrint
        {
            public Info(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }
        }

        public class Result : APrint
        {
            public Result(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }
        }

        public class Error : APrint
        {
            public Error(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }

            protected override ConsoleColor TextColorPrefix { get; } = ConsoleColor.Red;

            protected override string PrefixStdColor { get; } = "[Error]: ";

            protected override string PrefixStdNoColor { get; } = "[Error]: ";
        }

        public class Warning : APrint
        {
            public Warning(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }

            protected override ConsoleColor TextColorPrefix { get; } = ConsoleColor.DarkYellow;
            protected override ConsoleColor TextColorMessage { get; } = ConsoleColor.DarkYellow;

            protected override string PrefixStdColor { get; } = "[Warning]: ";

            protected override string PrefixStdNoColor { get; } = "[Warning]: ";
        }

        public class Usage : APrint
        {
            public Usage(bool color, IMessagePrinter printMessage)
            : base(color, printMessage)
            {
            }
        }
    }
}
