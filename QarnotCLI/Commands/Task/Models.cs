using Newtonsoft.Json;

namespace QarnotCLI;

// CLI-local representation of an advanced task dependency.
// Equivalent to QarnotSDK.AdvancedDependency for consistency.
public record AdvancedDependencyModel(
    [property: JsonProperty("TaskUuid")]
    Guid TaskUuid,
    [property: JsonProperty("TaskFinalStateCondition")]
    List<QarnotSDK.TaskFinalState>? TaskFinalStateCondition
);

[JsonObject(MemberSerialization.OptIn)]
public record CreateTaskModel(
    [property: JsonProperty("JobUuidOrShortname")]
    string? Job,
    [property: JsonProperty("PoolUuidOrShortname")]
    string? Pool,
    [property: JsonProperty("Name")]
    string Name,
    [property: JsonProperty("Shortname")]
    string? ShortName,
    [property: JsonProperty("Profile")]
    string? Profile,
    [property: JsonProperty("Range")]
    string? Range,
    [property: JsonProperty("InstanceCount")]
    uint Instance,
    [property: JsonProperty("Tags")]
    List<string> Tags,
    [property: JsonProperty("Constants")]
    List<string> Constants,
    [property: JsonProperty("Constraints")]
    List<string> Constraints,
    [property: JsonProperty("Labels")]
    List<string> Labels,
    [property: JsonProperty("Resources")]
    List<string> Resources,
    [property: JsonProperty("Result")]
    string? Result,
    [property: JsonProperty("WaitForPoolResourcesSynchronization")]
    bool WaitForResourcesSynchronization,
    [property: JsonProperty("MaxTotalRetries")]
    uint? MaxTotalRetries,
    [property: JsonProperty("MaxTimeQueueSeconds")]
    uint? MaxTimeQueueSeconds,
    [property: JsonProperty("MaxRetriesPerInstance")]
    uint? MaxRetriesPerInstance,
    [property: JsonProperty("DependsOn")]
    List<AdvancedDependencyModel> DependsOn,
    [property: JsonProperty("DefaultResourcesCacheTTLSec")]
    uint? Ttl,
    [property: JsonProperty("ResultsCacheTTLSec")]
    uint? ResultTtl,
    [property: JsonProperty("HardwareConstraints")]
    QarnotSDK.HardwareConstraints? HardwareConstraints,
    [property: JsonProperty("SecretsAccessRightsByKey")]
    List<string> SecretsAccessRightsByKey,
    [property: JsonProperty("SecretsAccessRightsByPrefix")]
    List<string> SecretsAccessRightsByPrefix,
    [property: JsonProperty("SchedulingType")]
    string? SchedulingType,
    [property: JsonProperty("TargetedReservedMachineKey")]
    string? MachineTarget,
    [property: JsonProperty("TargetedReservationName")]
    string? ReservationTarget,
    [property: JsonProperty("SnapshotPeriodicSec")]
    uint? Periodic,
    [property: JsonProperty("Whitelist")]
    string? Whitelist,
    [property: JsonProperty("Blacklist")]
    string? Blacklist,
    [property: JsonProperty("ExportApiAndStorageCredentialsInEnvironment")]
    bool? ExportCredentialsToEnv,
    [property: JsonProperty("ProjectUuid")]
    Guid? ProjectUuid
): GlobalModel
{
    public CreateTaskModel()
        : this(
            Job: null,
            Pool: null,
            Name: "",
            ShortName: null,
            Profile: null,
            Range: null,
            Instance: 0,
            Tags: new(),
            Constants: new(),
            Constraints: new(),
            Labels: new(),
            Resources: new(),
            Result: null,
            WaitForResourcesSynchronization: false,
            MaxTotalRetries: null,
            MaxTimeQueueSeconds: null,
            MaxRetriesPerInstance: null,
            DependsOn: new(),
            Ttl: null,
            ResultTtl: null,
            HardwareConstraints: null,
            SecretsAccessRightsByKey: new(),
            SecretsAccessRightsByPrefix: new(),
            SchedulingType: null,
            MachineTarget: null,
            ReservationTarget: null,
            Periodic: null,
            Whitelist: null,
            Blacklist: null,
            ExportCredentialsToEnv: null,
            ProjectUuid: null
        )
    {
    }
}

public record WaitTasksModel(
    bool Stdout,
    bool Stderr
): GetPoolsOrTasksModel;


public record SnapshotTasksModel(
    uint? Periodic,
    string? Whitelist,
    string? Blacklist,
    string? Bucket
): GetPoolsOrTasksModel;


public record GetTasksOutputModel(
    uint? InstanceId,
    bool Fresh
): GetPoolsOrTasksModel;


public record GetSnapshotStatusModel(
    string SnapshotId
): GetPoolsOrTasksModel;


public record WaitSnapshotModel(
    string SnapshotId,
    int TimeoutSeconds,
    int UpdateIntervalSeconds
): GetPoolsOrTasksModel;


public record TaskSummary(
    string Name,
    string State,
    string Uuid,
    string Shortname,
    string Profile,
    uint InstanceCount,
    Guid? ProjectUuid
);

