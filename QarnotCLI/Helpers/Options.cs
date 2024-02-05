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
        IdOpt = new Option<string>(
            aliases: new[] { "--id", "-i" },
            description: $"Short name or UUID of a {poolOrTask.Singular()}"
        );

        ShortnameOpt = new Option<string>(
            aliases: new[] { "--shortname" },
            description: $"Short name of a {poolOrTask.Singular()}"
        );

        NameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: $"Name of the {poolOrTask.Singular()}"
        );

        NoPaginateOpt = new Option<bool>(
            aliases: new[] { "--no-paginate" },
            description: $"No Pagination option for {poolOrTask.Plural()}, pages iteration will be done in the client",
            getDefaultValue: () => false
        );

        NextPageOpt = new Option<bool>(
            aliases: new[] { "--next-page" },
            description: $"Next page option for {poolOrTask.Plural()} pagination",
            getDefaultValue: () => false
        );

        MaxPageSizeOpt = new Option<int?>(
            aliases: new[] { "--max-page-size" },
            description: $"Max page size option for {poolOrTask.Plural()} pagination",
            getDefaultValue: () => null
        );

        NextPageTokenOpt = new Option<string>(
            aliases: new[] { "--next-page-token" },
            description: $"Provide the token to query next {poolOrTask.Plural()} page"
        );

        CreatedBeforeOpt = new Option<string>(
            aliases: new[] { "--created-before" },
            description: $"Filter {poolOrTask.Plural()} by creation date. Retrieve {poolOrTask.Plural()} created before the given date"
        );

        CreatedAfterOpt = new Option<string>(
            aliases: new[] { "--created-after" },
            description: $"Filter {poolOrTask.Plural()} by creation date. Retrieve {poolOrTask.Plural()} created after the given date"
        );

        NamePrefixOpt = new Option<string>(
            aliases: new[] { "--name-prefix" },
            description: $"Filter {poolOrTask.Plural()} by name prefix. Retrieve {poolOrTask.Plural()} with name starting with the given prefix"
        );

        TagsOpt = new Option<List<string>>(
            aliases: new[] { "--tags", "-t" },
            description: $"Filter {poolOrTask.Plural()} by tags. Retrieve {poolOrTask.Plural()} with any of the given tags"
        ) { AllowMultipleArgumentsPerToken = true };

        ExclusiveTagsOpt = new Option<List<string>>(
            name: "--exclusive-tags",
            description: $"Filter {poolOrTask.Plural()} by tags. Retrieve {poolOrTask.Plural()} with all of the given tags"
        ) { AllowMultipleArgumentsPerToken = true };
    }
}

public static class GetPoolsOrTasksOptionsExtension
{
    public static Command AddGetPoolsOrTasksOptions(this Command cmd, GetPoolsOrTasksOptions options)
    {
        cmd.AddOption(options.IdOpt);
        cmd.AddOption(options.NameOpt);
        cmd.AddOption(options.ShortnameOpt);
        cmd.AddOption(options.NoPaginateOpt);
        cmd.AddOption(options.NextPageTokenOpt);
        cmd.AddOption(options.NextPageOpt);
        cmd.AddOption(options.MaxPageSizeOpt);
        cmd.AddOption(options.TagsOpt);
        cmd.AddOption(options.ExclusiveTagsOpt);
        cmd.AddOption(options.CreatedAfterOpt);
        cmd.AddOption(options.CreatedBeforeOpt);
        cmd.AddOption(options.NamePrefixOpt);

        return cmd;
    }
}
