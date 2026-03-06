using System.CommandLine;

namespace QarnotCLI;

public class AllBinder : GlobalBinder<AllModel>
{
    private readonly Option<bool> DeleteOpt;
    private readonly Option<bool> ListOpt;
    private readonly Option<bool> AbortOpt;

    public AllBinder(
        Option<bool> deleteOpt,
        Option<bool> listOpt,
        Option<bool> abortOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        DeleteOpt = deleteOpt;
        ListOpt = listOpt;
        AbortOpt = abortOpt;
    }

    protected override AllModel GetBoundValueImpl(ParseResult parseResult)
    {
        var model = new AllModel(
            Delete: parseResult.GetValue(DeleteOpt),
            List: parseResult.GetValue(ListOpt),
            Abort: parseResult.GetValue(AbortOpt)
        );

        if ((model.Delete && (model.List || model.Abort))
                || (model.List && model.Abort))
        {
            throw new Exception("Only one of `--list`, `--abort` or `--delete` can be specified");
        }

        return model;
    }
}
