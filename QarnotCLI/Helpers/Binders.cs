using System.CommandLine;

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

    protected override GetPoolsOrTasksModel GetBoundValueImpl(ParseResult parseResult)
    {
        var model = new GetPoolsOrTasksModel(
            parseResult.GetValue(GetPoolsOrTasksOptions.NameOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.ShortnameOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.IdOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.TagsOpt) ?? new(),
            parseResult.GetValue(GetPoolsOrTasksOptions.ExclusiveTagsOpt) ?? new(),
            parseResult.GetValue(GetPoolsOrTasksOptions.NoPaginateOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.NextPageTokenOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.NextPageOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.MaxPageSizeOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.CreatedBeforeOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.CreatedAfterOpt),
            parseResult.GetValue(GetPoolsOrTasksOptions.NamePrefixOpt)
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

    protected override UpdatePoolsOrTasksConstantModel GetBoundValueImpl(ParseResult parseResult) =>
        new UpdatePoolsOrTasksConstantModel(
            parseResult.GetValue(ConstantNameOpt)!,
            parseResult.GetValue(ConstantValueOpt)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}

public class GetPoolOrTaskCarbonFactsBinder : GlobalBinder<GetCarbonFactsModel>
{
    private readonly Option<string?> ComparisonDatacenterName;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public GetPoolOrTaskCarbonFactsBinder(
        Option<string?> datacenterName,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        ComparisonDatacenterName = datacenterName;
        GetTasksOptions = getTasksOptions;
    }

    protected override GetCarbonFactsModel GetBoundValueImpl(ParseResult parseResult) =>
        new GetCarbonFactsModel(
            parseResult.GetValue(ComparisonDatacenterName)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}


public static class GetTasksModelExtension
{
    public static T BindGetPoolsOrTasksOptions<T>(this T model, ParseResult parseResult, GetPoolsOrTasksOptions opts)
        where T: GetPoolsOrTasksModel
        {
            model.Initialize(
                parseResult.GetValue(opts.NameOpt),
                parseResult.GetValue(opts.ShortnameOpt),
                parseResult.GetValue(opts.IdOpt),
                parseResult.GetValue(opts.TagsOpt) ?? new(),
                parseResult.GetValue(opts.ExclusiveTagsOpt) ?? new(),
                parseResult.GetValue(opts.NoPaginateOpt),
                parseResult.GetValue(opts.NextPageTokenOpt),
                parseResult.GetValue(opts.NextPageOpt),
                parseResult.GetValue(opts.MaxPageSizeOpt),
                parseResult.GetValue(opts.CreatedBeforeOpt),
                parseResult.GetValue(opts.CreatedAfterOpt),
                parseResult.GetValue(opts.NamePrefixOpt)
            );

            if (model.Tags.Any() && model.ExclusiveTags.Any())
            {
                throw new Exception("Only one of `--tags` or `--exclusive-tags` can be specified");
            }

            return model;
        }
}
