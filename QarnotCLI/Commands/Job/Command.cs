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

        AddCommand(BuildCreateCommand());
        AddCommand(BuildListCommand());
        AddCommand(BuildInfoCommand());
        AddCommand(BuildAbortCommand());
        AddCommand(BuildDeleteCommand());
    }

    private Command BuildDeleteCommand()
    {
        var getJobOptions = new GetJobOptions();
        var cmd = new CommandWithExamples("delete", "Delete the selected jobs")
            .AddGetJobOptions(getJobOptions)
            .AddGetJobExamples("delete", "Delete");

        cmd.SetHandler(
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

        cmd.SetHandler(
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

        cmd.SetHandler(
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

        cmd.SetHandler(
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

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the job"
        );

        var shortnameOpt = new Option<string>(
            aliases: new[] { "--shortname", "-s" },
            description: "Short name of the job"
        );

        var fileOpt = new Option<string>(
            aliases: new[] { "--file", "-f" },
            description: "File with a json configuration of the job. (example : echo '{\"IsDependents\":true, \"Shortname\": \"SN\",\"Name\": \"JobName\" }' > CreateJob.json)"
        );

        var isDependentOpt = new Option<bool?>(
            aliases: new[] { "--is-dependant", "-d" },
            description: "Job can have jobs depending on other ones to run"
        );

        var maxWallTimeOpt = new Option<string>(
            name: "--max-wall-time",
            description: "Wall time limit for the job execution. Once this time duration exceeded, the whole job will terminate. The wall time format can be a date in the 'yyyy/MM/dd HH:mm:ss', 'yyyy/MM/dd' date format or a TimeStamp format 'd', 'd.hh', 'd.hh:mm', 'd.hh:mm:ss', 'hh:mm', 'hh:mm:ss'"
        );

        var poolOpt = new Option<string>(
            name: "--pool",
            description: "UUID or shortname of the pool to attach the job to"
        );

        var cmd = new CommandWithExamples("create", "Create and launch a new job")
        {
            examples,
            nameOpt,
            shortnameOpt,
            poolOpt,
            isDependentOpt,
            maxWallTimeOpt,
            fileOpt
        };

        cmd.SetHandler(
            model => Factory(model).Create(model),
            new CreateJobBinder(
                nameOpt,
                shortnameOpt,
                poolOpt,
                fileOpt,
                isDependentOpt,
                maxWallTimeOpt,
                GlobalOptions
            )
        );

        return cmd;
    }
}
