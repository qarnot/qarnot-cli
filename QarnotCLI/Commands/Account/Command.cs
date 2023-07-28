using System.CommandLine;

namespace QarnotCLI;

public class AccountCommand : CommandWithExamples
{
    public AccountCommand(GlobalOptions options, Func<GlobalModel, IAccountUseCases> factory)
        : base("account", "Account commands")
    {
        AddExample(new Example(
            Title: "Regular usage",
            CommandLines: new[] { "qarnot account" }
        ));

        this.SetHandler(
            model => factory(model).Get(model),
            new GlobalBinder(options)
        );
    }
}
