namespace QarnotCLI;

public record GetPoolsOrTasksModel : GlobalModel
{
    public string? Name { get; private set; }
    public string? Id { get; private set; }
    public List<string> Tags { get; private set; }
    public List<string> ExclusiveTags { get; private set; }

    public GetPoolsOrTasksModel()
    {
        Tags = new();
        ExclusiveTags = new();
    }

    public GetPoolsOrTasksModel(
        string? name,
        string? id,
        List<string> tags,
        List<string> exclusiveTags
    ) : this()
    {
        Initialize(name, id, tags, exclusiveTags);
    }

    public GetPoolsOrTasksModel Initialize(
        string? name,
        string? id,
        List<string> tags,
        List<string> exclusiveTags
    )
    {
        Name = name;
        Id = id;
        Tags = tags;
        ExclusiveTags = exclusiveTags;

        return this;
    }
}

public record UpdatePoolsOrTasksConstantModel(
    string ConstantName,
    string? ConstantValue
): GetPoolsOrTasksModel;

