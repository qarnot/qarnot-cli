namespace QarnotCLI
{
    public interface IApiDataManager
    {
        bool Start(IConfiguration config);
    }

    public class ApiDataManager : IApiDataManager
    {
        private readonly IApiConnectionConfigurationRetriever GetConfig;

        private readonly IApiConnectionConfigurationWritter SetConfig;

        private ApiDataManager()
        {
        }

        public ApiDataManager(IApiConnectionConfigurationRetriever getConfig, IApiConnectionConfigurationWritter setConfig)
        {
            GetConfig = getConfig;
            SetConfig = setConfig;
        }

        /// <summary>
        /// if Config and Set is true, set the connections info and exit.
        /// else get the config values and return an IConfiguration object completed.
        /// </summary>
        /// <param name="config">Configuration Object.</param>
        /// <returns>null if you set a config, IConfiguration if you launch a command.</returns>
        public bool Start(IConfiguration config)
        {
            if (config.Type == ConfigType.Config && config.Command == CommandApi.Set)
            {
                this.SetConfig.SaveSettings(config);
                return false;
            }

            this.AddConnectionInformation(config);
            return true;
        }

        private void AddConnectionInformation(IConfiguration config)
        {
            CLILogs.Debug("Get the configuration info");

            config.ApiConnection = this.GetConfig.RetrieveConfigurationInformation(config.ApiConnection);
        }
    }
}
