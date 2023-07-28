using System.CommandLine;
using System.CommandLine.Binding;

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

    protected override AllModel GetBoundValueImpl(BindingContext bindingContext)
    {
        var model = new AllModel(
            Delete: bindingContext.ParseResult.GetValueForOption(DeleteOpt),
            List: bindingContext.ParseResult.GetValueForOption(ListOpt),
            Abort: bindingContext.ParseResult.GetValueForOption(AbortOpt)
        );

        if ((model.Delete && (model.List || model.Abort))
                || (model.List && model.Abort))
        {
            throw new Exception("Only one of `--list`, `--abort` or `--delete` can be specified");
        }

        return model;
    }
}
