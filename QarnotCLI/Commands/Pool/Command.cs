using System.CommandLine;

namespace QarnotCLI;

public class PoolCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, IPoolUseCases> Factory;

    public PoolCommand(GlobalOptions globalOptions, Func<GlobalModel, IPoolUseCases> factory)
        : base("pool", "Pool commands")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        Add(BuildCreateCommand());
        Add(BuildListCommand());
        Add(BuildInfoCommand());
        Add(BuildSetScalingCommand());
        Add(BuildDeleteCommand());
        Add(BuildUpdateResourcesCommand());
        Add(BuildUpdateConstantCommand());
        Add(BuildCarbonFactsCommand());
    }

    private Command BuildCreateCommand()
    {

        var examples = new[] {
            new Example(
                Title: "Regular usage",
                CommandLines: new[] {
                    "qarnot pool create --instanceNodes 4 --name \"Pool name\" --profile docker-batch",
                }
            ),
            new Example(
                Title: "Create a pool with scaling policies defined in a file",
                CommandLines: new[] {
                    "qarnot pool create --name \"Pool name\" --profile docker-batch --scaling @scaling_file.json",
                },
                IgnoreTest: true
            ),
            new Example(
                Title: "File config usage",
                CommandLines: new[] {
                    "qarnot pool create --file FileName.json",
                },
                IgnoreTest: true
            ),
            new Example(
                Title: "Error: missing instanceNodes",
                CommandLines: new[] {
                    "qarnot pool create --name \"Pool name\" --profile docker-batch"
                },
                IsError: true
            )
        };

        var nameOpt = new Option<string>("--name", "-n")
        {
            Description = "Name of the pool", Required = true,
        };

        var shortnameOpt = new Option<string>("--shortname", "-s")
        {
            Description = "Short name of the pool",
        };

        var profileOpt = new Option<string>("--profile", "-p")
        {
            Description = "Name of the profile used for the pool", Required = true,
        };

        var instanceCountOpt = new Option<uint?>("--instanceNodes", "-i")
        {
            Description = "Instance count of the pool (required if no scaling policy is provided)",
        };

        var fileOpt = new Option<string>("--file", "-f")
        {
            Description = "File with a json configuration of the pool. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"PoolName\",\"Profile\": \"docker-batch\",\"InstanceCount\": 1}' > CreatePool.json)",
        };

        var tagsOpt = new Option<List<string>?>("--tags", "-t")
        {
            Description = "Tags of the pool",
        }.WithMultipleArgs();

        var constantsOpt = new Option<List<string>?>("--constants", "-c")
        {
            Description = "Constants of the pool",
        }.WithMultipleArgs();

        var constraintsOpt = new Option<List<string>?>("--constraints")
        {
            Description = "Constraints of the pool",
        }.WithMultipleArgs();

        var labelsOpt = new Option<List<string>?>("--labels")
        {
            Description = "Labels of the pool",
        }.WithMultipleArgs();

        var resourcesOpt = new Option<List<string>?>("--resources", "-r")
        {
            Description = "Name of the buckets of the task",
        }.WithMultipleArgs();

        var tasksWaitForSynchronizationOpt = new Option<bool?>("--tasks-wait-for-synchronization")
        {
            Description = "Have all the pool's tasks wait for the resources to be synchronized before running if the pool resources are updated before the task submission. (set to true or false, default: false)",
        };

        var ttlOpt = new Option<uint?>("--ttl")
        {
            Description = "Default TTL for the pool resources cache (in seconds)",
        };

        var maxTotalRetriesOpt = new Option<uint?>("--max-total-retries")
        {
            Description = "Total number of times the pool can have its instances retried in case of failure",
        };

        var maxRetriesPerInstanceOpt = new Option<uint?>("--max-retries-per-instance")
        {
            Description = "Total number of times each pool instance will be allowed to retry in case of failure",
        };

        var maxTimeQueueSecondsOpt = new Option<uint?>("--max-time-queue")
        {
            Description = "Max time to wait before time out when there is not any place to execute the pool (in seconds)",
        };

        var scalingOpt = new Option<string>("--scaling")
        {
            Description = "Scaling policies of the pool. Use either direct json format or a file path prefixed by '@'",
        };

        var hardwareConstraintMinimumCoreCountOpt = new Option<uint?>("--min-core-count")
        {
            Description = "Minimum number of cores that tasks in the pool will have access to",
        };

        var hardwareConstraintMaximumCoreCountOpt = new Option<uint?>("--max-core-count")
        {
            Description = "Maximum number of cores that tasks in the pool will have access to",
        };

        var hardwareConstraintMinimumRamCoreRatioOpt = new Option<decimal?>("--min-ram-core-ratio")
        {
            Description = "Minimum ratio of RAM per number of cores that tasks in the pool will have access to",
        };

        var hardwareConstraintMaximumRamCoreRatioOpt = new Option<decimal?>("--max-ram-core-ratio")
        {
            Description = "Maximum ratio of RAM per number of cores that tasks in the pool will have access to",
        };

        var hardwareConstraintSpecificHardwareOpt = new Option<List<string>?>("--specific-hardware-constraints")
        {
            Description = "List of constraints for specific hardware, described by specification keys. Specification keys are to be separated by spaces. Make sure to quote specification keys if they contain spaces (example : qarnot pool create --name thename --profile theprofile --specific-hardware-constraints \"Amd Ryzen 7\" \"Another hardware constraint\") ",
        }.WithMultipleArgs();

        var hardwareConstraintGpuHardwareOpt = new Option<bool?>("--gpu-hardware")
        {
            Description = "Force the tasks in the pool to run on GPU powered machines",
        };

        var hardwareConstraintSsdOpt = new Option<bool?>("--ssd-hardware")
        {
            Description = "Force the tasks in the pool to run on machines that have SSDs", Arity = ArgumentArity.ZeroOrOne,
        };

        var hardwareConstraintNoSsdOpt = new Option<bool?>("--no-ssd-hardware")
        {
            Description = "Force the tasks in the pool to run on machines that don't have SSDs", Arity = ArgumentArity.ZeroOrOne,
        };

        var hardwareConstraintMinimumRamOpt = new Option<decimal?>("--min-ram")
        {
            Description = "Minimum amount of RAM (in MB) that tasks in the pool will have access to",
        };

        var hardwareConstraintMaximumRamOpt = new Option<decimal?>("--max-ram")
        {
            Description = "Maximum amount of RAM (in MB) that tasks in the pool will have access to",
        };

        var hardwareConstraintCpuModelHardwareOpt = new Option<string?>("--cpu-model")
        {
            Description = "Target a specific CPU model to use when running the tasks in the pool",
        };

        var secretsAccessRightsByKeyOpt = new Option<List<string>?>("--secrets-access-rights-by-key")
        {
            Description = "Give the pool access to secrets described by their keys",
        }.WithMultipleArgs();

        var secretsAccessRightsByPrefixOpt = new Option<List<string>?>("--secrets-access-rights-by-prefix")
        {
            Description = "Give the pool access to secrets described by their prefixs",
        }.WithMultipleArgs();

        var schedulingTypeOpt = new Option<string>("--scheduling-type")
        {
            Description = "Specify the type of scheduling used for the pool",
        };

        var machineTargetOpt = new Option<string>("--machine-target")
        {
            Description = "Available only for 'Reserved' scheduling. Specify the reserved machine on which the pool should run",
        };

        var exportCredentialsToEnvOpt = new Option<bool?>("--export-credentials-to-env")
        {
            Description = "Activate the exportation of the api and storage credentials to the pool environment (default is false)",
        };

        var slotsPerNodeOpt = new Option<uint?>("--slots-per-node")
        {
            Description = "Number of slots per node (Multi slots settings)",
        };

        var projectUuidOpt = new Option<Guid?>("--project-uuid")
        {
            Description = "UUID of the project this pool belongs to",
        };

        var cmd = new CommandWithExamples("create", "Create and launch a new pool")
        {
            examples,
            nameOpt,
            shortnameOpt,
            profileOpt,
            instanceCountOpt,
            fileOpt,
            tagsOpt,
            constantsOpt,
            constraintsOpt,
            labelsOpt,
            resourcesOpt,
            tasksWaitForSynchronizationOpt,
            ttlOpt,
            maxTotalRetriesOpt,
            maxRetriesPerInstanceOpt,
            maxTimeQueueSecondsOpt,
            scalingOpt,
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
            exportCredentialsToEnvOpt,
            slotsPerNodeOpt,
            projectUuidOpt
        };

        cmd.SetModelAction(
            model => Factory(model).Create(model),
            new CreatePoolBinder(
                nameOpt,
                shortnameOpt,
                profileOpt,
                instanceCountOpt,
                fileOpt,
                tagsOpt,
                constantsOpt,
                constraintsOpt,
                labelsOpt,
                resourcesOpt,
                tasksWaitForSynchronizationOpt,
                ttlOpt,
                maxTotalRetriesOpt,
                maxRetriesPerInstanceOpt,
                maxTimeQueueSecondsOpt,
                scalingOpt,
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
                exportCredentialsToEnvOpt,
                slotsPerNodeOpt,
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
                "qarnot pool list --name \"Pool name\" --tags TAG1 TAG2",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);
        var cmd = new CommandWithExamples("list", "List the running pools")
        {
            example,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).List(model),
            new GetPoolsOrTasksBinder(
                getPoolsOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildInfoCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new string[] {
                "qarnot pool info --name \"Pool name\" --tags TAG1 TAG2",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);
        var cmd = new CommandWithExamples("info", "Detailed info on a pool")
        {
            example,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).Info(model),
            new GetPoolsOrTasksBinder(
                getPoolsOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSetScalingCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot pool set-scaling --name \"Pool name\" --scaling @scaling_file.json",
            },
            IgnoreTest: true
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);

        var scalingOpt = new Option<string>("--scaling")
        {
            Description = "Scaling policies of the pool. Use either direct json format or a file path prefixed by '@'", Required = true,
        };

        var cmd = new CommandWithExamples("set-scaling", "Update the pool's scaling options")
        {
            example,
            scalingOpt,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).UpdateScaling(model),
            new UpdatePoolsScalingBinder(
                scalingOpt,
                getPoolsOptions,
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
                "qarnot pool delete --name \"Pool name\" --tags TAG1 TAG2",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);
        var cmd = new CommandWithExamples("delete", "Delete a pool")
        {
            example
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).Delete(model),
            new GetPoolsOrTasksBinder(
                getPoolsOptions,
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
                "qarnot pool update-resources --name \"Pool name\" --tags TAG1 TAG2",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);
        var cmd = new CommandWithExamples("update-resources", "Update resources for a running pool")
        {
            example
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).UpdateResources(model),
            new GetPoolsOrTasksBinder(
                getPoolsOptions,
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
                "qarnot pool update-constant --constant-name QARNOT_SECRET__SUPER_TOKEN --constant-value new-token --id PoolID",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);

        var constantNameOpt = new Option<string>("--constant-name")
        {
            Description = "Name of the constant to update", Required = true,
        };

        var constantValueOpt = new Option<string>("--constant-value")
        {
            Description = "New value for the constant to update",
        };

        var cmd = new CommandWithExamples("update-constant", "Update constant of a running pool")
        {
            example,
            constantNameOpt,
            constantValueOpt,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).UpdateConstant(model),
            new UpdatePoolsOrTasksConstantBinder(
                constantNameOpt,
                constantValueOpt,
                getPoolsOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildCarbonFactsCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Pool carbon-facts",
                CommandLines: new[] {
                  "qarnot pool carbon-facts --datacenter \"european_dc\" --name \"Pool name\"",
                }
            )
        };

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);

        var comparisonDatacenterOpt = new Option<string?>("--datacenter", "-d")
        {
            Description = "Compare the carbon facts to a specific datacenter. By default use generic european datacenter 'european_dc'.",
        };

        var cmd = new CommandWithExamples("carbon-facts", "Get the carbon facts of a pool")
        {
            examples,
            comparisonDatacenterOpt,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetModelAction(
            model => Factory(model).CarbonFacts(model),
            new GetPoolOrTaskCarbonFactsBinder(
                comparisonDatacenterOpt,
                getPoolsOptions,
                GlobalOptions
            )
        );

        return cmd;
    }
}
