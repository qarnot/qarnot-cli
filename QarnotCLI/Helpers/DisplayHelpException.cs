using System.CommandLine;

namespace QarnotCLI;

public class DisplayHelpException : Exception
{
    public Command Command { get; }

    public DisplayHelpException(Command command)
    {
        Command = command;
    }
}
