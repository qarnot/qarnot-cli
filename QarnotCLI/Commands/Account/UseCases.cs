using QarnotSDK;

namespace QarnotCLI;

public interface IAccountUseCases
{
    Task Get(GlobalModel model);
}

public class AccountUseCases : IAccountUseCases
{
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly ILogger Logger;

    public AccountUseCases(Connection api, IFormatter formatter, ILogger logger)
    {
        QarnotAPI = api;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task Get(GlobalModel model)
    {
        Logger.Debug("Retrieving account information");
        var account = await QarnotAPI.RetrieveUserInformationAsync();
        var bytesFormatter = new ByteValueFormatter(model.HumanReadable);
        Logger.Result(Formatter.Format(new Dictionary<string, string>
        {
            { "Email", account.Email },
            { "MaxInstances", account.MaxInstances.ToString() },
            { "Quota Bucket Number", $"{account.BucketCount}/{account.MaxBucket}" },
            { "Quota Bytes Bucket", $"{bytesFormatter.Format(account.UsedQuotaBytesBucket)}/{bytesFormatter.Format(account.QuotaBytesBucket)}" },
            { "Quota Task Count", $"{account.TaskCount}/{account.MaxTask}" },
            { "Quota Running Task", $"{account.RunningTaskCount}/{account.MaxRunningTask}" },
            { "Quota Total Pool", $"{account.RunningPoolCount}/{account.MaxRunningPool}" },
            { "Quota Running Instances", $"{account.RunningInstanceCount}/{account.MaxInstances}" },
            { "Quota Running Cores", account.RunningCoreCount.ToString() }
        }));
    }
}
