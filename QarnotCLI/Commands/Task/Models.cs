using Newtonsoft.Json;

namespace QarnotCLI;

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
    [property: JsonProperty("MaxRetriesPerInstance")]
    uint? MaxRetriesPerInstance,
    [property: JsonProperty("Dependents")]
    List<string> Dependents,
    [property: JsonProperty("DefaultResourcesCacheTTLSec")]
    uint? Ttl,
    [property: JsonProperty("SecretsAccessRightsByKey")]
    List<string> SecretsAccessRightsByKey,
    [property: JsonProperty("SecretsAccessRightsByPrefix")]
    List<string> SecretsAccessRightsByPrefix,
    [property: JsonProperty("SchedulingType")]
    string? SchedulingType,
    [property: JsonProperty("TargetedReservedMachineKey")]
    string? MachineTarget,
    [property: JsonProperty("SnapshotPeriodicSec")]
    uint? Periodic,
    [property: JsonProperty("Whitelist")]
    string? Whitelist,
    [property: JsonProperty("Blacklist")]
    string? Blacklist,
    [property: JsonProperty("ExportApiAndStorageCredentialsInEnvironment")]
    bool? ExportCredentialsToEnv
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
            MaxRetriesPerInstance: null,
            Dependents: new(),
            Ttl: null,
            SecretsAccessRightsByKey: new(),
            SecretsAccessRightsByPrefix: new(),
            SchedulingType: null,
            MachineTarget: null,
            Periodic: null,
            Whitelist: null,
            Blacklist: null,
            ExportCredentialsToEnv: null
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


public record TaskSummary(
    string Name,
    string State,
    string Uuid,
    string Shortname,
    string Profile,
    uint InstanceCount
);

