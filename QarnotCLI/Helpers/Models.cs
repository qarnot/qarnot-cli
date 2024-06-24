namespace QarnotCLI;

public record GetPoolsOrTasksModel : GlobalModel
{
    private const int DefaultMaxPageSize = 20;
    private const int MaximumMaxPageSize = 50;
    public string? Name { get; private set; }
    public string? Shortname { get; private set; }
    public string? Id { get; private set; }
    public bool NoPaginate { get; private set; }
    public int MaxPageSize { get; private set; }
    public string? NextPageToken { get; private set; }
    public string? CreatedBefore { get; private set; }
    public string? CreatedAfter { get; private set; }
    public string? NamePrefix { get; private set; }
    public bool NextPage { get; private set; }
    public List<string> Tags { get; private set; }
    public List<string> ExclusiveTags { get; private set; }

    public GetPoolsOrTasksModel()
    {
        Tags = new();
        ExclusiveTags = new();
        NoPaginate = false;
        NextPage = false;
        MaxPageSize = DefaultMaxPageSize;
    }

    public GetPoolsOrTasksModel(
        string? name,
        string? shortname,
        string? id,
        List<string> tags,
        List<string> exclusiveTags,
        bool noPaginate,
        string? pageToken,
        bool nextPage,
        int? maxPageSize,
        string? createdBefore,
        string? createdAfter,
        string? namePrefix
    ) : this()
    {
        Initialize(
            name,
            shortname,
            id,
            tags,
            exclusiveTags,
            noPaginate,
            pageToken,
            nextPage,
            maxPageSize,
            createdBefore,
            createdAfter,
            namePrefix);
    }

    public GetPoolsOrTasksModel Initialize(
        string? name,
        string? shortname,
        string? id,
        List<string> tags,
        List<string> exclusiveTags,
        bool noPaginate,
        string? pageToken,
        bool nextPage,
        int? maxPageSize,
        string? createdBefore,
        string? createdAfter,
        string? namePrefix
    )
    {
        Name = name;
        Shortname = shortname;
        Id = id;
        Tags = tags;
        ExclusiveTags = exclusiveTags;
        NoPaginate = noPaginate;
        NextPageToken = pageToken;
        NextPage = nextPage;
        CreatedBefore = createdBefore;
        CreatedAfter = createdAfter;
        NamePrefix = namePrefix;

        if (maxPageSize.HasValue && maxPageSize > 0 )
        {
            MaxPageSize = maxPageSize >= MaximumMaxPageSize ? MaximumMaxPageSize : maxPageSize.Value;
        }

        return this;
    }

    public bool IsTargetingSingleResource()
    {
        if (!string.IsNullOrEmpty(Shortname))
        {
            return true;
        }
        else if (!string.IsNullOrEmpty(Id))
        {
            return true;
        }
        return false;
    }
}

public record UpdatePoolsOrTasksConstantModel(
    string ConstantName,
    string? ConstantValue
): GetPoolsOrTasksModel;

public record ResourcesPageModel<T>(
    List<T> Items,
    int MaxPageSize,
    string? NextPageToken = null
);

public record GetCarbonFactsModel(
    string? EquivalentDataCenterName
): GetPoolsOrTasksModel;