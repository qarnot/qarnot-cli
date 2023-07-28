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

        AddCommand(BuildCreateCommand());
        AddCommand(BuildListCommand());
        AddCommand(BuildInfoCommand());
        AddCommand(BuildSetElasticSettingsCommand());
        AddCommand(BuildSetScalingCommand());
        AddCommand(BuildDeleteCommand());
        AddCommand(BuildUpdateResourcesCommand());
        AddCommand(BuildUpdateConstantCommand());
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

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the pool"
        ) { IsRequired = true };

        var shortnameOpt = new Option<string>(
            aliases: new[] { "--shortname", "-s" },
            description: "Short name of the pool"
        );

        var profileOpt = new Option<string>(
            aliases: new[] { "--profile", "-p" },
            description: "Name of the profile used for the pool"
        ) { IsRequired = true };

        var instanceCountOpt = new Option<uint?>(
            aliases: new[] { "--instanceNodes", "-i" },
            description: "(Required if not elastic) instance count of the pool"
        );

        var isElasticOpt = new Option<bool?>(
            aliases: new[] { "--pool-is-elastic", "-e" },
            description: "Make the pool elastic"
        );

        var fileOpt = new Option<string>(
            aliases: new[] { "--file", "-f" },
            description: "File with a json configuration of the pool. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"PoolName\",\"Profile\": \"docker-batch\",\"InstanceCount\": 1}' > CreatePool.json)"
        );

        var tagsOpt = new Option<List<string>>(
            aliases: new[] { "--tags", "-t" },
            description: "Tags of the pool"
        ) { AllowMultipleArgumentsPerToken = true };

        var constantsOpt = new Option<List<string>>(
            aliases: new[] { "--constants", "-c" },
            description: "Constants of the pool"
        ) { AllowMultipleArgumentsPerToken = true };

        var constraintsOpt = new Option<List<string>>(
            name: "--constraints",
            description: "Constraints of the pool"
        ) { AllowMultipleArgumentsPerToken = true };

        var labelsOpt = new Option<List<string>>(
            name: "--labels",
            description: "Labels of the pool"
        ) { AllowMultipleArgumentsPerToken = true };

        var resourcesOpt = new Option<List<string>>(
            aliases: new[] { "--resources", "-r" },
            description: "Name of the buckets of the task"
        ) { AllowMultipleArgumentsPerToken = true };

        var elasticMinSlotsOpt = new Option<uint?>(
            name: "--min-slot",
            description: "Minimum slot number for the pool in elastic mode"
        );

        var elasticMaxSlotsOpt = new Option<uint?>(
            name: "--max-slot",
            description: "Maximum slot number for the pool in elastic mode"
        );

        var elasticMinIdlingSlotsOpt = new Option<uint?>(
            name: "--min-idling-slot",
            description: "Minimum idling slot number"
        );

        var elasticResizePeriodOpt = new Option<uint?>(
            name: "--resize-period",
            description: "Elastic resize period"
        );

        var elasticResizeFactorOpt = new Option<float?>(
            name: "--resize-factor",
            description: "Elastic resize factor"
        );

        var elasticMinIdlingTimeOpt = new Option<uint?>(
            name: "--min-idling-time",
            description: "Minimum idling time"
        );

        var tasksWaitForSynchronizationOpt = new Option<bool?>(
            name: "--tasks-wait-for-synchronization",
            description: "Have all the pool's tasks wait for the resources to be synchronized before running if the pool resources are updated before the task submission. (set to true or false, default: false)"
        );

        var ttlOpt = new Option<uint?>(
            name: "--ttl",
            description: "Default TTL for the pool resources cache (in seconds)"
        );

        var maxTotalRetriesOpt = new Option<uint?>(
            name: "--max-total-retries",
            description: "Total number of times the pool can have its instances retried in case of failure"
        );

        var maxRetriesPerInstanceOpt = new Option<uint?>(
            name: "--max-retries-per-instance",
            description: "Total number of times each pool instance will be allowed to retry in case of failure"
        );

        var scalingOpt = new Option<string>(
            name: "--scaling",
            description: "Scaling policies of the pool. Use either direct json format or a file path prefixed by '@'"
        );

        var secretsAccessRightsByKeyOpt = new Option<List<string>>(
            name: "--secrets-access-rights-by-key",
            description: "Give the pool access to secrets described by their keys"
        ) { AllowMultipleArgumentsPerToken = true };

        var secretsAccessRightsByPrefixOpt = new Option<List<string>>(
            name: "--secrets-access-rights-by-prefix",
            description: "Give the pool access to secrets described by their prefixs"
        ) { AllowMultipleArgumentsPerToken = true };

        var schedulingTypeOpt = new Option<string>(
            name: "--scheduling-type",
            description: "Specify the type of scheduling used for the pool"
        );

        var machineTargetOpt = new Option<string>(
            name: "--machine-target",
            description: "Available only for 'Reserved' scheduling. Specify the reserved machine on which the pool should run"
        );

        var exportCredentialsToEnvOpt = new Option<bool?>(
            name: "--export-credentials-to-env",
            description: "Activate the exportation of the api and storage credentials to the pool environment (default is false)"
        );

        var cmd = new CommandWithExamples("create", "Create and launch a new pool")
        {
            examples,
            nameOpt,
            shortnameOpt,
            profileOpt,
            instanceCountOpt,
            isElasticOpt,
            fileOpt,
            tagsOpt,
            constantsOpt,
            constraintsOpt,
            labelsOpt,
            resourcesOpt,
            elasticMinSlotsOpt,
            elasticMaxSlotsOpt,
            elasticMinIdlingSlotsOpt,
            elasticResizePeriodOpt,
            elasticResizeFactorOpt,
            elasticMinIdlingTimeOpt,
            tasksWaitForSynchronizationOpt,
            ttlOpt,
            maxTotalRetriesOpt,
            maxRetriesPerInstanceOpt,
            scalingOpt,
            secretsAccessRightsByKeyOpt,
            secretsAccessRightsByPrefixOpt,
            schedulingTypeOpt,
            machineTargetOpt,
            exportCredentialsToEnvOpt,
        };

        cmd.SetHandler(
            model => Factory(model).Create(model),
            new CreatePoolBinder(
                nameOpt,
                shortnameOpt,
                profileOpt,
                instanceCountOpt,
                isElasticOpt,
                fileOpt,
                tagsOpt,
                constantsOpt,
                constraintsOpt,
                labelsOpt,
                resourcesOpt,
                elasticMinSlotsOpt,
                elasticMaxSlotsOpt,
                elasticMinIdlingSlotsOpt,
                elasticResizePeriodOpt,
                elasticResizeFactorOpt,
                elasticMinIdlingTimeOpt,
                tasksWaitForSynchronizationOpt,
                ttlOpt,
                maxTotalRetriesOpt,
                maxRetriesPerInstanceOpt,
                scalingOpt,
                secretsAccessRightsByKeyOpt,
                secretsAccessRightsByPrefixOpt,
                schedulingTypeOpt,
                machineTargetOpt,
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
                "qarnot pool list --name \"Pool name\" --tags TAG1 TAG2",
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);
        var cmd = new CommandWithExamples("list", "List the running pools")
        {
            example,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetHandler(
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

        cmd.SetHandler(
            model => Factory(model).Info(model),
            new GetPoolsOrTasksBinder(
                getPoolsOptions,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSetElasticSettingsCommand()
    {
        var example = new Example(
            Title: "Regular usage",
            CommandLines: new[] {
                "qarnot pool set-elastic-settings --id PoolID --min-slot 2 --max-slot 10"
            }
        );

        var getPoolsOptions = new GetPoolsOrTasksOptions(PoolOrTask.Pool);

        var elasticMinSlotsOpt = new Option<uint?>(
            name: "--min-slot",
            description: "Minimum slot number for the pool in elastic mode"
        );

        var elasticMaxSlotsOpt = new Option<uint?>(
            name: "--max-slot",
            description: "Maximum slot number for the pool in elastic mode"
        );

        var elasticMinIdlingSlotsOpt = new Option<uint?>(
            name: "--min-idling-slot",
            description: "Minimum idling slot number"
        );

        var elasticResizePeriodOpt = new Option<uint?>(
            name: "--resize-period",
            description: "Elastic resize period"
        );

        var elasticResizeFactorOpt = new Option<float?>(
            name: "--resize-factor",
            description: "Elastic resize factor"
        );

        var elasticMinIdlingTimeOpt = new Option<uint?>(
            name: "--min-idling-time",
            description: "Minimum idling time"
        );

        var cmd = new CommandWithExamples("set-elastic-settings", "Set the pool's elastic options")
        {
            example,
            elasticMinSlotsOpt,
            elasticMaxSlotsOpt,
            elasticMinIdlingSlotsOpt,
            elasticResizePeriodOpt,
            elasticResizeFactorOpt,
            elasticMinIdlingTimeOpt
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetHandler(
            model => Factory(model).UpdateElasticSettings(model),
            new UpdatePoolElasticSettingsBinder(
                elasticMinSlotsOpt,
                elasticMaxSlotsOpt,
                elasticMinIdlingSlotsOpt,
                elasticResizePeriodOpt,
                elasticResizeFactorOpt,
                elasticMinIdlingTimeOpt,
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

        var scalingOpt = new Option<string>(
            name: "--scaling",
            description: "Scaling policies of the pool. Use either direct json format or a file path prefixed by '@'"
        ) { IsRequired = true };

        var cmd = new CommandWithExamples("set-scaling", "Update the pool's scaling options")
        {
            example,
            scalingOpt,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetHandler(
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

        cmd.SetHandler(
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

        cmd.SetHandler(
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

        var constantNameOpt = new Option<string>(
            name: "--constant-name",
            description: "Name of the constant to update"
        ) { IsRequired = true };

        var constantValueOpt = new Option<string>(
            name: "--constant-value",
            description: "New value for the constant to update"
        );

        var cmd = new CommandWithExamples("update-constant", "Update constant of a running pool")
        {
            example,
            constantNameOpt,
            constantValueOpt,
        }.AddGetPoolsOrTasksOptions(getPoolsOptions);

        cmd.SetHandler(
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
}
