using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

namespace QarnotCLI;

public static class CustomHelpLayout
{
    public static void ApplyCustomHelp(RootCommand rootCommand, AssemblyDetails details)
    {
        ApplyToCommand(rootCommand, details);
        foreach (var cmd in GetAllCommands(rootCommand))
        {
            ApplyToCommand(cmd, details);
        }
    }

    private static void ApplyToCommand(Command command, AssemblyDetails details)
    {
        var helpOption = command.Options.OfType<HelpOption>().FirstOrDefault();
        if (helpOption is null)
        {
            return;
        }

        // Remove the default "-h" alias since it's used for "human readable"
        helpOption.Aliases.Remove("-h");

        helpOption.Action = new CustomHelpAction(details);
    }

    private static IEnumerable<Command> GetAllCommands(Command command)
    {
        foreach (var sub in command.Subcommands)
        {
            yield return sub;
            foreach (var nested in GetAllCommands(sub))
            {
                yield return nested;
            }
        }
    }
}

public class CustomHelpAction : SynchronousCommandLineAction
{
    private readonly AssemblyDetails Details;

    public CustomHelpAction(AssemblyDetails details)
    {
        Details = details;
    }

    public override int Invoke(ParseResult parseResult)
    {
        Console.WriteLine(Details);

        var helpAction = new HelpAction();
        helpAction.Invoke(parseResult);

        var command = parseResult.CommandResult.Command;
        if (!command.Hidden && command is CommandWithExamples cmdWithExamples)
        {
            PrintExamples(cmdWithExamples.Examples);
        }

        return 0;
    }

    private static void PrintExamples(IReadOnlyList<Example> examples)
    {
        foreach (var example in examples)
        {
            Console.WriteLine(example.Title + ":");
            foreach (var line in example.CommandLines)
            {
                Console.WriteLine("  " + line);
            }
        }
    }
}
