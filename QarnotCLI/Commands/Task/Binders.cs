using System.CommandLine;
using Newtonsoft.Json;

namespace QarnotCLI;

public class CreateTaskBinder : GlobalBinder<CreateTaskModel>
{
    private readonly Option<string> JobOpt;
    private readonly Option<string> PoolOpt;
    private readonly Option<string> NameOpt;
    private readonly Option<string> ShortNameOpt;
    private readonly Option<string> ProfileOpt;
    private readonly Option<string> RangeOpt;
    private readonly Option<uint?> InstanceOpt;
    private readonly Option<string> FileOpt;
    private readonly Option<List<string>?> TagsOpt;
    private readonly Option<List<string>?> ConstantsOpt;
    private readonly Option<List<string>?> ConstraintsOpt;
    private readonly Option<List<string>?> LabelsOpt;
    private readonly Option<List<string>?> ResourcesOpt;
    private readonly Option<string> ResultOpt;
    private readonly Option<bool?> WaitForResourcesSynchronizationOpt;
    private readonly Option<uint?> MaxTotalRetriesOpt;
    private readonly Option<uint?> MaxRetriesPerInstanceOpt;
    private readonly Option<uint?> MaxTimeQueueSecondsOpt;
    private readonly Option<List<string>?> DependsOnOpt;
    private readonly Option<uint?> TtlOpt;
    private readonly Option<uint?> ResultTtlOpt;
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
    private readonly Option<string> ReservationTargetOpt;
    private readonly Option<uint?> PeriodicOpt;
    private readonly Option<string> WhitelistOpt;
    private readonly Option<string> BlacklistOpt;
    private readonly Option<bool?> ExportCredentialsToEnvOpt;
    private readonly Option<Guid?> ProjectUuidOpt;

    public CreateTaskBinder(
        Option<string> jobOpt,
        Option<string> poolOpt,
        Option<string> nameOpt,
        Option<string> shortNameOpt,
        Option<string> profileOpt,
        Option<string> rangeOpt,
        Option<uint?> instanceOpt,
        Option<string> fileOpt,
        Option<List<string>?> tagsOpt,
        Option<List<string>?> constantsOpt,
        Option<List<string>?> constraintsOpt,
        Option<List<string>?> labelsOpt,
        Option<List<string>?> resourcesOpt,
        Option<string> resultOpt,
        Option<bool?> waitForResourcesSynchronizationOpt,
        Option<uint?> maxTotalRetriesOpt,
        Option<uint?> maxRetriesPerInstanceOpt,
        Option<uint?> maxTimeQueueSecondsOpt,
        Option<List<string>?> dependsOnOpt,
        Option<uint?> ttlOpt,
        Option<uint?> resultTtlOpt,
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
        Option<string> reservationTargetOpt,
        Option<uint?> periodicOpt,
        Option<string> whitelistOpt,
        Option<string> blacklistOpt,
        Option<bool?> exportCredentialsToEnv,
        Option<Guid?> projectUuidOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        JobOpt = jobOpt;
        PoolOpt = poolOpt;
        NameOpt = nameOpt;
        ShortNameOpt = shortNameOpt;
        ProfileOpt = profileOpt;
        RangeOpt = rangeOpt;
        InstanceOpt = instanceOpt;
        FileOpt = fileOpt;
        TagsOpt = tagsOpt;
        ConstantsOpt = constantsOpt;
        ConstraintsOpt = constraintsOpt;
        LabelsOpt = labelsOpt;
        ResourcesOpt = resourcesOpt;
        ResultOpt = resultOpt;
        WaitForResourcesSynchronizationOpt = waitForResourcesSynchronizationOpt;
        MaxTotalRetriesOpt = maxTotalRetriesOpt;
        MaxRetriesPerInstanceOpt = maxRetriesPerInstanceOpt;
        MaxTimeQueueSecondsOpt = maxTimeQueueSecondsOpt;
        DependsOnOpt = dependsOnOpt;
        TtlOpt = ttlOpt;
        ResultTtlOpt = resultTtlOpt;
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
        ReservationTargetOpt = reservationTargetOpt;
        PeriodicOpt = periodicOpt;
        WhitelistOpt = whitelistOpt;
        BlacklistOpt = blacklistOpt;
        ExportCredentialsToEnvOpt = exportCredentialsToEnv;
        ProjectUuidOpt = projectUuidOpt;
    }

    protected override CreateTaskModel GetBoundValueImpl(ParseResult parseResult)
    {
        var file = parseResult.GetValue(FileOpt);
        var model = file is not null
            ? DeserializeTaskModelFromFile(file)
            : new CreateTaskModel();

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
            Job: parseResult.GetValue(JobOpt) ?? model.Job,
            Pool: parseResult.GetValue(PoolOpt) ?? model.Pool,
            Name: parseResult.GetValue(NameOpt) ?? model.Name,
            ShortName: parseResult.GetValue(ShortNameOpt) ?? model.ShortName,
            Profile: parseResult.GetValue(ProfileOpt) ?? model.Profile,
            Range: parseResult.GetValue(RangeOpt) ?? model.Range,
            Instance: parseResult.GetValue(InstanceOpt) ?? model.Instance,
            Tags: Helpers.CoalesceEmpty(parseResult.GetValue(TagsOpt), model.Tags),
            Constants: Helpers.CoalesceEmpty(parseResult.GetValue(ConstantsOpt), model.Constants),
            Constraints: Helpers.CoalesceEmpty(parseResult.GetValue(ConstraintsOpt), model.Constraints),
            Labels: Helpers.CoalesceEmpty(parseResult.GetValue(LabelsOpt), model.Labels),
            Resources: Helpers.CoalesceEmpty(parseResult.GetValue(ResourcesOpt), model.Resources),
            Result: parseResult.GetValue(ResultOpt) ?? model.Result,
            WaitForResourcesSynchronization: parseResult.GetValue(WaitForResourcesSynchronizationOpt) ?? model.WaitForResourcesSynchronization,
            MaxTotalRetries: parseResult.GetValue(MaxTotalRetriesOpt) ?? model.MaxTotalRetries,
            MaxRetriesPerInstance: parseResult.GetValue(MaxRetriesPerInstanceOpt) ?? model.MaxRetriesPerInstance,
            MaxTimeQueueSeconds: parseResult.GetValue(MaxTimeQueueSecondsOpt) ?? model.MaxTimeQueueSeconds,
            DependsOn: Helpers.CoalesceEmpty(
                Helpers.ParseAdvancedDependencies(parseResult.GetValue(DependsOnOpt)),
                model.DependsOn),
            Ttl: parseResult.GetValue(TtlOpt) ?? model.Ttl,
            ResultTtl : parseResult.GetValue(ResultTtlOpt) ?? model.ResultTtl,
            HardwareConstraints: hardwareConstraints ?? model.HardwareConstraints,
            SecretsAccessRightsByKey: Helpers.CoalesceEmpty(parseResult.GetValue(SecretsAccessRightsByKeyOpt), model.SecretsAccessRightsByKey),
            SecretsAccessRightsByPrefix: Helpers.CoalesceEmpty(parseResult.GetValue(SecretsAccessRightsByPrefixOpt), model.SecretsAccessRightsByPrefix),
            SchedulingType: parseResult.GetValue(SchedulingTypeOpt) ?? model.SchedulingType,
            MachineTarget: parseResult.GetValue(MachineTargetOpt) ?? model.MachineTarget,
            ReservationTarget: parseResult.GetValue(ReservationTargetOpt) ?? model.ReservationTarget,
            Periodic: parseResult.GetValue(PeriodicOpt) ?? model.Periodic,
            Whitelist: parseResult.GetValue(WhitelistOpt) ?? model.Whitelist,
            Blacklist: parseResult.GetValue(BlacklistOpt) ?? model.Blacklist,
            ExportCredentialsToEnv: parseResult.GetValue(ExportCredentialsToEnvOpt) ?? model.ExportCredentialsToEnv,
            ProjectUuid: parseResult.GetValue(ProjectUuidOpt) ?? model.ProjectUuid
        );

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new Exception("A name must be given to the task");
        }

        if (string.IsNullOrWhiteSpace(model.Profile) && string.IsNullOrWhiteSpace(model.Pool)
                && string.IsNullOrWhiteSpace(model.Job))
        {
            throw new Exception("A task must have either a profile, a pool or a job");
        }

        if (model.Instance == 0 && model.Range is null)
        {
            throw new Exception("A number of instances or a range must be given to the task");
        }
        else if (model.Instance != 0 && !string.IsNullOrEmpty(model.Range))
        {
            throw new Exception("A task can't have both an instance count and a range");
        }

        return model;
    }

    private static CreateTaskModel DeserializeTaskModelFromFile(string file)
    {
        try
        {
            return JsonConvert.DeserializeObject<CreateTaskModel>(File.ReadAllText(file))!;
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new Exception(
                $"Invalid task configuration in file '{Path.GetFileName(file)}': {ex.Message}");
        }
    }
}

public class WaitTasksBinder : GlobalBinder<WaitTasksModel>
{
    private readonly Option<bool> StdoutOpt;
    private readonly Option<bool> StderrOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public WaitTasksBinder(
        Option<bool> stdoutOpt,
        Option<bool> stderrOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        StdoutOpt = stdoutOpt;
        StderrOpt = stderrOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override WaitTasksModel GetBoundValueImpl(ParseResult parseResult) =>
        new WaitTasksModel(
            parseResult.GetValue(StdoutOpt),
            parseResult.GetValue(StderrOpt)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}

public class SnapshotTaskBinder : GlobalBinder<SnapshotTasksModel>
{
    private readonly Option<uint> PeriodicOpt;
    private readonly Option<string> WhitelistOpt;
    private readonly Option<string> BlacklistOpt;
    private readonly Option<string> BucketOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public SnapshotTaskBinder(
        Option<uint> periodicOpt,
        Option<string> whitelistOpt,
        Option<string> blacklistOpt,
        Option<string> bucketOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        PeriodicOpt = periodicOpt;
        WhitelistOpt = whitelistOpt;
        BlacklistOpt = blacklistOpt;
        BucketOpt = bucketOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override SnapshotTasksModel GetBoundValueImpl(ParseResult parseResult) =>
        new SnapshotTasksModel(
            parseResult.GetValue(PeriodicOpt),
            parseResult.GetValue(WhitelistOpt),
            parseResult.GetValue(BlacklistOpt),
            parseResult.GetValue(BucketOpt)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}

public class GetTasksOutputBinder : GlobalBinder<GetTasksOutputModel>
{
    private readonly Option<uint?> InstanceIdOpt;
    private readonly Option<bool> FreshOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public GetTasksOutputBinder(
        Option<uint?> instanceIdOpt,
        Option<bool> freshOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        InstanceIdOpt = instanceIdOpt;
        FreshOpt = freshOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override GetTasksOutputModel GetBoundValueImpl(ParseResult parseResult) =>
        new GetTasksOutputModel(
            parseResult.GetValue(InstanceIdOpt),
            parseResult.GetValue(FreshOpt)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}

public class GetSnapshotStatusBinder : GlobalBinder<GetSnapshotStatusModel>
{
    private readonly Option<string> SnapshotIdOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public GetSnapshotStatusBinder(
        Option<string> snapshotIdOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        SnapshotIdOpt = snapshotIdOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override GetSnapshotStatusModel GetBoundValueImpl(ParseResult parseResult) =>
        new GetSnapshotStatusModel(
            parseResult.GetValue(SnapshotIdOpt)! // Option is marked Required in Command.cs
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}

public class WaitSnapshotBinder : GlobalBinder<WaitSnapshotModel>
{
    private readonly Option<string> SnapshotIdOpt;
    private readonly Option<int> TimeoutOpt;
    private readonly Option<int> UpdateIntervalOpt;
    private readonly GetPoolsOrTasksOptions GetTasksOptions;

    public WaitSnapshotBinder(
        Option<string> snapshotIdOpt,
        Option<int> timeoutOpt,
        Option<int> updateIntervalOpt,
        GetPoolsOrTasksOptions getTasksOptions,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        SnapshotIdOpt = snapshotIdOpt;
        TimeoutOpt = timeoutOpt;
        UpdateIntervalOpt = updateIntervalOpt;
        GetTasksOptions = getTasksOptions;
    }

    protected override WaitSnapshotModel GetBoundValueImpl(ParseResult parseResult) =>
        new WaitSnapshotModel(
            parseResult.GetValue(SnapshotIdOpt)!, // Option is marked Required in Command.cs
            parseResult.GetValue(TimeoutOpt),
            parseResult.GetValue(UpdateIntervalOpt)
        ).BindGetPoolsOrTasksOptions(parseResult, GetTasksOptions);
}
