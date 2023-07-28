using System.CommandLine;
using System.CommandLine.Binding;
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
    private readonly Option<List<string>> TagsOpt;
    private readonly Option<List<string>> ConstantsOpt;
    private readonly Option<List<string>> ConstraintsOpt;
    private readonly Option<List<string>> LabelsOpt;
    private readonly Option<List<string>> ResourcesOpt;
    private readonly Option<string> ResultOpt;
    private readonly Option<bool?> WaitForResourcesSynchronizationOpt;
    private readonly Option<uint?> MaxTotalRetriesOpt;
    private readonly Option<uint?> MaxRetriesPerInstanceOpt;
    private readonly Option<List<string>> DependentsOpt;
    private readonly Option<uint?> TtlOpt;
    private readonly Option<List<string>> SecretsAccessRightsByKeyOpt;
    private readonly Option<List<string>> SecretsAccessRightsByPrefixOpt;
    private readonly Option<string> SchedulingTypeOpt;
    private readonly Option<string> MachineTargetOpt;
    private readonly Option<uint?> PeriodicOpt;
    private readonly Option<string> WhitelistOpt;
    private readonly Option<string> BlacklistOpt;
    private readonly Option<bool?> ExportCredentialsToEnvOpt;

    public CreateTaskBinder(
        Option<string> jobOpt,
        Option<string> poolOpt,
        Option<string> nameOpt,
        Option<string> shortNameOpt,
        Option<string> profileOpt,
        Option<string> rangeOpt,
        Option<uint?> instanceOpt,
        Option<string> fileOpt,
        Option<List<string>> tagsOpt,
        Option<List<string>> constantsOpt,
        Option<List<string>> constraintsOpt,
        Option<List<string>> labelsOpt,
        Option<List<string>> resourcesOpt,
        Option<string> resultOpt,
        Option<bool?> waitForResourcesSynchronizationOpt,
        Option<uint?> maxTotalRetriesOpt,
        Option<uint?> maxRetriesPerInstanceOpt,
        Option<List<string>> dependentsOpt,
        Option<uint?> ttlOpt,
        Option<List<string>> secretsAccessRightsByKeyOpt,
        Option<List<string>> secretsAccessRightsByPrefixOpt,
        Option<string> schedulingTypeOpt,
        Option<string> machineTargetOpt,
        Option<uint?> periodicOpt,
        Option<string> whitelistOpt,
        Option<string> blacklistOpt,
        Option<bool?> exportCredentialsToEnv,
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
        DependentsOpt = dependentsOpt;
        TtlOpt = ttlOpt;
        SecretsAccessRightsByKeyOpt = secretsAccessRightsByKeyOpt;
        SecretsAccessRightsByPrefixOpt = secretsAccessRightsByPrefixOpt;
        SchedulingTypeOpt = schedulingTypeOpt;
        MachineTargetOpt = machineTargetOpt;
        PeriodicOpt = periodicOpt;
        WhitelistOpt = whitelistOpt;
        BlacklistOpt = blacklistOpt;
        ExportCredentialsToEnvOpt = exportCredentialsToEnv;
    }

    protected override CreateTaskModel GetBoundValueImpl(BindingContext bindingContext)
    {
        var file = bindingContext.ParseResult.GetValueForOption(FileOpt);
        var model = file is not null
            ? JsonConvert.DeserializeObject<CreateTaskModel>(File.ReadAllText(file))!
            : new CreateTaskModel();

        model = new(
            Job: bindingContext.ParseResult.GetValueForOption(JobOpt) ?? model.Job,
            Pool: bindingContext.ParseResult.GetValueForOption(PoolOpt) ?? model.Pool,
            Name: bindingContext.ParseResult.GetValueForOption(NameOpt) ?? model.Name,
            ShortName: bindingContext.ParseResult.GetValueForOption(ShortNameOpt) ?? model.ShortName,
            Profile: bindingContext.ParseResult.GetValueForOption(ProfileOpt) ?? model.Profile,
            Range: bindingContext.ParseResult.GetValueForOption(RangeOpt) ?? model.Range,
            Instance: bindingContext.ParseResult.GetValueForOption(InstanceOpt) ?? model.Instance,
            Tags: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(TagsOpt), model.Tags),
            Constants: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ConstantsOpt), model.Constants),
            Constraints: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ConstraintsOpt), model.Constraints),
            Labels: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(LabelsOpt), model.Labels),
            Resources: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(ResourcesOpt), model.Resources),
            Result: bindingContext.ParseResult.GetValueForOption(ResultOpt) ?? model.Result,
            WaitForResourcesSynchronization: bindingContext.ParseResult.GetValueForOption(WaitForResourcesSynchronizationOpt) ?? model.WaitForResourcesSynchronization,
            MaxTotalRetries: bindingContext.ParseResult.GetValueForOption(MaxTotalRetriesOpt) ?? model.MaxTotalRetries,
            MaxRetriesPerInstance: bindingContext.ParseResult.GetValueForOption(MaxRetriesPerInstanceOpt) ?? model.MaxRetriesPerInstance,
            Dependents: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(DependentsOpt), model.Dependents),
            Ttl: bindingContext.ParseResult.GetValueForOption(TtlOpt) ?? model.Ttl,
            SecretsAccessRightsByKey: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(SecretsAccessRightsByKeyOpt), model.SecretsAccessRightsByKey),
            SecretsAccessRightsByPrefix: Helpers.CoalesceEmpty(bindingContext.ParseResult.GetValueForOption(SecretsAccessRightsByPrefixOpt), model.SecretsAccessRightsByPrefix),
            SchedulingType: bindingContext.ParseResult.GetValueForOption(SchedulingTypeOpt) ?? model.SchedulingType,
            MachineTarget: bindingContext.ParseResult.GetValueForOption(MachineTargetOpt) ?? model.MachineTarget,
            Periodic: bindingContext.ParseResult.GetValueForOption(PeriodicOpt) ?? model.Periodic,
            Whitelist: bindingContext.ParseResult.GetValueForOption(WhitelistOpt) ?? model.Whitelist,
            Blacklist: bindingContext.ParseResult.GetValueForOption(BlacklistOpt) ?? model.Blacklist,
            ExportCredentialsToEnv: bindingContext.ParseResult.GetValueForOption(ExportCredentialsToEnvOpt) ?? model.ExportCredentialsToEnv
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

    protected override WaitTasksModel GetBoundValueImpl(BindingContext bindingContext) =>
        new WaitTasksModel(
            bindingContext.ParseResult.GetValueForOption(StdoutOpt),
            bindingContext.ParseResult.GetValueForOption(StderrOpt)
        ).BindGetPoolsOrTasksOptions(bindingContext, GetTasksOptions);
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

    protected override SnapshotTasksModel GetBoundValueImpl(BindingContext bindingContext) =>
        new SnapshotTasksModel(
            bindingContext.ParseResult.GetValueForOption(PeriodicOpt),
            bindingContext.ParseResult.GetValueForOption(WhitelistOpt),
            bindingContext.ParseResult.GetValueForOption(BlacklistOpt),
            bindingContext.ParseResult.GetValueForOption(BucketOpt)
        ).BindGetPoolsOrTasksOptions(bindingContext, GetTasksOptions);
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

    protected override GetTasksOutputModel GetBoundValueImpl(BindingContext bindingContext) =>
        new GetTasksOutputModel(
            bindingContext.ParseResult.GetValueForOption(InstanceIdOpt),
            bindingContext.ParseResult.GetValueForOption(FreshOpt)
        ).BindGetPoolsOrTasksOptions(bindingContext, GetTasksOptions);
}
