using System.CommandLine.Builder;
using System.CommandLine.Help;

namespace QarnotCLI;

public static class HelpBuilderExtensions
{
    public static CommandLineBuilder UseCustomHelp(this CommandLineBuilder builder, AssemblyDetails details) =>
        builder
            // Exclude "-h" as it's used for the "human readable" option.
            .UseHelp("--help")
            .UseHelp(ctx => {
                ctx.HelpBuilder.UseCustomLayout(details);
            });

    // WARNING: current implementation erases all previous modification,
    // that might have been made to the layout. Use this as the first
    // extension method if you want to use multiple ones.
    public static HelpBuilder UseCustomLayout(this HelpBuilder helpBuilder, AssemblyDetails details)
    {
        helpBuilder.CustomizeLayout(ctx => new List<HelpSectionDelegate>
        {
            _ => ctx.Output.WriteLine(details),
            HelpBuilder.Default.SynopsisSection(),
            HelpBuilder.Default.CommandUsageSection(),
            _ => {
                if (!ctx.Command.IsHidden && ctx.Command is CommandWithExamples cmd)
                {
                    foreach (var example in cmd.Examples)
                    {
                        ctx.Output.WriteLine(example.Title + ":");
                        foreach (var line in example.CommandLines)
                        {
                            ctx.Output.WriteLine("  " + line);
                        }
                    }
                }
            },
            HelpBuilder.Default.CommandArgumentsSection(),
            HelpBuilder.Default.OptionsSection(),
            HelpBuilder.Default.SubcommandsSection(),
            HelpBuilder.Default.AdditionalArgumentsSection(),
        });

        return helpBuilder;
    }
}
