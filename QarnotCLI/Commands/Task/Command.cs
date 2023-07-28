using System.CommandLine;

namespace QarnotCLI;

public class TaskCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, ITaskUseCases> Factory;

    public TaskCommand(GlobalOptions globalOptions, Func<GlobalModel, ITaskUseCases> factory)
        : base("task", "Task commands")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        AddCommand(BuildCreateCommand());
        AddCommand(BuildListCommand());
        AddCommand(BuildInfoCommand());
        AddCommand(BuildWaitCommand());
        AddCommand(BuildAbortCommand());
        AddCommand(BuildDeleteCommand());
        AddCommand(BuildUpdateResourcesCommand());
        AddCommand(BuildUpdateConstantCommand());
        AddCommand(BuildSnapshotCommand());
        AddCommand(BuildStdoutCommand());
        AddCommand(BuildStderrCommand());
    }

    private Command BuildCreateCommand()
    {
        var examples = new Example[] {
            new(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot task create --constants \"DOCKER_CMD=echo hello world\" --instance 4 --name \"Task name\" --profile docker-batch",
                }
            ),
            new(
                Title: "Usage with a set of constants",
                CommandLines: new[] {
                    "qarnot task create --constants \"DOCKER_CMD=echo hello world\" DOCKER_REPO=library/ubuntu DOCKER_TAG=latest --instance 4 --name \"Task name\" --profile docker-batch"
                }
            ),
            new(
                Title: "File config usage (see documentation)",
                CommandLines: new[] {
                    "qarnot task create --file FileName.json"
                },
                IgnoreTest: true
            ),
            new(
                Title: "Error: missing instance count",
                CommandLines: new[] {
                    "qarnot task create --constants \"DOCKER_CMD=echo hello world\" --name \"Task name\" --pool POOL-UUID"
                },
                IsError: true
            )
        };

        var jobOpt = new Option<string>(
            name: "--job",
            description: "UUID or short name of the job the task should be attached to"
        );

        var poolOpt = new Option<string>(
            name: "--pool",
            description: "UUID or short name of the pool the task should be attached to"
        );

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the task"
        );

        var shortNameOpt = new Option<string>(
            aliases: new[] { "--shortname", "-s" },
            description: "Short name of the task"
        );

        var profileOpt = new Option<string>(
            aliases: new[] { "--profile", "-p" },
            description: "Name of the profile used for the task"
        );

        var rangeOpt = new Option<string>(
            name: "--range",
            description: "Range of the task"
        );

        var instanceOpt = new Option<uint?>(
            aliases: new[] { "--instance", "-i" },
            description: "Instance count of the task"
        );

        var fileOpt = new Option<string>(
            aliases: new[] { "--file", "-f" },
            description: "File with a json configuration of the task. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"TaskName\",\"Profile\": \"docker-batch\",\"Constants\": [ \"DOCKER_CMD=echo hello world\", ],\"InstanceCount\": 1}' > CreateTask.json)"
        );

        var tagsOpt = new Option<List<string>>(
            aliases: new[] { "--tags", "-t" },
            description: "Tags of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var constantsOpt = new Option<List<string>>(
            aliases: new[] { "--constants", "-c" },
            description: "Constants of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var constraintsOpt = new Option<List<string>>(
            name: "--constraints",
            description: "Constraints of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var labelsOpt = new Option<List<string>>(
            name: "--labels",
            description: "Labels of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var resourcesOpt = new Option<List<string>>(
            aliases: new[] { "--resources", "-r" },
            description: "Name of the buckets of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var resultOpt = new Option<string>(
            name: "--result",
            description: "Name of result bucket of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var waitForResourcesSynchronizationOpt = new Option<bool?>(
            name: "--wait-for-resources-synchronization",
            description: "Wait for the pool resources to synchronized before launching the task"
        );

        var maxTotalRetriesOpt = new Option<uint?>(
            name: "--max-total-retries",
            description: "Total number of times the task can have its instances retried in case of failure"
        );

        var maxRetriesPerInstanceOpt = new Option<uint?>(
            name: "--max-retries-per-instance",
            description: "Total number of times each task instance will be allowed to retry in case of failure"
        );

        var dependentsOpt = new Option<List<string>>(
            aliases: new[] { "--dependents", "-d" },
            description: "List of UUID the task need to wait before start running.(must be use with a job with \"is-dependent\" set)"
        ) { AllowMultipleArgumentsPerToken = true };

        var ttlOpt = new Option<uint?>(
            name: "--ttl",
            description: "Default TTL for the task resources cache (in seconds)"
        );

        var secretsAccessRightsByKeyOpt = new Option<List<string>>(
            name: "--secrets-access-rights-by-key",
            description: "Give the task access to secrets described by their keys. Only available to standalone task, use `--secrets-access-rights-by-key` on the pool for tasks running within a pool"
        ) { AllowMultipleArgumentsPerToken = true };

        var secretsAccessRightsByPrefixOpt = new Option<List<string>>(
            name: "--secrets-access-rights-by-prefix",
            description: "Give the task access to secrets described by their prefixs. Only available to standalone task, use `--secrets-access-rights-by-prefix` on the pool for tasks running within a pool"
        ) { AllowMultipleArgumentsPerToken = true };
;

        var schedulingTypeOpt = new Option<string>(
            name: "--scheduling-type",
            description: "Specify the type of scheduling used for the task"
        );

        var machineTargetOpt = new Option<string>(
            name: "--machine-target",
            description: "Available only for 'Reserved' scheduling. Specify the reserved machine on which the task should run"
        );

        var periodicOpt = new Option<uint?>(
            name: "--periodic",
            description: "Periodic time, in seconds, to synchronize the task files to the output bucket"
        );

        var whitelistOpt = new Option<string>(
            name: "--whitelist",
            description: "Whitelist of task files to be synchronized to the output bucket"
        );

        var blacklistOpt = new Option<string>(
            name:  "--blacklist",
            description: "Blacklist of task files to synchronize to the output bucket"
        );

        var exportCredentialsToEnvOpt = new Option<bool?>(
            name: "--export-credentials-to-env",
            description: "Activate the exportation of the api and storage credentials to the task environment (default is false)"
        );

        var cmd = new CommandWithExamples("create", "Create and launch new task")
        {
            examples,

            nameOpt,
            instanceOpt,

            jobOpt,
            poolOpt,
            shortNameOpt,
            profileOpt,
            rangeOpt,
            fileOpt,
            tagsOpt,
            constantsOpt,
            constraintsOpt,
            labelsOpt,
            resourcesOpt,
            resultOpt,
            waitForResourcesSynchronizationOpt,
            maxTotalRetriesOpt,
            maxRetriesPerInstanceOpt,
            dependentsOpt,
            ttlOpt,
            secretsAccessRightsByKeyOpt,
            secretsAccessRightsByPrefixOpt,
            schedulingTypeOpt,
            machineTargetOpt,
            periodicOpt,
            whitelistOpt,
            blacklistOpt,
            exportCredentialsToEnvOpt,
        };

        cmd.SetHandler(
            model => Factory(model).Create(model),
            new CreateTaskBinder(
                jobOpt,
                poolOpt,
                nameOpt,
                shortNameOpt,
                profileOpt,
                rangeOpt,
                instanceOpt,
                fileOpt,
                tagsOpt,
                constantsOpt,
                constraintsOpt,
                labelsOpt,
                resourcesOpt,
                resultOpt,
                waitForResourcesSynchronizationOpt,
                maxTotalRetriesOpt,
                maxRetriesPerInstanceOpt,
                dependentsOpt,
                ttlOpt,
                secretsAccessRightsByKeyOpt,
                secretsAccessRightsByPrefixOpt,
                schedulingTypeOpt,
                machineTargetOpt,
                periodicOpt,
                whitelistOpt,
                blacklistOpt,
                exportCredentialsToEnvOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildListCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new string[] {
                "qarnot task list --name \"Task name\" --tags TAG1 TAG2",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples("list", "List the running tasks")
        {
            example,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).List(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildInfoCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task info --name \"Task name\" --tags TAG1 TAG2",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples("info", "Detailed info on a task")
        {
            example,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);


        cmd.SetHandler(
            model => Factory(model).Info(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildWaitCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                  "qarnot task wait --name \"Task name\" --tags TAG1 TAG2",
                }
            ),
            new Example(
                Title: "Print STDOUT and STDERR while waiting",
                CommandLines: new[] {
                  "qarnot task wait --stderr --name=\"Task name\" --stdout --tags=TAG1 TAG2",
                  "qarnot task wait -e -n \"Task name\" -o -t TAG1 TAG2"
                }
            ),
        };

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var stdoutOpt = new Option<bool>(
            aliases: new[] { "--stdout", "-o" },
            description: "Print STDOUT events while waiting"
        );

        var stderrOpt = new Option<bool>(
            aliases: new[] { "--stderr", "-e" },
            description: "Print STDERR events while waiting"
        );

        var cmd = new CommandWithExamples("wait", "Wait for the end of a task")
        {
            examples,
            stdoutOpt,
            stderrOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Wait(model),
            new WaitTasksBinder(
                stdoutOpt,
                stderrOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildAbortCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task abort --name \"Task name\" --tags TAG1 TAG2",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples("abort", "Terminate a task")
        {
            example
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Abort(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildDeleteCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task delete --name \"Task name\" --tags TAG1 TAG2",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples("delete", "Delete a task")
        {
            example
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Delete(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildUpdateResourcesCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task update-resources --name \"Task name\" --tags TAG1 TAG2",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples("update-resources", "Update resources for a running task")
        {
            example
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).UpdateResources(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildUpdateConstantCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task update-constant --constant-name QARNOT_SECRET__SUPER_TOKEN --constant-value new-token --id TaskID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var constantNameOpt = new Option<string>(
            name: "--constant-name",
            description: "Name of the constant to update"
        ) { IsRequired = true };

        var constantValueOpt = new Option<string>(
            name: "--constant-value",
            description: "New value for the constant to update"
        );

        var cmd = new CommandWithExamples("update-constant", "Update constant of a running task")
        {
            example,
            constantNameOpt,
            constantValueOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).UpdateConstant(model),
            new UpdatePoolsOrTasksConstantBinder(
                constantNameOpt,
                constantValueOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSnapshotCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
              "qarnot task snapshot --id TaskID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var periodicOpt = new Option<uint>(
            name: "--periodic",
            description: "Periodic time, in seconds, to synchronize the task files to the output bucket"
        );

        var whitelistOpt = new Option<string>(
            name: "--whitelist",
            description: "Whitelist of task files to be synchronized to the output bucket"
        );

        var blacklistOpt = new Option<string>(
            name:  "--blacklist",
            description: "Blacklist of task files to synchronize to the output bucket"
        );

        var bucketNameOpt = new Option<string>(
            name:  "--bucket",
            description: "Name of the output bucket used for the snapshot"
        );

        var cmd = new CommandWithExamples(
            "snapshot",
            "Trigger a snapshot: request to upload a version of the running task files into the output bucket"
        )
        {
            example,
            periodicOpt,
            whitelistOpt,
            blacklistOpt,
            bucketNameOpt
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Snapshot(model),
            new SnapshotTaskBinder(
                periodicOpt,
                whitelistOpt,
                blacklistOpt,
                bucketNameOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildStdoutCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Task stdout",
                CommandLines: new[] {
                  "qarnot task stdout --name \"Task name\"",
                }
            ),
            new Example(
                Title: "Task instance stdout",
                CommandLines: new[] {
                  "qarnot task stdout --fresh --instance-id=0 --name=\"Task name\"",
                }
            )
        };

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var instanceIdOpt = new Option<uint?>(
            name: "--instance-id",
            description: "Get the stdout of a specific instance"
        );

        var freshOpt = new Option<bool>(
            aliases: new[] { "--fresh", "-f" },
            description: "Get the last stdout dump"
        );

        var cmd = new CommandWithExamples("stdout", "Get the stdout of a task")
        {
            examples,
            instanceIdOpt,
            freshOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Stdout(model),
            new GetTasksOutputBinder(
                instanceIdOpt,
                freshOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildStderrCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Task stderr",
                CommandLines: new[] {
                  "qarnot task stderr --name \"Task name\"",
                }
            ),
            new Example(
                Title: "Task instance stderr",
                CommandLines: new[] {
                  "qarnot task stderr --fresh --instance-id=0 --name=\"Task name\"",
                }
            )
        };

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var instanceIdOpt = new Option<uint?>(
            name: "--instance-id",
            description: "Get the stderr of a specific instance"
        );

        var freshOpt = new Option<bool>(
            aliases: new[] { "--fresh", "-f" },
            description: "Get the last stderr dump"
        );

        var cmd = new CommandWithExamples("stderr", "Get the stderr of a task")
        {
            examples,
            instanceIdOpt,
            freshOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetHandler(
            model => Factory(model).Stderr(model),
            new GetTasksOutputBinder(
                instanceIdOpt,
                freshOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }
}
