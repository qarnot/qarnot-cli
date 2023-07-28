using Moq;
using QarnotCLI;
using System.CommandLine;
using System.Text;

var releasesService = new ReleasesService();

// Mock everything as we are only interested in the options themselves.
var parser = new CommandLineBuilderFactory(
    _ => new Mock<ITaskUseCases>().Object,
    _ => new Mock<IPoolUseCases>().Object,
    _ => new Mock<IJobUseCases>().Object,
    _ => new Mock<IBucketUseCases>().Object,
    _ => new Mock<IAllUseCases>().Object,
    _ => new Mock<ISecretsUseCases>().Object,
    _ => new Mock<IConfigUseCases>().Object,
    _ => new Mock<IAccountUseCases>().Object
).Create(
    new(), releasesService, new Mock<ILogger>().Object
).Build();

var rootCmd = parser.Configuration.RootCommand;
rootCmd.Name = "Commands";

var formatter = new Formatter(releasesService, rootCmd.Options);
formatter.FormatCmd(rootCmd);
foreach (var cmd in rootCmd.Children.OfType<Command>())
{
    formatter.FormatCmd(cmd);
}

public class Formatter
{
    private const string OUTPUT_DIR = "manMarkDown";
    private const string BINARY_NAME = "qarnot";

    private readonly string Version;
    private readonly string Copyright;
    private readonly IReadOnlyList<Option> GlobalOptions;

    public Formatter(IReleasesService releasesService, IReadOnlyList<Option> globalOptions)
    {
        Version = releasesService.GetAssemblyDetails().ToString();
        Copyright = $"Copyright (C) {DateTime.UtcNow.Year} Qarnot computing";
        GlobalOptions = globalOptions;
    }

    public void FormatCmd(Command cmd, IEnumerable<Command>? parents = null)
    {
        parents ??= Enumerable.Empty<Command>();
        var fullCmd = !parents.Any()
            ? cmd.Name
            : $"{cmd.Name} {string.Join(" ", parents.Select(p => p.Name))}";

        var mdFile = Path.Join(OUTPUT_DIR, Helpers.MdFile(parents.Append(cmd)));
        using var f = File.OpenWrite(mdFile);
        using var writer = new StreamWriter(f);

        Console.WriteLine($"Writing markdown documentation for `qarnot {fullCmd}` to {mdFile}");

        var subcmds = cmd.Children.OfType<Command>();
        if (subcmds.Any())
        {
            FormatCmdWithSubcmds(cmd, subcmds, writer);

            if (cmd is not RootCommand)
            {
                foreach (var subcmd in subcmds)
                {
                    FormatCmd(subcmd, parents.Append(cmd));
                }
            }
        }
        else
        {
            FormatCmdWithoutSubcmds(cmd, fullCmd, writer);
        }
    }

    private void FormatCmdWithoutSubcmds(Command cmd, string fullCmd, StreamWriter writer)
    {
        writer.WriteLine($"# {cmd.CapitalizedName()}");
        writer.WriteLine($"> {cmd.Description}");
        writer.WriteLine();

        writer.WriteLine("Unix");
        writer.WriteLine("```bash");
        writer.WriteLine($" {BINARY_NAME} {fullCmd}");
        writer.WriteLine("```");

        writer.WriteLine("Windows");
        writer.WriteLine("```bash");
        writer.WriteLine($" {BINARY_NAME}.exe {fullCmd}");
        writer.WriteLine("```");


        if (cmd is CommandWithExamples withExamples)
        {
            writer.WriteLine("***");
            FormatExamples(withExamples, writer);
        }

        writer.WriteLine("***");
        writer.WriteLine("### Flags");
        writer.WriteLine();
        writer.WriteLine("| flag | description |");
        writer.WriteLine("|:--|:--|");
        foreach (var arg in cmd.Arguments)
        {
            writer.WriteLine($"|{arg.Name}|{arg.Description}|");
        }
        foreach (var opt in cmd.Options.Concat(GlobalOptions))
        {
            writer.WriteLine($"|{string.Join(", ", opt.Aliases)}|{opt.Description}|");
        }

        writer.WriteLine();
        writer.WriteLine($"*Version*: *{Version}*\\");
        writer.WriteLine($"*Copyright*: *{Copyright}*");
    }

    private void FormatCmdWithSubcmds(Command cmd, IEnumerable<Command> subcmds, TextWriter writer)
    {
        writer.WriteLine("# QarnotCLI");
        writer.WriteLine("> List of commands");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine($"### {cmd.CapitalizedName()}");
        writer.WriteLine();
        writer.WriteLine("| name | description |");
        writer.WriteLine("|:--|:--|");

        foreach (var subcmd in subcmds)
        {
            var mdFile = Helpers.MdFile(new List<Command> {cmd, subcmd});
            var prefix = cmd.Name ;
            if (cmd is RootCommand)
            {
                prefix = "";
                mdFile = Helpers.MdFile(new List<Command> {subcmd});
            }
            writer.WriteLine($"|[{prefix}{subcmd.Name}]({mdFile})|{subcmd.Description}|");
        }
    }

    private void FormatExamples(CommandWithExamples cmd, TextWriter writer)
    {
        writer.WriteLine("### USAGE");
        foreach (var example in cmd.Examples)
        {
            writer.WriteLine($">{example.Title}");
            foreach (var cmdLine in example.CommandLines)
            {
                writer.WriteLine($"> * `  {cmdLine}`");
            }
        }

        writer.WriteLine();
    }
}

public static class Helpers
{
    public static string MdFile(IEnumerable<Command> hierarchy)
    {
        var builder = new StringBuilder();
        foreach (var cmd in hierarchy)
        {
            builder.Append(cmd.CapitalizedName());
        }

        builder.Append(".md");

        return builder.ToString();
    }
}

public static class CommandExtensions
{
    public static string CapitalizedName(this Command cmd)
    {
        var builder = new StringBuilder();
        builder.Append(cmd.Name.Substring(0, 1).ToUpper());
        builder.Append(cmd.Name.Substring(1));

        return builder.ToString();
    }
}
