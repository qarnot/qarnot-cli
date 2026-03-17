using System.CommandLine;
using Newtonsoft.Json;

namespace QarnotCLI;

public class UpdatePoolsScalingBinder : GlobalBinder<UpdatePoolScalingModel>
{
    private readonly JsonConverter[] Converters = new JsonConverter[] {
        new TimePeriodSpecificationJsonConverter(),
        new ScalingPolicyConverter(),
    };

    private readonly Option<string> ScalingOpt;
    private readonly GetPoolsOrTasksOptions GetPoolsOrTasksOptions;

    public UpdatePoolsScalingBinder(
        Option<string> scalingOpt,
        GetPoolsOrTasksOptions getPoolsOrTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        ScalingOpt = scalingOpt;
        GetPoolsOrTasksOptions = getPoolsOrTasksOptions;
    }

    protected override UpdatePoolScalingModel GetBoundValueImpl(ParseResult parseResult)
    {
        QarnotSDK.Scaling? scaling = null;
        var scalingStr = parseResult.GetValue(ScalingOpt);
        if (!string.IsNullOrWhiteSpace(scalingStr))
        {
            scaling = ParseScaling(scalingStr);
        }

        return new UpdatePoolScalingModel(scaling).BindGetPoolsOrTasksOptions(parseResult, GetPoolsOrTasksOptions);
    }

    private QarnotSDK.Scaling? ParseScaling(string? scaling)
    {
        if (string.IsNullOrWhiteSpace(scaling))
        {
            return null;
        }
        if (scaling.StartsWith('@'))
        {
            return JsonConvert.DeserializeObject<QarnotSDK.Scaling>(
                File.ReadAllText(scaling.Substring(1)),
                Converters
            );
        }
        else
        {
            return JsonConvert.DeserializeObject<QarnotSDK.Scaling>(scaling, Converters);
        }
    }
}

public class CreatePoolBinder : GlobalBinder<CreatePoolModel>
{
    private readonly JsonConverter[] Converters = new JsonConverter[] {
        new TimePeriodSpecificationJsonConverter(),
        new ScalingPolicyConverter(),
    };

    private readonly Option<string> NameOpt;
    private readonly Option<string> ShortnameOpt;
    private readonly Option<string> ProfileOpt;
    private readonly Option<uint?> InstanceCountOpt;
    private readonly Option<string> FileOpt;
    private readonly Option<List<string>?> TagsOpt;
    private readonly Option<List<string>?> ConstantsOpt;
    private readonly Option<List<string>?> ConstraintsOpt;
    private readonly Option<List<string>?> LabelsOpt;
    private readonly Option<List<string>?> ResourcesOpt;
    private readonly Option<bool?> TasksWaitForSynchronizationOpt;
    private readonly Option<uint?> TtlOpt;
    private readonly Option<uint?> MaxTotalRetriesOpt;
    private readonly Option<uint?> MaxRetriesPerInstanceOpt;
    private readonly Option<uint?> MaxTimeQueueSecondsOpt;
    private readonly Option<string> ScalingOpt;
    private readonly Option<uint?> HardwareConstraintMinimumCoreCountOpt;
    private readonly Option<uint?> HardwareConstraintMaximumCoreCountOpt;
    private readonly Option<decimal?> HardwareConstraintMinimumRamCoreRatioOpt;
    private readonly Option<decimal?> HardwareConstraintMaximumRamCoreRatioOpt;
    private readonly Option<List<string>?> HardwareConstraintSpecificHardware;
    private readonly Option<bool?> HardwareConstraintGpuHardware;
    private readonly Option<bool?> HardwareConstraintSsdHardware;
    private readonly Option<bool?> HardwareConstraintNoSsdHardware;
    private readonly Option<decimal?> HardwareConstraintMinimumRamHardware;
    private readonly Option<decimal?> HardwareConstraintMaximumRamHardware;
    private readonly Option<string?> HardwareConstraintCpuModelHardware;
    private readonly Option<List<string>?> SecretsAccessRightsByKeyOpt;
    private readonly Option<List<string>?> SecretsAccessRightsByPrefixOpt;
    private readonly Option<string> SchedulingTypeOpt;
    private readonly Option<string> MachineTargetOpt;
    private readonly Option<bool?> ExportCredentialsToEnvOpt;
    private readonly Option<uint?> SlotsPerNodeOpt;
    private readonly Option<Guid?> ProjectUuidOpt;

    public CreatePoolBinder(
        Option<string> nameOpt,
        Option<string> shortnameOpt,
        Option<string> profileOpt,
        Option<uint?> instanceCountOpt,
        Option<string> fileOpt,
        Option<List<string>?> tagsOpt,
        Option<List<string>?> constantsOpt,
        Option<List<string>?> constraintsOpt,
        Option<List<string>?> labelsOpt,
        Option<List<string>?> resourcesOpt,
        Option<bool?> tasksWaitForSynchronizationOpt,
        Option<uint?> ttlOpt,
        Option<uint?> maxTotalRetriesOpt,
        Option<uint?> maxRetriesPerInstanceOpt,
        Option<uint?> maxTimeQueueSecondsOpt,
        Option<string> scalingOpt,
        Option<uint?> hardwareConstraintMinimumCoreCount,
        Option<uint?> hardwareConstraintMaximumCoreCount,
        Option<decimal?> hardwareConstraintMinimumRamCoreRatio,
        Option<decimal?> hardwareConstraintMaximumRamCoreRatio,
        Option<List<string>?> hardwareConstraintSpecificHardware,
        Option<bool?> hardwareConstraintGpuHardware,
        Option<bool?> hardwareConstraintSsdHardware,
        Option<bool?> hardwareConstraintNoSsdHardware,
        Option<decimal?> hardwareConstraintMinimumRamHardware,
        Option<decimal?> hardwareConstraintMaximumRamHardware,
        Option<string?> hardwareConstraintCpuModelHardware,
        Option<List<string>?> secretsAccessRightsByKeyOpt,
        Option<List<string>?> secretsAccessRightsByPrefixOpt,
        Option<string> schedulingTypeOpt,
        Option<string> machineTargetOpt,
        Option<bool?> exportCredentialsToEnvOpt,
        Option<uint?> slotsPerNodeOpt,
        Option<Guid?> projectUuidOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        ShortnameOpt = shortnameOpt;
        ProfileOpt = profileOpt;
        InstanceCountOpt = instanceCountOpt;
        FileOpt = fileOpt;
        TagsOpt = tagsOpt;
        ConstantsOpt = constantsOpt;
        ConstraintsOpt = constraintsOpt;
        LabelsOpt = labelsOpt;
        ResourcesOpt = resourcesOpt;
        TasksWaitForSynchronizationOpt = tasksWaitForSynchronizationOpt;
        TtlOpt = ttlOpt;
        MaxTotalRetriesOpt = maxTotalRetriesOpt;
        MaxRetriesPerInstanceOpt = maxRetriesPerInstanceOpt;
        MaxTimeQueueSecondsOpt = maxTimeQueueSecondsOpt;
        ScalingOpt = scalingOpt;
        HardwareConstraintMinimumCoreCountOpt = hardwareConstraintMinimumCoreCount;
        HardwareConstraintMaximumCoreCountOpt = hardwareConstraintMaximumCoreCount;
        HardwareConstraintMinimumRamCoreRatioOpt = hardwareConstraintMinimumRamCoreRatio;
        HardwareConstraintMaximumRamCoreRatioOpt = hardwareConstraintMaximumRamCoreRatio;
        HardwareConstraintSpecificHardware = hardwareConstraintSpecificHardware;
        HardwareConstraintGpuHardware = hardwareConstraintGpuHardware;
        HardwareConstraintSsdHardware = hardwareConstraintSsdHardware;
        HardwareConstraintNoSsdHardware = hardwareConstraintNoSsdHardware;
        HardwareConstraintMinimumRamHardware = hardwareConstraintMinimumRamHardware;
        HardwareConstraintMaximumRamHardware = hardwareConstraintMaximumRamHardware;
        HardwareConstraintCpuModelHardware = hardwareConstraintCpuModelHardware;
        SecretsAccessRightsByKeyOpt = secretsAccessRightsByKeyOpt;
        SecretsAccessRightsByPrefixOpt = secretsAccessRightsByPrefixOpt;
        SchedulingTypeOpt = schedulingTypeOpt;
        MachineTargetOpt = machineTargetOpt;
        ExportCredentialsToEnvOpt = exportCredentialsToEnvOpt;
        SlotsPerNodeOpt = slotsPerNodeOpt;
        ProjectUuidOpt = projectUuidOpt;
    }

    protected override CreatePoolModel GetBoundValueImpl(ParseResult parseResult)
    {
        var file = parseResult.GetValue(FileOpt);
        var model = !string.IsNullOrWhiteSpace(file)
            ? ParseFromFile(file)
            : new CreatePoolModel();

        QarnotSDK.Scaling? scaling = null;
        var scalingStr = parseResult.GetValue(ScalingOpt);
        if (!string.IsNullOrWhiteSpace(scalingStr))
        {
            scaling = ParseScaling(scalingStr);
        }

        QarnotSDK.HardwareConstraints? hardwareConstraints = Helpers.BuildHardwareConstraints(
            minimumCoreCount: parseResult.GetValue(HardwareConstraintMinimumCoreCountOpt),
            maximumCoreCount: parseResult.GetValue(HardwareConstraintMaximumCoreCountOpt),
            minimumRamCoreRatio: parseResult.GetValue(HardwareConstraintMinimumRamCoreRatioOpt),
            maximumRamCoreRatio: parseResult.GetValue(HardwareConstraintMaximumRamCoreRatioOpt),
            specificHardware: parseResult.GetValue(HardwareConstraintSpecificHardware),
            gpuHardware: parseResult.GetValue(HardwareConstraintGpuHardware),
            ssdHardware: parseResult.GetValue(HardwareConstraintSsdHardware),
            noSsdHardware: parseResult.GetValue(HardwareConstraintNoSsdHardware),
            minimumRamHardware: parseResult.GetValue(HardwareConstraintMinimumRamHardware),
            maximumRamHardware: parseResult.GetValue(HardwareConstraintMaximumRamHardware),
            cpuModelHardware: parseResult.GetValue(HardwareConstraintCpuModelHardware)
        );

        model = new(
            Name: parseResult.GetValue(NameOpt) ?? model.Name,
            Shortname: parseResult.GetValue(ShortnameOpt) ?? model.Shortname,
            Profile: parseResult.GetValue(ProfileOpt) ?? model.Profile,
            InstanceCount: parseResult.GetValue(InstanceCountOpt) ?? model.InstanceCount,
            Tags: Helpers.CoalesceEmpty(parseResult.GetValue(TagsOpt), model.Tags),
            Constants: Helpers.CoalesceEmpty(parseResult.GetValue(ConstantsOpt), model.Constants),
            Constraints: Helpers.CoalesceEmpty(parseResult.GetValue(ConstraintsOpt), model.Constraints),
            Labels: Helpers.CoalesceEmpty(parseResult.GetValue(LabelsOpt), model.Labels),
            Resources: Helpers.CoalesceEmpty(parseResult.GetValue(ResourcesOpt), model.Resources),
            TasksWaitForSynchronization: parseResult.GetValue(TasksWaitForSynchronizationOpt) ?? model.TasksWaitForSynchronization,
            Ttl: parseResult.GetValue(TtlOpt) ?? model.Ttl,
            MaxTotalRetries: parseResult.GetValue(MaxTotalRetriesOpt) ?? model.MaxTotalRetries,
            MaxRetriesPerInstance: parseResult.GetValue(MaxRetriesPerInstanceOpt) ?? model.MaxRetriesPerInstance,
            MaxTimeQueueSeconds: parseResult.GetValue(MaxTimeQueueSecondsOpt) ?? model.MaxTimeQueueSeconds,
            Scaling: scaling ?? model.Scaling,
            HardwareConstraints: hardwareConstraints ?? model.HardwareConstraints,
            SecretsAccessRightsByKey: Helpers.CoalesceEmpty(parseResult.GetValue(SecretsAccessRightsByKeyOpt), model.SecretsAccessRightsByKey),
            SecretsAccessRightsByPrefix: Helpers.CoalesceEmpty(parseResult.GetValue(SecretsAccessRightsByPrefixOpt), model.SecretsAccessRightsByPrefix),
            SchedulingType: parseResult.GetValue(SchedulingTypeOpt) ?? model.SchedulingType,
            MachineTarget: parseResult.GetValue(MachineTargetOpt) ?? model.MachineTarget,
            ExportCredentialsToEnv: parseResult.GetValue(ExportCredentialsToEnvOpt) ?? model.ExportCredentialsToEnv,
            SlotsPerNode: parseResult.GetValue(SlotsPerNodeOpt) ?? model.SlotsPerNode,
            ProjectUuid: parseResult.GetValue(ProjectUuidOpt) ?? model.ProjectUuid
        );

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new Exception("A name must be given to the pool");
        }

        if (model.InstanceCount == 0 && model.Scaling == null)
        {
            throw new Exception("An instance count must be given to the pool");
        }

        return model;
    }

    private CreatePoolModel ParseFromFile(string file)
    {
        var content = File.ReadAllText(file);
        var model = JsonConvert.DeserializeObject<CreatePoolModel>(content, Converters);
        if (model is null)
        {
            throw new Exception($"Couldn't parse pool creation settings from {file}");
        }

        return model;
    }

    private QarnotSDK.Scaling? ParseScaling(string? scaling)
    {
        if (string.IsNullOrWhiteSpace(scaling))
        {
            return null;
        }
        if (scaling.StartsWith('@'))
        {
            return JsonConvert.DeserializeObject<QarnotSDK.Scaling>(
                File.ReadAllText(scaling.Substring(1)),
                Converters
            );
        }
        else
        {
            return JsonConvert.DeserializeObject<QarnotSDK.Scaling>(scaling, Converters);
        }
    }
}
