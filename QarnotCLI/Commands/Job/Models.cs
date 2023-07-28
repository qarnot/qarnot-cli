using Newtonsoft.Json;

namespace QarnotCLI;

[JsonObject(MemberSerialization.OptIn)]
public record CreateJobModel(
    [property: JsonProperty("Name")]
    string Name,
    [property: JsonProperty("Shortname")]
    string? Shortname,
    [property: JsonProperty("PoolUuidOrShortname")]
    string? Pool,
    [property: JsonProperty("IsDependent")]
    bool IsDependent,
    [property: JsonProperty("MaximumWallTime")]
    string? MaxWallTimeStr,
    TimeSpan? MaxWallTime
) : GlobalModel
{
    public CreateJobModel()
        : this("", null, null, false, null, null)
    {
    }
}

public record GetJobModel(
    bool All,
    string? Id,
    string? Name
) : GlobalModel;

public record JobSummary(
    string Name,
    string State,
    string Uuid,
    string Shortname
);
