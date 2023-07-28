using System.Runtime.CompilerServices;

namespace QarnotCLI;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Result,
}

public interface ILogger
{
    void Debug(string msg, string file = "", string member = "", int line = 0);
    void Info(string msg);
    void Warning(string msg);
    void Error(string msg);
    void Error(Exception e, string msg);
    void Result(string msg);
    void Log(string msg, LogLevel level);
}

public interface ILoggerFactory
{
    ILogger Create(GlobalModel options);
}

public class LoggerFactory : ILoggerFactory
{
    public ILogger Create(GlobalModel options) =>
        new Logger(options);
}

public class Logger : ILogger
{
    public bool NoColor { get; private set; }
    public LogLevel Level { get; private set; } = LogLevel.Info;

    public Logger()
    {
    }

    public Logger(GlobalModel options)
    {
        NoColor = options.NoColor;
        if (options.Verbose)
        {
            Level = LogLevel.Debug;
        }
        else if (options.Quiet)
        {
            Level = LogLevel.Error;
        }
    }

    public void Debug(
        string msg,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0
    )
    {
        string logTime = string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", DateTime.UtcNow);
        Log($"[{logTime}] {Path.GetFileName(file)}_l.{line}({member}): {msg}", LogLevel.Debug);
    }

    public void Info(string msg) =>
        Log(msg, LogLevel.Info);

    public void Warning(string msg) =>
        Log(msg, LogLevel.Warning);

    public void Error(string msg) =>
        Log(msg, LogLevel.Error);

    public void Error(Exception e, string msg) =>
        Log($"{msg}{Environment.NewLine}{e}", LogLevel.Error);

    public void Result(string msg) =>
        Log(msg, LogLevel.Result);

    public void Log(string msg, LogLevel level)
    {
        if (Level > level)
        {
            return;
        }

        WritePrefix(level);

        if (level == LogLevel.Result)
        {
            Console.WriteLine(msg);
        }
        else
        {
            Console.Error.WriteLine(msg);
        }

        if (!NoColor) Console.ResetColor();
    }

    private void WritePrefix(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Debug:
                if (!NoColor) Console.ForegroundColor = ConsoleColor.Green;
                Console.Error.Write("[Debug]: ");
                if (!NoColor) Console.ResetColor();
                break;

            case LogLevel.Warning:
                if (!NoColor) Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Error.Write("[Warning]: ");
                // Don't reset the color so the rest of the message is yellow.
                break;

            case LogLevel.Error:
                if (!NoColor) Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write("[Error]: ");
                if (!NoColor) Console.ResetColor();
                break;

            default:
                // Nothing to do.
                break;
        }
    }
}
