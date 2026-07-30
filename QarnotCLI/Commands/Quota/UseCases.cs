using QarnotSDK;

namespace QarnotCLI;

public interface IQuotaUseCases
{
    Task Get(GetQuotaModel model);
}

public class QuotaUseCases : IQuotaUseCases
{
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly ILogger Logger;

    public QuotaUseCases(
        Connection api,
        IFormatter formatter,
        IStateManager _,
        ILogger logger)
    {
        QarnotAPI = api;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task Get(GetQuotaModel model)
    {
        if (model.Scope == "organization")
        {
            Logger.Debug("Retrieving organization computing quotas usage");
            var quotas = await QarnotAPI.RetrieveOrganizationComputingQuotasUsageAsync();
            Logger.Result(Formatter.Format(quotas));
        }
        else
        {
            Logger.Debug("Retrieving user computing quotas usage");
            var quotas = await QarnotAPI.RetrieveUserComputingQuotasUsageAsync();
            Logger.Result(Formatter.Format(quotas));
        }
    }
}
