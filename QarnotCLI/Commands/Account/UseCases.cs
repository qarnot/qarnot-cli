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

    public AccountUseCases(
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

    public async Task Get(GlobalModel model)
    {
        Logger.Debug("Retrieving account information");
        var account = await QarnotAPI.RetrieveUserInformationAsync();
        var bytesFormatter = new ByteValueFormatter(model.HumanReadable);

        var outputDictionary = new Dictionary<string,string>()
        {
            { "Email", account.Email },
            { "MaxInstances", account.MaxInstances.ToString() },
            { "Quota Bucket Number", $"{account.BucketCount}/{WrapMaxQuota(account.MaxBucket)}" },
            { "Quota Bytes Bucket", $"{bytesFormatter.Format(account.UsedQuotaBytesBucket)}/{bytesFormatter.Format(account.QuotaBytesBucket)}" },
            { "Quota Task Count", $"{account.TaskCount}/{WrapMaxQuota(account.MaxTask)}" },
            { "Quota Running Task", $"{account.RunningTaskCount}/{WrapMaxQuota(account.MaxRunningTask)}" },
            { "Quota Total Pool", $"{account.RunningPoolCount}/{WrapMaxQuota(account.MaxRunningPool)}" },
            { "Quota Running Instances", $"{account.RunningInstanceCount}/{WrapMaxQuota(account.MaxInstances)}" },
            { "Quota Running Cores", account.RunningCoreCount.ToString() }
        };


        if (account?.ComputingQuotas?.User is not null)
        {
            var computingQuota = account.ComputingQuotas.User;
            outputDictionary.Add("User Flex Instances", $"{computingQuota.Flex.RunningInstancesCount}/{WrapMaxQuota(computingQuota.Flex.MaxInstances)}");
            outputDictionary.Add("User Flex Cores", $"{computingQuota.Flex.RunningCoresCount}/{WrapMaxQuota(computingQuota.Flex.MaxCores)}");

            if (IsOnDemandApplicable(computingQuota.OnDemand.MaxInstances, computingQuota.OnDemand.MaxCores))
            {
                outputDictionary.Add("User OnDemand Instances", $"{computingQuota.OnDemand.RunningInstancesCount}/{WrapMaxQuota(computingQuota.OnDemand.MaxInstances)}");
                outputDictionary.Add("User OnDemand Cores", $"{computingQuota.OnDemand.RunningCoresCount}/{WrapMaxQuota(computingQuota.OnDemand.MaxCores)}");
            }
            else
            {
                outputDictionary.Add("User OnDemand Instances", "NA");
                outputDictionary.Add("User OnDemand Cores", "NA");
            }

            computingQuota.Reserved?.ForEach(quota =>
                    {
                        outputDictionary.Add($"User Reserved {quota.MachineKey}", $"Instances : {quota.RunningInstancesCount}/{WrapMaxQuota(quota.MaxInstances)} , Cores : {quota.RunningCoresCount}/{WrapMaxQuota(quota.MaxCores)}");
                    });
        }
        if (account?.ComputingQuotas?.Organization is not null)
        {
            var computingQuota = account.ComputingQuotas.Organization;
            outputDictionary.Add("Organization Name", computingQuota.Name);
            outputDictionary.Add("Organization Flex Instances", $"{computingQuota.Flex.RunningInstancesCount}/{WrapMaxQuota(computingQuota.Flex.MaxInstances)}");
            outputDictionary.Add("Organization Flex Cores", $"{computingQuota.Flex.RunningCoresCount}/{WrapMaxQuota(computingQuota.Flex.MaxCores)}");

            if (IsOnDemandApplicable(computingQuota.OnDemand.MaxInstances, computingQuota.OnDemand.MaxCores))
            {
                outputDictionary.Add("Organization OnDemand Instances", $"{computingQuota.OnDemand.RunningInstancesCount}/{WrapMaxQuota(computingQuota.OnDemand.MaxInstances)}");
                outputDictionary.Add("Organization OnDemand Cores", $"{computingQuota.OnDemand.RunningCoresCount}/{WrapMaxQuota(computingQuota.OnDemand.MaxCores)}");
            }
            else
            {
                outputDictionary.Add("Organization OnDemand Instances", "NA");
                outputDictionary.Add("Organization OnDemand Cores", "NA");
            }

            computingQuota.Reserved?.ForEach(quota =>
                    {
                        outputDictionary.Add($"Organization Reserved {quota.MachineKey}", $"Instances : {quota.RunningInstancesCount}/{WrapMaxQuota(quota.MaxInstances)}, Cores : {quota.RunningCoresCount}/{WrapMaxQuota(quota.MaxCores)}");
                    });
        }

        Logger.Result(Formatter.FormatCollection(outputDictionary));
    }

    private string WrapMaxQuota(int maxQuota)
    {
        if (maxQuota == 0)
        {
            return "-";
        }
        else
        {
            return maxQuota.ToString();
        }
    }
    
    private bool IsOnDemandApplicable(int maxInstances, int maxCores) => maxInstances != 0 || maxCores != 0;
}
