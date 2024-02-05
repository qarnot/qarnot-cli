using System.CommandLine;

namespace QarnotCLI;

public class HardwareConstraintsCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, IHardwareConstraintsUseCases> Factory;
    public HardwareConstraintsCommand(GlobalOptions options, Func<GlobalModel, IHardwareConstraintsUseCases> factory)
        : base("hw-constraints", "Hardware constraints commands")
    {
        GlobalOptions = options;
        Factory = factory;

        AddCommand(BuildListCommand());
    }

    private CommandWithExamples BuildListCommand()
    {
        var examples = new []{
            new Example(
                Title: "List hardware constraints",
                CommandLines: new[] {
                    "qarnot hw-constraints list"
                }
            )
        };

        var cmd = new CommandWithExamples("list", "List available hardware constraints");
        cmd.AddExamples(examples);

        cmd.SetHandler(
            model => Factory(model).List(model),
            new GlobalBinder(GlobalOptions)
        );

        return cmd;
    }
}
