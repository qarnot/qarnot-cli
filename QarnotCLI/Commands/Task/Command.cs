using System.CommandLine;

namespace QarnotCLI;

public class TaskCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, ITaskUseCases> Factory;

    public TaskCommand(
        GlobalOptions globalOptions,
        Func<GlobalModel, ITaskUseCases> factory)
        : base("task", "Task commands")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        Add(BuildCreateCommand());
        Add(BuildListCommand());
        Add(BuildInfoCommand());
        Add(BuildWaitCommand());
        Add(BuildAbortCommand());
        Add(BuildDeleteCommand());
        Add(BuildUpdateResourcesCommand());
        Add(BuildUpdateConstantCommand());
        Add(BuildSnapshotCommand());
        Add(BuildStdoutCommand());
        Add(BuildStderrCommand());
        Add(BuildCarbonFactsCommand());
        Add(BuildDependenciesStateCommand());
    }

    private Command BuildCreateCommand()
    {
        var examples = new Example[] {
            new(
                Title: "Regular usage",
                CommandLines: new[] {
                """
                qarnot task create
                    --constants "DOCKER_CMD=echo hello world"
                    --instance 4
                    --name "Task name"
                    --profile docker-batch
                """,
                }
            ),
            new(
                Title: "Usage with a set of constants",
                CommandLines: new[] {
                """
                qarnot task create
                    --constants "DOCKER_CMD=echo hello world" DOCKER_REPO=library/ubuntu DOCKER_TAG=latest
                    --instance 4
                    --name "Task name"
                    --profile docker-batch
                """
                }
            ),
            new(
                Title:"Usage with simple tasks dependencies (NB: MUST be in a Job with 'use-dependencies' enabled). The created task will be started when the dependency task completes, in any state.",
                CommandLines: new[] {
                """
                qarnot task create
                    --job some-job-with-dependencies-enabled
                    --instance 4
                    --name "Task name"
                    --profile docker-batch
                    --depends-on 12345678-1111-1111-1111-111111111111
                """
                }
            ),
            new(
                Title:
                """
                Usage with advanced tasks dependencies (NB: MUST be in a Job with 'use-dependencies' enabled).
                    - the created task will be started when the dependency task is cancelled or failed
                    - if however the dependency task completes as 'Success', then the created task will be cancelled before execution
                """,
                CommandLines: new[] {
                """
                qarnot task create
                    --job some-job-with-dependencies-enabled
                    --instance 4
                    --name "Task name"
                    --profile docker-batch
                    --depends-on 12345678-1111-1111-1111-111111111111:Failure,Cancelled
                """
                }
            ),
            new(
                Title:
                """
                Usage with multiple tasks dependencies. (NB: MUST be in a Job with 'use-dependencies' enabled). The created task will
                    - be started when the first dependency task succeeds AND the second one completes in any state
                    - cancelled without executing if the first dependency completes as Failure or Cancelled
                """,
                CommandLines: new[] {
                """
                qarnot task create
                    --job some-job-with-dependencies-enabled
                    --instance 4 
                    --name "Task name"
                    --profile docker-batch
                    --depends-on 12345678-1111-1111-1111-111111111111:Success aaaaaaaa-0000-0000-0000-000000000000
                """
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

        var jobOpt = new Option<string>("--job")
        {
            Description = "UUID or short name of the job the task should be attached to",
        };

        var poolOpt = new Option<string>("--pool")
        {
            Description = "UUID or short name of the pool the task should be attached to",
        };

        var nameOpt = new Option<string>("--name", "-n")
        {
            Description = "Name of the task",
        };

        var shortNameOpt = new Option<string>("--shortname", "-s")
        {
            Description = "Short name of the task",
        };

        var profileOpt = new Option<string>("--profile", "-p")
        {
            Description = "Name of the profile used for the task",
        };

        var rangeOpt = new Option<string>("--range")
        {
            Description = "Range of the task",
        };

        var instanceOpt = new Option<uint?>("--instance", "-i")
        {
            Description = "Instance count of the task",
        };

        var fileOpt = new Option<string>("--file", "-f")
        {
            Description = "File with a json configuration of the task. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"TaskName\",\"Profile\": \"docker-batch\",\"Constants\": [ \"DOCKER_CMD=echo hello world\", ],\"InstanceCount\": 1}' > CreateTask.json)",
        };

        var tagsOpt = new Option<List<string>?>("--tags", "-t")
        {
            Description = "Tags of the task",
        }.WithMultipleArgs();

        var constantsOpt = new Option<List<string>?>("--constants", "-c")
        {
            Description = "Constants of the task",
        }.WithMultipleArgs();

        var constraintsOpt = new Option<List<string>?>("--constraints")
        {
            Description = "Constraints of the task",
        }.WithMultipleArgs();

        var labelsOpt = new Option<List<string>?>("--labels")
        {
            Description = "Labels of the task",
        }.WithMultipleArgs();

        var resourcesOpt = new Option<List<string>?>("--resources", "-r")
        {
            Description = "Name of the buckets of the task",
        }.WithMultipleArgs();

        var resultOpt = new Option<string>("--result")
        {
            Description = "Name of result bucket of the task", AllowMultipleArgumentsPerToken = true,
        };

        var waitForResourcesSynchronizationOpt = new Option<bool?>("--wait-for-resources-synchronization")
        {
            Description = "Wait for the pool resources to synchronized before launching the task",
        };

        var maxTotalRetriesOpt = new Option<uint?>("--max-total-retries")
        {
            Description = "Total number of times the task can have its instances retried in case of failure",
        };

        var maxRetriesPerInstanceOpt = new Option<uint?>("--max-retries-per-instance")
        {
            Description = "Total number of times each task instance will be allowed to retry in case of failure",
        };

        var maxTimeQueueSecondsOpt = new Option<uint?>("--max-time-queue")
        {
            Description = "Max time to wait before time out when there is not any place to execute the task (in seconds)",
        };

        var dependsOnOpt = new Option<List<string>?>("--depends-on", "-d")
        {
            Description = "List of task UUIDs and optional final states that this task must wait for before starting. (Must be used with a job with 'use-dependencies' set.)",
        }.WithMultipleArgs();

        var ttlOpt = new Option<uint?>("--ttl")
        {
            Description = "Default TTL for the task resources cache (in seconds)",
        };

        var resultTtlOpt = new Option<uint?>("--result-ttl")
        {
            Description = "Default TTL for the task results cache (in seconds)",
        };

        var hardwareConstraintMinimumCoreCountOpt = new Option<uint?>("--min-core-count")
        {
            Description = "Minimum number of cores that tasks in the pool will have access to",
        };

        var hardwareConstraintMaximumCoreCountOpt = new Option<uint?>("--max-core-count")
        {
            Description = "Maximum number of cores that the task will have access to",
        };

        var hardwareConstraintMinimumRamCoreRatioOpt = new Option<decimal?>("--min-ram-core-ratio")
        {
            Description = "Minimum ratio of RAM per number of cores that the task will have access to",
        };

        var hardwareConstraintMaximumRamCoreRatioOpt = new Option<decimal?>("--max-ram-core-ratio")
        {
            Description = "Maximum ratio of RAM per number of cores that task will have access to",
        };

        var hardwareConstraintSpecificHardwareOpt = new Option<List<string>?>("--specific-hardware-constraints")
        {
            Description = "List of constraints for specific hardware, described by specification keys. Specification keys are to be separated by spaces. Make sure to quote specification keys if they contain spaces (example : qarnot pool create --name thename --profile theprofile --specific-hardware-constraints \"Amd Ryzen 7\" \"Another hardware constraint\") ",
        }.WithMultipleArgs();

        var hardwareConstraintGpuHardwareOpt = new Option<bool?>("--gpu-hardware")
        {
            Description = "Force the task to run on GPU powered machines",
        };

        var hardwareConstraintSsdOpt = new Option<bool?>("--ssd-hardware")
        {
            Description = "Force the task to run on machines that have SSDs", Arity = ArgumentArity.ZeroOrOne,
        };

        var hardwareConstraintNoSsdOpt = new Option<bool?>("--no-ssd-hardware")
        {
            Description = "Force the tasks to run on machines that don't have SSDs", Arity = ArgumentArity.ZeroOrOne,
        };

        var hardwareConstraintMinimumRamOpt = new Option<decimal?>("--min-ram")
        {
            Description = "Minimum amount of RAM (in MB) that the task will have access to",
        };

        var hardwareConstraintMaximumRamOpt = new Option<decimal?>("--max-ram")
        {
            Description = "Maximum amount of RAM (in MB) that the task will have access to",
        };

        var hardwareConstraintCpuModelHardwareOpt = new Option<string?>("--cpu-model")
        {
            Description = "Target a specific CPU model to use when running the task",
        };

        var secretsAccessRightsByKeyOpt = new Option<List<string>?>("--secrets-access-rights-by-key")
        {
            Description = "Give the task access to secrets described by their keys. Only available to standalone task, use `--secrets-access-rights-by-key` on the pool for tasks running within a pool",
        }.WithMultipleArgs();

        var secretsAccessRightsByPrefixOpt = new Option<List<string>?>("--secrets-access-rights-by-prefix")
        {
            Description = "Give the task access to secrets described by their prefixs. Only available to standalone task, use `--secrets-access-rights-by-prefix` on the pool for tasks running within a pool",
        }.WithMultipleArgs();
;

        var schedulingTypeOpt = new Option<string>("--scheduling-type")
        {
            Description = "Specify the type of scheduling used for the task",
        };

        var machineTargetOpt = new Option<string>("--machine-target")
        {
            Description = "Available only for 'Reserved' scheduling. Specify the reserved machine on which the task should run",
        };

        var reservationTargetOpt = new Option<string>("--reservation-target")
        {
            Description = "Available only for 'Reserved' scheduling. Specify the name of the reservation to use to define the machine on which the task should run",
        };

        var periodicOpt = new Option<uint?>("--periodic")
        {
            Description = "Periodic time, in seconds, to synchronize the task files to the output bucket",
        };

        var whitelistOpt = new Option<string>("--whitelist")
        {
            Description = "Whitelist of task files to be synchronized to the output bucket",
        };

        var blacklistOpt = new Option<string>("--blacklist")
        {
            Description = "Blacklist of task files to synchronize to the output bucket",
        };

        var exportCredentialsToEnvOpt = new Option<bool?>("--export-credentials-to-env")
        {
            Description = "Activate the exportation of the api and storage credentials to the task environment (default is false)",
        };

        var projectUuidOpt = new Option<Guid?>("--project-uuid")
        {
            Description = "UUID of the project this task belongs to",
        };

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
            maxTimeQueueSecondsOpt,
            dependsOnOpt,
            ttlOpt,
            resultTtlOpt,
            hardwareConstraintMinimumCoreCountOpt,
            hardwareConstraintMaximumCoreCountOpt,
            hardwareConstraintMinimumRamCoreRatioOpt,
            hardwareConstraintMaximumRamCoreRatioOpt,
            hardwareConstraintSpecificHardwareOpt,
            hardwareConstraintGpuHardwareOpt,
            hardwareConstraintSsdOpt,
            hardwareConstraintNoSsdOpt,
            hardwareConstraintMinimumRamOpt,
            hardwareConstraintMaximumRamOpt,
            hardwareConstraintCpuModelHardwareOpt,
            secretsAccessRightsByKeyOpt,
            secretsAccessRightsByPrefixOpt,
            schedulingTypeOpt,
            machineTargetOpt,
            reservationTargetOpt,
            periodicOpt,
            whitelistOpt,
            blacklistOpt,
            exportCredentialsToEnvOpt,
            projectUuidOpt,
        };

        cmd.SetModelAction(
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
                maxTimeQueueSecondsOpt,
                dependsOnOpt,
                ttlOpt,
                resultTtlOpt,
                hardwareConstraintMinimumCoreCountOpt,
                hardwareConstraintMaximumCoreCountOpt,
                hardwareConstraintMinimumRamCoreRatioOpt,
                hardwareConstraintMaximumRamCoreRatioOpt,
                hardwareConstraintSpecificHardwareOpt,
                hardwareConstraintGpuHardwareOpt,
                hardwareConstraintSsdOpt,
                hardwareConstraintNoSsdOpt,
                hardwareConstraintMinimumRamOpt,
                hardwareConstraintMaximumRamOpt,
                hardwareConstraintCpuModelHardwareOpt,
                secretsAccessRightsByKeyOpt,
                secretsAccessRightsByPrefixOpt,
                schedulingTypeOpt,
                machineTargetOpt,
                reservationTargetOpt,
                periodicOpt,
                whitelistOpt,
                blacklistOpt,
                exportCredentialsToEnvOpt,
                projectUuidOpt,
                GlobalOptions
            )
        );

        cmd.Validators.Add(result =>
        {
            if (result.GetResult(hardwareConstraintSsdOpt) is not null && result.GetResult(hardwareConstraintNoSsdOpt) is not null)
            {
                result.AddError($"{hardwareConstraintSsdOpt.Name} and {hardwareConstraintNoSsdOpt.Name} are mutually exclusive.");
            }
        });

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

        cmd.SetModelAction(
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


        cmd.SetModelAction(
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

        var stdoutOpt = new Option<bool>("--stdout", "-o")
        {
            Description = "Print STDOUT events while waiting",
        };

        var stderrOpt = new Option<bool>("--stderr", "-e")
        {
            Description = "Print STDERR events while waiting",
        };

        var cmd = new CommandWithExamples("wait", "Wait for the end of a task")
        {
            examples,
            stdoutOpt,
            stderrOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
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

        cmd.SetModelAction(
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

        cmd.SetModelAction(
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

        cmd.SetModelAction(
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

        var constantNameOpt = new Option<string>("--constant-name")
        {
            Description = "Name of the constant to update", Required = true,
        };

        var constantValueOpt = new Option<string>("--constant-value")
        {
            Description = "New value for the constant to update",
        };

        var cmd = new CommandWithExamples("update-constant", "Update constant of a running task")
        {
            example,
            constantNameOpt,
            constantValueOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
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
        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        // TODO: Remove these options once the command 'task snapshot' is definitely replaced by 'task snapshot create'
        var periodicOpt = new Option<uint>("--periodic")
        {
            Description = "Periodic time, in seconds, to synchronize the task files to the output bucket",
        };

        var whitelistOpt = new Option<string>("--whitelist")
        {
            Description = "Whitelist of task files to be synchronized to the output bucket",
        };

        var blacklistOpt = new Option<string>("--blacklist")
        {
            Description = "Blacklist of task files to synchronize to the output bucket",
        };

        var bucketNameOpt = new Option<string>("--bucket")
        {
            Description = "Name of the output bucket used for the snapshot",
        };

        var cmd = new CommandWithExamples(
            "snapshot",
            "Commands to manage task snapshots." +
            "[deprecated] trigger a snasphot: prefer the use of subcommand 'create' to trigger snapshot: 'qarnot task snapshot create'"
        )
        {
            periodicOpt,
            whitelistOpt,
            blacklistOpt,
            bucketNameOpt
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
            model => Factory(model).Snapshot(model, true),
            new SnapshotTaskBinder(
                periodicOpt,
                whitelistOpt,
                blacklistOpt,
                bucketNameOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        cmd.Add(BuildSnapshotCreateSubcommand());
        cmd.Add(BuildSnapshotGetSubcommand());
        cmd.Add(BuildSnapshotWaitSubcommand());

        return cmd;
    }

    private Command BuildSnapshotCreateSubcommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task snapshot create --id TaskID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var periodicOpt = new Option<uint>("--periodic")
        {
            Description = "Periodic time, in seconds, to synchronize the task files to the output bucket",
        };

        var whitelistOpt = new Option<string>("--whitelist")
        {
            Description = "Whitelist of task files to be synchronized to the output bucket",
        };

        var blacklistOpt = new Option<string>("--blacklist")
        {
            Description = "Blacklist of task files to synchronize to the output bucket",
        };

        var bucketNameOpt = new Option<string>("--bucket")
        {
            Description = "Name of the output bucket used for the snapshot",
        };

        var cmd = new CommandWithExamples(
            "create",
            "Trigger a snapshot: request to upload a version of the running task files into the output bucket"
        )
        {
            example,
            periodicOpt,
            whitelistOpt,
            blacklistOpt,
            bucketNameOpt
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
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

    private Command BuildSnapshotGetSubcommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task snapshot get --id TASK_UUID --snapshot-id SNAPSHOT_ID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var snapshotIdOpt = new Option<string>("--snapshot-id")
        {
            Description = "ID of the snapshot to retrieve status for", Required = true,
        };

        var cmd = new CommandWithExamples(
            "get",
            "Get the status of a task snapshot"
        )
        {
            example,
            snapshotIdOpt
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
            model => Factory(model).SnapshotStatus(model),
            new GetSnapshotStatusBinder(
                snapshotIdOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSnapshotWaitSubcommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task snapshot wait --id TASK_UUID --snapshot-id SNAPSHOT_ID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var snapshotIdOpt = new Option<string>("--snapshot-id")
        {
            Description = "ID of the snapshot to wait for", Required = true,
        };

        var timeoutOpt = new Option<int>("--timeout")
        {
            Description = "Maximum time to wait in seconds (-1 for no timeout)",
            DefaultValueFactory = _ => -1,
        };

        var updateIntervalOpt = new Option<int>("--update-interval")
        {
            Description = "Time between status updates in seconds",
            DefaultValueFactory = _ => 10,
        };

        var cmd = new CommandWithExamples(
            "wait",
            "Wait for a task snapshot to complete and return its status"
        )
        {
            example,
            snapshotIdOpt,
            timeoutOpt,
            updateIntervalOpt
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
            model => Factory(model).WaitSnapshot(model),
            new WaitSnapshotBinder(
                snapshotIdOpt,
                timeoutOpt,
                updateIntervalOpt,
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

        var instanceIdOpt = new Option<uint?>("--instance-id")
        {
            Description = "Get the stdout of a specific instance",
        };

        var freshOpt = new Option<bool>("--fresh", "-f")
        {
            Description = "Get the last stdout dump",
        };

        var cmd = new CommandWithExamples("stdout", "Get the stdout of a task")
        {
            examples,
            instanceIdOpt,
            freshOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
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

        var instanceIdOpt = new Option<uint?>("--instance-id")
        {
            Description = "Get the stderr of a specific instance",
        };

        var freshOpt = new Option<bool>("--fresh", "-f")
        {
            Description = "Get the last stderr dump",
        };

        var cmd = new CommandWithExamples("stderr", "Get the stderr of a task")
        {
            examples,
            instanceIdOpt,
            freshOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
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

    private Command BuildDependenciesStateCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot task dependencies-state --id TASK_UUID",
            }
        );

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);
        var cmd = new CommandWithExamples(
            "dependencies-state",
            "Show the dependency resolution state of a task"
        )
        {
            example,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
            model => Factory(model).DependenciesState(model),
            new GetPoolsOrTasksBinder(
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildCarbonFactsCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Task carbon-facts",
                CommandLines: new[] {
                  "qarnot task carbon-facts --datacenter \"european_dc\" --name \"Task name\"",
                }
            )
        };

        var getTasksOptions = new GetPoolsOrTasksOptions(PoolOrTask.Task);

        var comparisonDatacenterOpt = new Option<string?>("--datacenter", "-d")
        {
            Description = "Compare the carbon facts to a specific datacenter. By default use generic european datacenter 'european_dc'.",
        };

        var cmd = new CommandWithExamples("carbon-facts", "Get the carbon facts of a task")
        {
            examples,
            comparisonDatacenterOpt,
        }.AddGetPoolsOrTasksOptions(getTasksOptions);

        cmd.SetModelAction(
            model => Factory(model).CarbonFacts(model),
            new GetPoolOrTaskCarbonFactsBinder(
                comparisonDatacenterOpt,
                getTasksOptions,
                GlobalOptions
            )
        );

        return cmd;
    }
}
