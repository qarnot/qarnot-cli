using System.CommandLine;

namespace QarnotCLI;

public class JobCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, IJobUseCases> Factory;

    public JobCommand(GlobalOptions options, Func<GlobalModel, IJobUseCases> factory)
        : base("job", "Create and launch a new job")
    {
        Factory = factory;
        GlobalOptions = options;

        Add(BuildCreateCommand());
        Add(BuildListCommand());
        Add(BuildInfoCommand());
        Add(BuildAbortCommand());
        Add(BuildDeleteCommand());
    }

    private Command BuildDeleteCommand()
    {
        var getJobOptions = new GetJobOptions();
        var cmd = new CommandWithExamples("delete", "Delete the selected jobs")
            .AddGetJobOptions(getJobOptions)
            .AddGetJobExamples("delete", "Delete");

        cmd.SetModelAction(
            model => Factory(model).Delete(model),
            new GetJobBinder(
                getJobOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildAbortCommand()
    {
        var getJobOptions = new GetJobOptions();
        var cmd = new CommandWithExamples("abort", "Terminate the selected jobs")
            .AddGetJobOptions(getJobOptions)
            .AddGetJobExamples("abort", "Abort");

        cmd.SetModelAction(
            model => Factory(model).Abort(model),
            new GetJobBinder(
                getJobOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildListCommand()
    {
        var getJobOptions = new GetJobOptions();
        var cmd = new CommandWithExamples("list", "List running jobs")
            .AddGetJobOptions(getJobOptions)
            .AddGetJobExamples("list", "List");

        cmd.SetModelAction(
            model => Factory(model).List(model),
            new GetJobBinder(
                getJobOptions,
                GlobalOptions,
                strict: false
            )
        );

        return cmd;
    }

    private Command BuildInfoCommand()
    {
        var getJobOptions = new GetJobOptions();
        var cmd = new CommandWithExamples("info", "Detailed info on running jobs")
                .AddGetJobOptions(getJobOptions)
                .AddGetJobExamples("info", "Get detailed info on");

        cmd.SetModelAction(
            model => Factory(model).Info(model),
            new GetJobBinder(
                getJobOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildCreateCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot job create --name \"Job name\""
                }
            ),
            new Example(
                Title: "Configure from a file",
                CommandLines: new[] {
                    "qarnot job create --file FileName.json"
                },
                IgnoreTest: true
            ),
            new Example(
                Title: "Error: missing name",
                CommandLines: new[] {
                    "qarnot job  create"
                },
                IsError: true
            )
        };

        var nameOpt = new Option<string>("--name", "-n")
        {
            Description = "Name of the job",
        };

        var shortnameOpt = new Option<string>("--shortname", "-s")
        {
            Description = "Short name of the job",
        };

        var fileOpt = new Option<string>("--file", "-f")
        {
            Description = "File with a json configuration of the job. (example : echo '{\"UseDependencies\":true, \"Shortname\": \"SN\",\"Name\": \"JobName\" }' > CreateJob.json)",
        };

        var useDependenciesOpt = new Option<bool?>("--use-dependencies", "-d")
        {
            Description = "Job can have jobs depending on other ones to run",
        };

        var maxWallTimeOpt = new Option<string>("--max-wall-time")
        {
            Description = "Wall time limit for the job execution. Once this time duration exceeded, the whole job will terminate. The wall time format can be a date in the 'yyyy/MM/dd HH:mm:ss', 'yyyy/MM/dd' date format or a TimeStamp format 'd', 'd.hh', 'd.hh:mm', 'd.hh:mm:ss', 'hh:mm', 'hh:mm:ss'",
        };

        var poolOpt = new Option<string>("--pool")
        {
            Description = "UUID or shortname of the pool to attach the job to",
        };

        var cmd = new CommandWithExamples("create", "Create and launch a new job")
        {
            examples,
            nameOpt,
            shortnameOpt,
            poolOpt,
            useDependenciesOpt,
            maxWallTimeOpt,
            fileOpt
        };

        cmd.SetModelAction(
            model => Factory(model).Create(model),
            new CreateJobBinder(
                nameOpt,
                shortnameOpt,
                poolOpt,
                fileOpt,
                useDependenciesOpt,
                maxWallTimeOpt,
                GlobalOptions
            )
        );

        return cmd;
    }
}
