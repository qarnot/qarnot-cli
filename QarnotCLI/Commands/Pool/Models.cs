using Newtonsoft.Json;

namespace QarnotCLI;

[JsonObject(MemberSerialization.OptIn)]
public record CreatePoolModel(
    [property: JsonProperty("Name")]
    string Name,
    [property: JsonProperty("Shortname")]
    string? Shortname,
    [property: JsonProperty("Profile")]
    string Profile,
    [property: JsonProperty("InstanceCount")]
    uint InstanceCount,
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
    [property: JsonProperty("TaskDefaultWaitForPoolResourcesSynchronization")]
    bool TasksWaitForSynchronization,
    [property: JsonProperty("DefaultResourcesCacheTTLSec")]
    uint? Ttl,
    [property: JsonProperty("MaxTotalRetries")]
    uint? MaxTotalRetries,
    [property: JsonProperty("MaxRetriesPerInstance")]
    uint? MaxRetriesPerInstance,
    [property: JsonProperty("MaxTimeQueueSeconds")]
    uint? MaxTimeQueueSeconds,
    [property: JsonProperty("Scaling")]
    QarnotSDK.Scaling? Scaling,
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
    [property: JsonProperty("ExportApiAndStorageCredentialsInEnvironment")]
    bool? ExportCredentialsToEnv,
    [property: JsonProperty("SlotsPerNode")]
    uint? SlotsPerNode,
    [property: JsonProperty("ProjectUuid")]
    Guid? ProjectUuid
): GlobalModel
{
    public CreatePoolModel()
        : this(
            Name: "",
            Shortname: null,
            Profile: "",
            InstanceCount: 0,
            Tags: new(),
            Constants: new(),
            Constraints: new(),
            Labels: new(),
            Resources: new(),
            TasksWaitForSynchronization: false,
            Ttl: null,
            MaxTotalRetries: null,
            MaxRetriesPerInstance: null,
            MaxTimeQueueSeconds: null,
            Scaling: null,
            HardwareConstraints: null,
            SecretsAccessRightsByKey: new(),
            SecretsAccessRightsByPrefix: new(),
            SchedulingType: null,
            MachineTarget: null,
            ExportCredentialsToEnv: null,
            SlotsPerNode: null,
            ProjectUuid: null
        )
    {
    }
}


public record UpdatePoolScalingModel(
    QarnotSDK.Scaling? Scaling
): GetPoolsOrTasksModel;


public record PoolSummary(
    string Name,
    string Shortname,
    string Profile,
    string State,
    DateTime CreationDate,
    int QueuedOrRunningTaskInstancesCount,
    bool? TaskDefaultWaitForPoolResourcesSynchronization,
    Guid? ProjectUuid
);
