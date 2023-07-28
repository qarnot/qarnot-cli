using System.CommandLine;

namespace QarnotCLI;

public class AllCommand : CommandWithExamples
{
    public AllCommand(GlobalOptions options, Func<GlobalModel, IAllUseCases> factory)
        : base("all", "List, abort or delete all the objects (tasks, jobs, pools, buckets) from your profile")
    {
        var examples = new Example[] {
            new Example(
                Title: "Delete all the tasks, jobs, pools and buckets",
                CommandLines: new[] { "qarnot all --delete" }
            ),
            new Example(
                Title: "Terminate all the tasks and jobs",
                CommandLines: new[] { "qarnot all --abort" }
            ),
            new Example(
                Title: "List all the tasks, jobs, pools and buckets",
                CommandLines: new[] { "qarnot all --list" }
            ),
            new Example(
                Title: "Default: list all the thet tasks, jobs, pools and buckets",
                CommandLines: new[] { "qarnot all" }
            ),
            new Example(
                Title: "Error: only one rule can be set at a time",
                CommandLines: new[] { "qarnot all --abort --delete --list" },
                IsError: true
            )
        };

        var deleteOpt = new Option<bool>(
            aliases: new[] { "--delete", "-d" },
            description: "Delete all the tasks, pools, jobs and buckets"
        );

        var abortOpt = new Option<bool>(
            aliases: new[] { "--abort", "-a" },
            description: "Abort all the tasks and jobs"
        );

        var listOpt = new Option<bool>(
            aliases: new[] { "--list", "-l" },
            description: "List all the tasks, pools, jobs and buckets"
        );

        AddExamples(examples);
        AddOption(deleteOpt);
        AddOption(abortOpt);
        AddOption(listOpt);

        this.SetHandler(
            async model => {
                var useCases = factory(model);
                if (model.Delete)
                {
                    await useCases.Delete(model);
                }
                else if (model.Abort)
                {
                    await useCases.Abort(model);
                }
                else
                {
                    await useCases.List(model);
                }
            },
            new AllBinder(
                deleteOpt: deleteOpt,
                abortOpt: abortOpt,
                listOpt: listOpt,
                globalOptions: options
            )
        );
    }
}
