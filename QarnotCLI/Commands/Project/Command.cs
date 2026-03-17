using System.CommandLine;

namespace QarnotCLI;

public class ProjectCommand : CommandWithExamples
{
    public ProjectCommand(GlobalOptions options, Func<GlobalModel, IProjectUseCases> factory)
        : base("project", "Project commands")
    {
        Add(BuildListCommand(options, factory));
    }

    private Command BuildListCommand(GlobalOptions options, Func<GlobalModel, IProjectUseCases> factory)
    {
        var cmd = new CommandWithExamples("list", "List available projects")
        {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] { "qarnot project list" }
            )
        };

        cmd.SetModelAction(
            model => factory(model).List(model),
            new GlobalBinder(options)
        );

        return cmd;
    }
}
