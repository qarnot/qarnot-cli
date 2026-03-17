using QarnotSDK;

namespace QarnotCLI;

public record ProjectSummary(
    string Name,
    string Uuid
);

public record ProjectDetail(
    string Uuid,
    string Name,
    string OrganizationUuid,
    string Description,
    string Slug
);

public interface IProjectUseCases
{
    Task List(GlobalModel model);
}

public class ProjectUseCases : IProjectUseCases
{
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly ILogger Logger;

    public ProjectUseCases(
        Connection api,
        IFormatter formatter,
        IStateManager _,
        ILogger logger
    )
    {
        QarnotAPI = api;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task List(GlobalModel model)
    {
        Logger.Debug("Retrieving available projects");
        var account = await QarnotAPI.RetrieveUserInformationAsync();
        
        var projectDetails = account.Projects
            .Select(p => new ProjectDetail(
                p.Uuid.ToString(),
                p.Name,
                p.OrganizationUuid.ToString(),
                p.Description,
                p.Slug))
            .ToList();
        Logger.Result(Formatter.FormatCollection(projectDetails));
    }
}
