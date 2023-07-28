using System.CommandLine;
using System.CommandLine.Binding;

namespace QarnotCLI;

public class GetSecretBinder : GlobalBinder<GetSecretModel>
{
    private readonly Argument<string> KeyArg;

    public GetSecretBinder(
        Argument<string> keyArg,
        GlobalOptions globalOptions
    ) : base (globalOptions)
    {
        KeyArg = keyArg;
    }

    protected override GetSecretModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForArgument(KeyArg)
        );
}

public class WriteSecretBinder: GlobalBinder<WriteSecretModel>
{
    private readonly Argument<string> KeyArg;
    private readonly Argument<string> ValueArg;

    public WriteSecretBinder(
        Argument<string> keyArg,
        Argument<string> valueArg,
        GlobalOptions globalOptions
    ) : base (globalOptions)
    {
        KeyArg = keyArg;
        ValueArg = valueArg;
    }

    protected override WriteSecretModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForArgument(KeyArg),
            bindingContext.ParseResult.GetValueForArgument(ValueArg)
        );
}

public class ListSecretBinder: GlobalBinder<ListSecretsModel>
{
    private readonly Argument<string> PrefixArg;
    private readonly Option<bool> RecursiveOpt;

    public ListSecretBinder(
        Argument<string> prefixArg,
        Option<bool> recursiveOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        PrefixArg = prefixArg;
        RecursiveOpt = recursiveOpt;
    }

    protected override ListSecretsModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForArgument(PrefixArg),
            bindingContext.ParseResult.GetValueForOption(RecursiveOpt)
        );
}
