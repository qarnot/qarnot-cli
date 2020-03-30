namespace QarnotCLI
{
    using System.IO;

    public interface IApiConnectionConfigurationRetriever
    {
        APIConnectionInformation RetrieveConfigurationInformation(APIConnectionInformation config);
    }

    /// <summary>
    /// The General parser verb, it's used if there are not sous verbs.
    /// </summary>
    public class ApiConnectionConfigurationRetriever : IApiConnectionConfigurationRetriever
    {
        private readonly IConfigurationFileGetter GetFilePath;

        private readonly IConfigurationFileReader FileInformationGetter;

        private readonly IEnvironmentVariableReader GetEnvConnectionInformation;

        public ApiConnectionConfigurationRetriever(IConfigurationFileGetter getFilePath, IEnvironmentVariableReader getEnvConnectionInformation, IConfigurationFileReader getFileInformation)
        {
            GetFilePath = getFilePath;
            GetEnvConnectionInformation = getEnvConnectionInformation;
            FileInformationGetter = getFileInformation;
        }

        /// <summary>
        /// Add the miss information to the APIConnectionInformation.
        /// </summary>
        /// <param name="apiInfo">Object contain the Api info form the env and file.</param>
        /// <returns>Object contain the Api info.</returns>
        public APIConnectionInformation RetrieveConfigurationInformation(APIConnectionInformation apiInfo)
        {
            GetEnvConnectionInformation.RetrieveEnvironmentInformation(apiInfo);
            if (apiInfo.IsComplete())
            {
                return apiInfo;
            }

            try
            {
                string configFilePath = this.GetConfigFilePath();
                APIConnectionInformation fileInfo = this.FileInformationGetter.ReadFile(configFilePath);
                apiInfo.Update(fileInfo);
            }
            catch (FileNotFoundException)
            {
                if (string.IsNullOrEmpty(apiInfo.Token))
                {
                    throw;
                }
            }

            return apiInfo;
        }

        private string GetConfigFilePath()
        {
            if (this.GetFilePath.DoesConfigurationFileExist())
            {
                return this.GetFilePath.ConfigurationFilePath;
            }

            throw new FileNotFoundException("No configuration file find. Launch 'qarnot config -g -t [token]' to create a new configuration file or use the qarnot environment variable QARNOT_CLIENT_TOKEN, or precise the configuration file path in the env variable: QARNOT_LOCAL_PATH");
        }
    }
}
