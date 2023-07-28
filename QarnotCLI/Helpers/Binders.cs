using System.CommandLine;
using System.CommandLine.Binding;

namespace QarnotCLI;

public class GetPoolsOrTasksBinder : GlobalBinder<GetPoolsOrTasksModel>
{
    private readonly GetPoolsOrTasksOptions GetPoolsOrTasksOptions;

    public GetPoolsOrTasksBinder(
        GetPoolsOrTasksOptions getPoolsOrTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        GetPoolsOrTasksOptions = getPoolsOrTasksOptions;
    }

    protected override GetPoolsOrTasksModel GetBoundValueImpl(BindingContext bindingContext)
    {
        var model = new GetPoolsOrTasksModel(
            bindingContext.ParseResult.GetValueForOption(GetPoolsOrTasksOptions.NameOpt),
            bindingContext.ParseResult.GetValueForOption(GetPoolsOrTasksOptions.IdOpt),
            bindingContext.ParseResult.GetValueForOption(GetPoolsOrTasksOptions.TagsOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(GetPoolsOrTasksOptions.ExclusiveTagsOpt) ?? new()
        );

        if (model.Tags.Any() && model.ExclusiveTags.Any())
        {
            throw new Exception("Only one of `--tags` or `--exclusive-tags` can be specified");
        }

        return model;
    }
}

public class UpdatePoolsOrTasksConstantBinder : GlobalBinder<UpdatePoolsOrTasksConstantModel>
{
    private readonly Option<string> ConstantNameOpt;
    private readonly Option<string> ConstantValueOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public UpdatePoolsOrTasksConstantBinder(
        Option<string> constantNameOpt,
        Option<string> constantValueOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        ConstantNameOpt = constantNameOpt;
        ConstantValueOpt = constantValueOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override UpdatePoolsOrTasksConstantModel GetBoundValueImpl(BindingContext bindingContext) =>
        new UpdatePoolsOrTasksConstantModel(
            bindingContext.ParseResult.GetValueForOption(ConstantNameOpt)!,
            bindingContext.ParseResult.GetValueForOption(ConstantValueOpt)
        ).BindGetPoolsOrTasksOptions(bindingContext, GetTasksOptions);
}


public static class GetTasksModelExtension
{
    public static T BindGetPoolsOrTasksOptions<T>(this T model, BindingContext bindingContext, GetPoolsOrTasksOptions opts)
        where T: GetPoolsOrTasksModel
        {
            model.Initialize(
                bindingContext.ParseResult.GetValueForOption(opts.NameOpt),
                bindingContext.ParseResult.GetValueForOption(opts.IdOpt),
                bindingContext.ParseResult.GetValueForOption(opts.TagsOpt) ?? new(),
                bindingContext.ParseResult.GetValueForOption(opts.ExclusiveTagsOpt) ?? new()
            );

            if (model.Tags.Any() && model.ExclusiveTags.Any())
            {
                throw new Exception("Only one of `--tags` or `--exclusive-tags` can be specified");
            }

            return model;
        }
}
