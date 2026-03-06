using System.CommandLine;

namespace QarnotCLI;

public enum PoolOrTask
{
    Pool,
    Task,
};

public static class PoolOrTaskExtension
{
    public static string Singular(this PoolOrTask pOrt) =>
        pOrt switch {
            PoolOrTask.Pool => "pool",
            PoolOrTask.Task => "task",
            _               => throw new Exception(),
        };

    public static string Plural(this PoolOrTask pOrt) =>
        pOrt switch {
            PoolOrTask.Pool => "pools",
            PoolOrTask.Task => "tasks",
            _               => throw new Exception(),
        };
}

public class GetPoolsOrTasksOptions
{
    public Option<string> IdOpt { get; }
    public Option<string> NameOpt { get; }
    public Option<string> ShortnameOpt { get; }
    public Option<string> NextPageTokenOpt { get; }
    public Option<bool> NextPageOpt { get; }
    public Option<int?> MaxPageSizeOpt { get; }
    public Option<bool> NoPaginateOpt { get; }
    public Option<string> CreatedBeforeOpt { get; }
    public Option<string> CreatedAfterOpt { get; }
    public Option<string> NamePrefixOpt { get; }
    public Option<List<string>> TagsOpt { get; }
    public Option<List<string>> ExclusiveTagsOpt { get; }

    public GetPoolsOrTasksOptions(PoolOrTask poolOrTask)
    {
        IdOpt = new Option<string>("--id", "-i")
        {
            Description = $"Short name or UUID of a {poolOrTask.Singular()}",
        };

        ShortnameOpt = new Option<string>("--shortname")
        {
            Description = $"Short name of a {poolOrTask.Singular()}",
        };

        NameOpt = new Option<string>("--name", "-n")
        {
            Description = $"Name of the {poolOrTask.Singular()}",
        };

        NoPaginateOpt = new Option<bool>("--no-paginate")
        {
            Description = $"No Pagination option for {poolOrTask.Plural()}, pages iteration will be done in the client",
            DefaultValueFactory = _ => false,
        };

        NextPageOpt = new Option<bool>("--next-page")
        {
            Description = $"Next page option for {poolOrTask.Plural()} pagination",
            DefaultValueFactory = _ => false,
        };

        MaxPageSizeOpt = new Option<int?>("--max-page-size")
        {
            Description = $"Max page size option for {poolOrTask.Plural()} pagination",
            DefaultValueFactory = _ => null,
        };

        NextPageTokenOpt = new Option<string>("--next-page-token")
        {
            Description = $"Provide the token to query next {poolOrTask.Plural()} page",
        };

        CreatedBeforeOpt = new Option<string>("--created-before")
        {
            Description = $"Filter {poolOrTask.Plural()} by creation date. Retrieve {poolOrTask.Plural()} created before the given date",
        };

        CreatedAfterOpt = new Option<string>("--created-after")
        {
            Description = $"Filter {poolOrTask.Plural()} by creation date. Retrieve {poolOrTask.Plural()} created after the given date",
        };

        NamePrefixOpt = new Option<string>("--name-prefix")
        {
            Description = $"Filter {poolOrTask.Plural()} by name prefix. Retrieve {poolOrTask.Plural()} with name starting with the given prefix",
        };

        TagsOpt = new Option<List<string>>("--tags", "-t")
        {
            Description = $"Filter {poolOrTask.Plural()} by tags. Retrieve {poolOrTask.Plural()} with any of the given tags", AllowMultipleArgumentsPerToken = true,
        };

        ExclusiveTagsOpt = new Option<List<string>>("--exclusive-tags")
        {
            Description = $"Filter {poolOrTask.Plural()} by tags. Retrieve {poolOrTask.Plural()} with all of the given tags", AllowMultipleArgumentsPerToken = true,
        };
    }
}

public static class GetPoolsOrTasksOptionsExtension
{
    public static Command AddGetPoolsOrTasksOptions(this Command cmd, GetPoolsOrTasksOptions options)
    {
        cmd.Add(options.IdOpt);
        cmd.Add(options.NameOpt);
        cmd.Add(options.ShortnameOpt);
        cmd.Add(options.NoPaginateOpt);
        cmd.Add(options.NextPageTokenOpt);
        cmd.Add(options.NextPageOpt);
        cmd.Add(options.MaxPageSizeOpt);
        cmd.Add(options.TagsOpt);
        cmd.Add(options.ExclusiveTagsOpt);
        cmd.Add(options.CreatedAfterOpt);
        cmd.Add(options.CreatedBeforeOpt);
        cmd.Add(options.NamePrefixOpt);

        return cmd;
    }
}
