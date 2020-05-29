namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Type log static class.
    /// </summary>
    public static class CLILogs
    {
        /// <summary>
        /// Cli Logger enum.
        /// </summary>
        public enum LogsLevel
        {
            /// <summary>
            /// Type for all the log info.
            /// </summary>
            Debug,

            /// <summary>
            /// Type for the standard log info.
            /// </summary>
            Info,

            /// <summary>
            /// Type for the Error log info.
            /// </summary>
            Error,

            /// <summary>
            /// Type for the Error log info.
            /// </summary>
            Warning,

            /// <summary>
            /// Type for the Usage log info.
            /// </summary>
            Usage,

            /// <summary>
            /// Type for the no-log option.
            /// </summary>
            NoVerbose,

            /// <summary>
            /// Unused Type.
            /// </summary>
            Result,
        }

        public static Dictionary<LogsLevel, IPrinter> Logs { get; } = new Dictionary<LogsLevel, IPrinter>();

        public static LogsLevel Verbose { get; set; } = LogsLevel.Info;

        public static bool NoColor { get; set; } = false;

        public static bool LogSet { get; set; } = false;

        public static void CreateOneLogger(CLILogs.LogsLevel level)
        {
            if (Verbose <= level && level < LogsLevel.NoVerbose)
            {
                Logs[level] = Printer.PrinterFactory.Factory(level, !NoColor);
                CLILogs.Debug("Verbose set " + Enum.GetName(typeof(LogsLevel), level));
            }
        }

        public static void CreateLoggers()
        {
            if (!LogSet)
            {
                LogSet = true;
                foreach (LogsLevel level in Enum.GetValues(typeof(LogsLevel)))
                {
                    CreateOneLogger(level);
                }

                CLILogs.Debug("Set Console Color " + (!NoColor).ToString());
            }
        }

        public static void SetNoColor(bool suppressColor)
        {
            NoColor = suppressColor;
        }

        public static void ChangeVerboseLevel(int verbose)
        {
            switch (verbose)
            {
                case 1:
                    Verbose = LogsLevel.Debug;
                    break;
                case 2:
                    Verbose = LogsLevel.Info;
                    break;
                case 3:
                    Verbose = LogsLevel.Error;
                    break;
                case 4:
                    Verbose = LogsLevel.Usage;
                    break;
                case 5:
                    Verbose = LogsLevel.NoVerbose;
                    break;
                default:
                    break;
            }
        }

        public static void Log(string message, LogsLevel level)
        {
            if (Verbose <= level)
            {
                Logs[level].PrintAsync(message);
            }
        }

        public static void Info(string logMessage)
        {
            Log(logMessage, LogsLevel.Info);
        }

        public static void Warn(string logMessage)
        {
            Log(logMessage, LogsLevel.Warning);
        }

        public static void Debug(
                        string logMessage,
                        [CallerFilePath] string file = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            string logTime = string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", DateTime.UtcNow);
            logMessage = $"[{logTime}] {Path.GetFileName(file)}_l.{line}({member}): {logMessage}";

            Log(logMessage, LogsLevel.Debug);
        }

        public static void Error(string logMessage)
        {
            Log(logMessage, LogsLevel.Error);
        }

        public static void Usage(string logMessage)
        {
            Log(logMessage, LogsLevel.Usage);
        }
    }
}
