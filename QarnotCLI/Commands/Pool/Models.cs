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
    [property: JsonProperty("IsElastic")]
    bool IsElastic,
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
    [property: JsonProperty("ElasticMinimumTotalSlots")]
    uint? ElasticMinSlots,
    [property: JsonProperty("ElasticMaximumTotalSlots")]
    uint? ElasticMaxSlots,
    [property: JsonProperty("ElasticMinimumIdlingSlots")]
    uint? ElasticMinIdlingSlots,
    [property: JsonProperty("ElasticResizePeriod")]
    uint? ElasticResizePeriod,
    [property: JsonProperty("ElasticResizeFactor")]
    float? ElasticResizeFactor,
    [property: JsonProperty("ElasticMinimumIdlingTime")]
    uint? ElasticMinIdlingTime,
    [property: JsonProperty("TaskDefaultWaitForPoolResourcesSynchronization")]
    bool TasksWaitForSynchronization,
    [property: JsonProperty("DefaultResourcesCacheTTLSec")]
    uint? Ttl,
    [property: JsonProperty("MaxTotalRetries")]
    uint? MaxTotalRetries,
    [property: JsonProperty("MaxRetriesPerInstance")]
    uint? MaxRetriesPerInstance,
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
    bool? ExportCredentialsToEnv
): GlobalModel
{
    public CreatePoolModel()
        : this(
            Name: "",
            Shortname: null,
            Profile: "",
            InstanceCount: 0,
            IsElastic: false,
            Tags: new(),
            Constants: new(),
            Constraints: new(),
            Labels: new(),
            Resources: new(),
            ElasticMinSlots: null,
            ElasticMaxSlots: null,
            ElasticMinIdlingSlots: null,
            ElasticResizePeriod: null,
            ElasticResizeFactor: null,
            ElasticMinIdlingTime: null,
            TasksWaitForSynchronization: false,
            Ttl: null,
            MaxTotalRetries: null,
            MaxRetriesPerInstance: null,
            Scaling: null,
            HardwareConstraints: null,
            SecretsAccessRightsByKey: new(),
            SecretsAccessRightsByPrefix: new(),
            SchedulingType: null,
            MachineTarget: null,
            ExportCredentialsToEnv: null
        )
    {
    }
}

public record UpdatePoolElasticSettingsModel(
    uint? MinSlots,
    uint? MaxSlots,
    uint? MinIdlingSlots,
    uint? ResizePeriod,
    float? ResizeFactor,
    uint? MinIdlingTime
): GetPoolsOrTasksModel;


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
    bool? TaskDefaultWaitForPoolResourcesSynchronization
);
