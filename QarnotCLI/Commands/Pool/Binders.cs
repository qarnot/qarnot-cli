using System.CommandLine;
using System.CommandLine.Binding;
using Newtonsoft.Json;
using QarnotSDK;

namespace QarnotCLI;

public class UpdatePoolElasticSettingsBinder : GlobalBinder<UpdatePoolElasticSettingsModel>
{
    private readonly Option<uint?> MinSlotsOpt;
    private readonly Option<uint?> MaxSlotsOpt;
    private readonly Option<uint?> MinIdlingSlotsOpt;
    private readonly Option<uint?> ResizePeriodOpt;
    private readonly Option<float?> ResizeFactorOpt;
    private readonly Option<uint?> MinIdlingTimeOpt;
    private readonly GetPoolsOrTasksOptions GetPoolsOptions;

    public UpdatePoolElasticSettingsBinder(
        Option<uint?> minSlotsOpt,
        Option<uint?> maxSlotsOpt,
        Option<uint?> minIdlingSlotsOpt,
        Option<uint?> resizePeriodOpt,
        Option<float?> resizeFactorOpt,
        Option<uint?> minIdlingtimeOpt,
        GetPoolsOrTasksOptions getPoolsOptions,
        GlobalOptions  globalOptions
    ) : base(globalOptions)
    {
        MinSlotsOpt = minSlotsOpt;
        MaxSlotsOpt = maxSlotsOpt;
        MinIdlingSlotsOpt = minIdlingSlotsOpt;
        ResizePeriodOpt = resizePeriodOpt;
        ResizeFactorOpt = resizeFactorOpt;
        MinIdlingTimeOpt = minIdlingtimeOpt;
        GetPoolsOptions = getPoolsOptions;
    }

    protected override UpdatePoolElasticSettingsModel GetBoundValueImpl(BindingContext bindingContext) =>
        new UpdatePoolElasticSettingsModel(
            bindingContext.ParseResult.GetValueForOption(MinSlotsOpt),
            bindingContext.ParseResult.GetValueForOption(MaxSlotsOpt),
            bindingContext.ParseResult.GetValueForOption(MinIdlingSlotsOpt),
            bindingContext.ParseResult.GetValueForOption(ResizePeriodOpt),
            bindingContext.ParseResult.GetValueForOption(ResizeFactorOpt),
            bindingContext.ParseResult.GetValueForOption(MinIdlingTimeOpt)
        ).BindGetPoolsOrTasksOptions(bindingContext, GetPoolsOptions);
}

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

    protected override UpdatePoolScalingModel GetBoundValueImpl(BindingContext bindingContext)
    {
        QarnotSDK.Scaling? scaling = null;
        var scalingStr = bindingContext.ParseResult.GetValueForOption(ScalingOpt);
        if (!string.IsNullOrWhiteSpace(scalingStr))
        {
            scaling = ParseScaling(scalingStr);
        }

        return new UpdatePoolScalingModel(scaling).BindGetPoolsOrTasksOptions(bindingContext, GetPoolsOrTasksOptions);
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
    private readonly Option<bool?> IsElasticOpt;
    private readonly Option<string> FileOpt;
    private readonly Option<List<string>> TagsOpt;
    private readonly Option<List<string>> ConstantsOpt;
    private readonly Option<List<string>> ConstraintsOpt;
    private readonly Option<List<string>> LabelsOpt;
    private readonly Option<List<string>> ResourcesOpt;
    private readonly Option<uint?> ElasticMinSlotsOpt;
    private readonly Option<uint?> ElasticMaxSlotsOpt;
    private readonly Option<uint?> ElasticMinIdlingSlotsOpt;
    private readonly Option<uint?> ElasticResizePeriodOpt;
    private readonly Option<float?> ElasticResizeFactorOpt;
    private readonly Option<uint?> ElasticMinIdlingTimeOpt;
    private readonly Option<bool?> TasksWaitForSynchronizationOpt;
    private readonly Option<uint?> TtlOpt;
    private readonly Option<uint?> MaxTotalRetriesOpt;
    private readonly Option<uint?> MaxRetriesPerInstanceOpt;
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
    private readonly Option<List<string>> SecretsAccessRightsByKeyOpt;
    private readonly Option<List<string>> SecretsAccessRightsByPrefixOpt;
    private readonly Option<string> SchedulingTypeOpt;
    private readonly Option<string> MachineTargetOpt;
    private readonly Option<bool?> ExportCredentialsToEnvOpt;

    public CreatePoolBinder(
        Option<string> nameOpt,
        Option<string> shortnameOpt,
        Option<string> profileOpt,
        Option<uint?> instanceCountOpt,
        Option<bool?> isElasticOpt,
        Option<string> fileOpt,
        Option<List<string>> tagsOpt,
        Option<List<string>> constantsOpt,
        Option<List<string>> constraintsOpt,
        Option<List<string>> labelsOpt,
        Option<List<string>> resourcesOpt,
        Option<uint?> elasticMinSlotsOpt,
        Option<uint?> elasticMaxSlotsOpt,
        Option<uint?> elasticMinIdlingSlotsOpt,
        Option<uint?> elasticResizePeriodOpt,
        Option<float?> elasticResizeFactorOpt,
        Option<uint?> elasticMinIdlingTimeOpt,
        Option<bool?> tasksWaitForSynchronizationOpt,
        Option<uint?> ttlOpt,
        Option<uint?> maxTotalRetriesOpt,
        Option<uint?> maxRetriesPerInstanceOpt,
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
        Option<List<string>> secretsAccessRightsByKeyOpt,
        Option<List<string>> secretsAccessRightsByPrefixOpt,
        Option<string> schedulingTypeOpt,
        Option<string> machineTargetOpt,
        Option<bool?> exportCredentialsToEnvOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        ShortnameOpt = shortnameOpt;
        ProfileOpt = profileOpt;
        InstanceCountOpt = instanceCountOpt;
        IsElasticOpt = isElasticOpt;
        FileOpt = fileOpt;
        TagsOpt = tagsOpt;
        ConstantsOpt = constantsOpt;
        ConstraintsOpt = constraintsOpt;
        LabelsOpt = labelsOpt;
        ResourcesOpt = resourcesOpt;
        ElasticMinSlotsOpt = elasticMinSlotsOpt;
        ElasticMaxSlotsOpt = elasticMaxSlotsOpt;
        ElasticMinIdlingSlotsOpt = elasticMinIdlingSlotsOpt;
        ElasticResizePeriodOpt = elasticResizePeriodOpt;
        ElasticResizeFactorOpt = elasticResizeFactorOpt;
        ElasticMinIdlingTimeOpt = elasticMinIdlingTimeOpt;
        TasksWaitForSynchronizationOpt = tasksWaitForSynchronizationOpt;
        TtlOpt = ttlOpt;
        MaxTotalRetriesOpt = maxTotalRetriesOpt;
        MaxRetriesPerInstanceOpt = maxRetriesPerInstanceOpt;
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
    }

    protected override CreatePoolModel GetBoundValueImpl(BindingContext bindingContext)
    {
        var file = bindingContext.ParseResult.GetValueForOption(FileOpt);
        var model = !string.IsNullOrWhiteSpace(file)
            ? ParseFromFile(file)
            : new CreatePoolModel();

        QarnotSDK.Scaling? scaling = null;
        var scalingStr = bindingContext.ParseResult.GetValueForOption(ScalingOpt);
        if (!string.IsNullOrWhiteSpace(scalingStr))
        {
            scaling = ParseScaling(scalingStr);
        }

        QarnotSDK.HardwareConstraints? hardwareConstraints = Helpers.BuildHardwareConstraints(
            minimumCoreCount: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMinimumCoreCountOpt),
            maximumCoreCount: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMaximumCoreCountOpt),
            minimumRamCoreRatio: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMinimumRamCoreRatioOpt),
            maximumRamCoreRatio: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMaximumRamCoreRatioOpt),
            specificHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintSpecificHardware),
            gpuHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintGpuHardware),
            ssdHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintSsdHardware),
            noSsdHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintNoSsdHardware),
            minimumRamHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMinimumRamHardware),
            maximumRamHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintMaximumRamHardware),
            cpuModelHardware: bindingContext.ParseResult.GetValueForOption(HardwareConstraintCpuModelHardware)
        );

        model = new(
            Name: bindingContext.ParseResult.GetValueForOption(NameOpt) ?? model.Name,
            Shortname: bindingContext.ParseResult.GetValueForOption(ShortnameOpt) ?? model.Shortname,
            Profile: bindingContext.ParseResult.GetValueForOption(ProfileOpt) ?? model.Profile,
            InstanceCount: bindingContext.ParseResult.GetValueForOption(InstanceCountOpt) ?? model.InstanceCount,
            IsElastic: bindingContext.ParseResult.GetValueForOption(IsElasticOpt) ?? model.IsElastic,
            Tags: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(TagsOpt), model.Tags),
            Constants: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ConstantsOpt), model.Constants),
            Constraints: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ConstraintsOpt), model.Constraints),
            Labels: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(LabelsOpt), model.Labels),
            Resources: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ResourcesOpt), model.Resources),
            ElasticMinSlots: bindingContext.ParseResult.GetValueForOption(ElasticMinSlotsOpt) ?? model.ElasticMinSlots,
            ElasticMaxSlots: bindingContext.ParseResult.GetValueForOption(ElasticMaxSlotsOpt) ?? model.ElasticMaxSlots,
            ElasticMinIdlingSlots: bindingContext.ParseResult.GetValueForOption(ElasticMinIdlingSlotsOpt) ?? model.ElasticMinIdlingSlots,
            ElasticResizePeriod: bindingContext.ParseResult.GetValueForOption(ElasticResizePeriodOpt) ?? model.ElasticResizePeriod,
            ElasticResizeFactor: bindingContext.ParseResult.GetValueForOption(ElasticResizeFactorOpt) ?? model.ElasticResizeFactor,
            ElasticMinIdlingTime: bindingContext.ParseResult.GetValueForOption(ElasticMinIdlingTimeOpt) ?? model.ElasticMinIdlingTime,
            TasksWaitForSynchronization: bindingContext.ParseResult.GetValueForOption(TasksWaitForSynchronizationOpt) ?? model.TasksWaitForSynchronization,
            Ttl: bindingContext.ParseResult.GetValueForOption(TtlOpt) ?? model.Ttl,
            MaxTotalRetries: bindingContext.ParseResult.GetValueForOption(MaxTotalRetriesOpt) ?? model.MaxTotalRetries,
            MaxRetriesPerInstance: bindingContext.ParseResult.GetValueForOption(MaxRetriesPerInstanceOpt) ?? model.MaxRetriesPerInstance,
            Scaling: scaling ?? model.Scaling,
            HardwareConstraints: hardwareConstraints ?? model.HardwareConstraints,
            SecretsAccessRightsByKey: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(SecretsAccessRightsByKeyOpt), model.SecretsAccessRightsByKey),
            SecretsAccessRightsByPrefix: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(SecretsAccessRightsByPrefixOpt), model.SecretsAccessRightsByPrefix),
            SchedulingType: bindingContext.ParseResult.GetValueForOption(SchedulingTypeOpt) ?? model.SchedulingType,
            MachineTarget: bindingContext.ParseResult.GetValueForOption(MachineTargetOpt) ?? model.MachineTarget,
            ExportCredentialsToEnv: bindingContext.ParseResult.GetValueForOption(ExportCredentialsToEnvOpt) ?? model.ExportCredentialsToEnv
        );

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new Exception("A name must be given to the pool");
        }

        if (model.InstanceCount == 0 && model.ElasticMinSlots == null && model.Scaling == null)
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
