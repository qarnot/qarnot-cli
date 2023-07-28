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
    public Option<List<string>> TagsOpt { get; }
    public Option<List<string>> ExclusiveTagsOpt { get; }

    public GetPoolsOrTasksOptions(PoolOrTask poolOrTask)
    {
        IdOpt = new Option<string>(
            aliases: new[] { "--id", "-i" },
            description: $"Short name or UUID of a {poolOrTask.Singular()}"
        );

        NameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: $"Name of the {poolOrTask.Singular()}"
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
        cmd.AddOption(options.TagsOpt);
        cmd.AddOption(options.ExclusiveTagsOpt);

        return cmd;
    }
}
