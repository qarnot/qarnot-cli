namespace QarnotCLI
{
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class GetAccountInformationCommand : ICommand<UserInformation, CommandValues.UserInformationCommandValue>
    {
        private GetAccountInformationCommand()
        {
        }

        public GetAccountInformationCommand(IModelMapper<UserInformation> infoObj, IFormatter bytesFormater)
        {
            this.InfoObj = infoObj;
            this.BytesFormat = bytesFormater;

        }

        private IModelMapper<UserInformation> InfoObj { get; }

        private IFormatter BytesFormat { get; }

        public virtual Task<CommandValues.UserInformationCommandValue> ExecuteAsync(UserInformation apiSettings, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("print Account info");

            CommandValues.UserInformationCommandValue objToPrint = InfoObj.CreateObj(apiSettings) as CommandValues.UserInformationCommandValue;
            objToPrint.AccountInformation["Email"] = apiSettings.Email;
            objToPrint.AccountInformation["MaxInstances"] = apiSettings.MaxInstances.ToString();
            objToPrint.AccountInformation["Quota Bucket Number"] = apiSettings.BucketCount.ToString() + "/" + apiSettings.MaxBucket.ToString();
            if (iconfig.HumanReadable)
            {
                objToPrint.AccountInformation["Quota Bytes Bucket"] = BytesFormat.ConvertToHumanReadable(apiSettings.UsedQuotaBytesBucket) + "/" + BytesFormat.ConvertToHumanReadable(apiSettings.QuotaBytesBucket);
            }
            else
            {
                objToPrint.AccountInformation["Quota Bytes Bucket"] = apiSettings.UsedQuotaBytesBucket.ToString() + "/" + apiSettings.QuotaBytesBucket.ToString();
            }
            objToPrint.AccountInformation["Quota Task Count"] = apiSettings.TaskCount.ToString() + "/" + apiSettings.MaxTask.ToString();
            objToPrint.AccountInformation["Quota Running Task"] = apiSettings.RunningTaskCount.ToString() + "/" + apiSettings.MaxRunningTask.ToString();
            objToPrint.AccountInformation["Quota Total Pool"] = apiSettings.PoolCount.ToString() + "/" + apiSettings.MaxPool.ToString();
            objToPrint.AccountInformation["Quota Running Pool"] = apiSettings.RunningPoolCount.ToString() + "/" + apiSettings.MaxRunningPool.ToString();
            objToPrint.AccountInformation["Quota Running Instances"] = apiSettings.RunningInstanceCount.ToString() + "/" + apiSettings.MaxInstances.ToString();
            objToPrint.AccountInformation["Quota Running Cores"] = apiSettings.RunningCoreCount.ToString();

            return Task.FromResult(objToPrint);
        }
    }

    public class ApiSettingsCommand : ICommand<ApiSettings, ApiSettings>
    {
        public ApiSettingsCommand()
        {
        }

        public virtual Task<ApiSettings> ExecuteAsync(ApiSettings apiSettings, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("print Account info");

            return Task.FromResult(apiSettings);
        }
    }
}