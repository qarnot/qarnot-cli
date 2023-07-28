using System.CommandLine;

namespace QarnotCLI;

public class GetJobOptions
{
    public Option<string> IdOpt { get; }
    public Option<string> NameOpt { get; }
    public Option<bool> AllOpt { get; }

    public GetJobOptions()
    {
        NameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the job"
        );

        IdOpt = new Option<string>(
            aliases: new[] { "--id", "-i" },
            description: "Shortname or UUID of the job"
        );

        AllOpt = new Option<bool>(
            aliases: new[] { "--all", "-a" },
            description: "All the jobs"
        );
    }
}

public static class GetJobOptionsExtension
{
    public static T AddGetJobOptions<T>(this T cmd, GetJobOptions options)
        where T: Command
    {
        cmd.AddOption(options.IdOpt);
        cmd.AddOption(options.NameOpt);
        cmd.AddOption(options.AllOpt);

        return cmd;
    }

    public static Command AddGetJobExamples(this CommandWithExamples cmd, string verb, string pretty)
    {
        var examples = new [] {
            new Example(
                Title: $"{pretty} all jobs",
                CommandLines: new[] { $"qarnot job {verb} --all" }
            ),
            new Example(
                Title: $"{pretty} the job with a specific name",
                CommandLines: new[] { $"qarnot job {verb} -n JOB_NAME" }
            ),
            new Example(
                Title: $"{pretty} the job with a specific ID",
                CommandLines: new[] { $"qarnot job {verb} -i JOB_ID" }
            )
        };

        cmd.AddExamples(examples);

        return cmd;
    }
}
