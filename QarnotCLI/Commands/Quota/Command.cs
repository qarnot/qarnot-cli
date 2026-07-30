using System.CommandLine;

namespace QarnotCLI;

public class QuotaCommand : Command
{
    public QuotaCommand(GlobalOptions globalOptions, Func<GlobalModel, IQuotaUseCases> factory)
        : base("quota", "Quota commands")
    {
        Add(new ComputingQuotaCommand(globalOptions, factory));
    }
}

public class ComputingQuotaCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, IQuotaUseCases> Factory;

    public ComputingQuotaCommand(GlobalOptions globalOptions, Func<GlobalModel, IQuotaUseCases> factory)
        : base("computing", "Computing quota commands")
    {
        GlobalOptions = globalOptions;
        Factory = factory;

        Add(BuildScopeCommand("user", "Get your own computing quotas usage"));
        Add(BuildScopeCommand("organization", "Get your organization's computing quotas usage"));
    }

    private Command BuildScopeCommand(string scope, string description)
    {
        var examples = new[] {
            new Example(
                Title: description,
                CommandLines: new[] { $"qarnot quota computing {scope}" }
            ),
        };

        var cmd = new CommandWithExamples(scope, description)
        {
            examples,
        };

        cmd.SetModelAction(
            model => Factory(model).Get(model),
            new GetQuotaBinder(
                scope,
                GlobalOptions
            )
        );

        return cmd;
    }
}
