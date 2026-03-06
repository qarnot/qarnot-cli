using System.CommandLine;
using Newtonsoft.Json;

namespace QarnotCLI;

public class GetJobBinder : GlobalBinder<GetJobModel>
{
    private readonly bool Strict;
    private readonly GetJobOptions GetJobOptions;

    public GetJobBinder(
        GetJobOptions getJobOptions,
        GlobalOptions globalOptions,
        bool strict = true
    )
        : base(globalOptions)
    {
        Strict = strict;
        GetJobOptions = getJobOptions;
    }

    protected override GetJobModel GetBoundValueImpl(ParseResult parseResult)
    {
        var model = new GetJobModel(
            All: parseResult.GetValue(GetJobOptions.AllOpt),
            Id: parseResult.GetValue(GetJobOptions.IdOpt),
            Name: parseResult.GetValue(GetJobOptions.NameOpt)
        );

        if (Strict)
        {

            if (!model.All && model.Id == null && model.Name == null)
            {
                throw new Exception("One of `--all`, `--id` or `--name` must be specified");
            }

            if (model.All && (!string.IsNullOrWhiteSpace(model.Id) || !string.IsNullOrWhiteSpace(model.Name))
                 || (!string.IsNullOrWhiteSpace(model.Id) && !string.IsNullOrWhiteSpace(model.Name)))
            {
                throw new Exception("Only one of `--all`, `--id` or `--name` must be specified");
            }
        }

        return model;
    }
}

public class CreateJobBinder : GlobalBinder<CreateJobModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<string> ShortnameOpt;
    private readonly Option<string> PoolOpt;
    private readonly Option<string> FileOpt;
    private readonly Option<bool?> UseDependenciesOpt;
    private readonly Option<string> MaxWallTimeOpt;

    public CreateJobBinder(
        Option<string> nameOpt,
        Option<string> shortnameOpt,
        Option<string> poolOpt,
        Option<string> fileOpt,
        Option<bool?> useDependenciesOpt,
        Option<string> maxWallTimeOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        ShortnameOpt = shortnameOpt;
        PoolOpt = poolOpt;
        FileOpt = fileOpt;
        UseDependenciesOpt = useDependenciesOpt;
        MaxWallTimeOpt = maxWallTimeOpt;
    }

    protected override CreateJobModel GetBoundValueImpl(ParseResult parseResult)
    {
        var file = parseResult.GetValue(FileOpt);
        var model = !string.IsNullOrWhiteSpace(file)
            ? JsonConvert.DeserializeObject<CreateJobModel>(File.ReadAllText(file))!
            : new CreateJobModel();

        TimeSpan? maxWallTime = null;
        var maxWallTimeStr = parseResult.GetValue(MaxWallTimeOpt) ?? model.MaxWallTimeStr;
        if (!string.IsNullOrEmpty(maxWallTimeStr))
        {
            try
            {
                maxWallTime = Helpers.ParseTimeSpanString(maxWallTimeStr);
            }
            catch (Exception)
            {
                throw new Exception("Maximum wall time has an invalid format");
            }
        }

        model = new CreateJobModel(
            Name: parseResult.GetValue(NameOpt) ?? model.Name,
            Shortname: parseResult.GetValue(ShortnameOpt) ?? model.Shortname,
            Pool: parseResult.GetValue(PoolOpt) ?? model.Pool,
            UseDependencies: parseResult.GetValue(UseDependenciesOpt) ?? model.UseDependencies,
            MaxWallTimeStr: maxWallTimeStr,
            MaxWallTime: maxWallTime
        );

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new Exception("A name must be given to the job");
        }

        return model;
    }
}
