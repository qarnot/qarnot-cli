using System.CommandLine;

namespace QarnotCLI;

public class VersionCommand : Command
{
    public VersionCommand(AssemblyDetails details, ILogger logger)
        : base("version", "Display version information")
    {
        this.SetHandler(() => logger.Result(details.ToString()));
    }
}
