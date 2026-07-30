using System.CommandLine;

namespace QarnotCLI;

public class GetQuotaBinder : GlobalBinder<GetQuotaModel>
{
    private readonly string Scope;

    public GetQuotaBinder(
        string scope,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        Scope = scope;
    }

    protected override GetQuotaModel GetBoundValueImpl(ParseResult parseResult) =>
        new(Scope);
}
