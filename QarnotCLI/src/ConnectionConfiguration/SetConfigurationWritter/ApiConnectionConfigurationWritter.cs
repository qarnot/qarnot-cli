namespace QarnotCLI
{
    using System;
    using System.IO;
    using System.Text;

    public interface IApiConnectionConfigurationWritter
    {
        void SaveSettings(IConfiguration iconfig);
    }

    /// <summary>
    /// Class to Set The connection Information.
    /// </summary>
    public class ApiConnectionConfigurationWritter : IApiConnectionConfigurationWritter
    {
        private readonly IConfigurationFileGetter EnvironmentPathGetter;

        private readonly IFileStreamWriter FileWriter;

        private readonly IFileWrapper FileWrap;

        private readonly EnvironmentVariableReader GetEnvironmentVariable;

        public ApiConnectionConfigurationWritter(
            IConfigurationFileGetter getEnvPath = null,
            IConfigurationFileReader getFileInformation = null,
            IFileStreamWriter writer = null,
            IFileWrapper fileWrapper = null,
            EnvironmentVariableReader envVariablesReader = null)
        {
            FileWriter = writer ?? new FileStreamWriter();
            EnvironmentPathGetter = getEnvPath ?? new ConfigurationFileGetter();
            FileInformationGetter = getFileInformation ?? new ConfigurationFileReader();
            FileWrap = fileWrapper ?? new FileWrapper();
            GetEnvironmentVariable = envVariablesReader ?? new EnvironmentVariableReader();
        }

        public IConfigurationFileReader FileInformationGetter { get; }

        public string GetPath(LocalSetUpConfiguration config)
        {
            if (config.Path != null)
            {
                return config.Path;
            }
            else if (config.GlobalPath)
            {
                return QarnotCLI.ConfigurationFileGetter.DefaultPathName;
            }

            if (this.EnvironmentPathGetter.CanConfigurationFileBeCreated())
            {
                return this.EnvironmentPathGetter.NewConfigurationFilePath;
            }

            throw new FileNotFoundException("Impossible to create a new configuration file.");
        }

        /// <summary>
        /// Set The connection Information to the computer.
        /// </summary>
        /// <param name="iconfig">Configuration Object.</param>
        public void SaveSettings(IConfiguration iconfig)
        {
            CLILogs.Debug("Set the config value");

            string path = this.GetPath((LocalSetUpConfiguration)iconfig);
            this.SetConfigInformation(path, iconfig);
        }

        private void PrintInformationFiles(string path, APIConnectionInformation connectionInformation)
        {
            string lines = $"Configuration on the file '{path}'";
            CLILogs.Info(lines);
            CLILogs.Info(connectionInformation.ToString());
        }

        public void CheckDirectory(string path)
        {
            string dir = FileWrap.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(dir) &&
                !FileWrap.DirectoryExist(dir))
            {
                FileWrap.CreateDirectory(dir);
            }
        }

        public void SetConfigInformation(string path, IConfiguration config)
        {
            APIConnectionInformation info = new APIConnectionInformation();

            if (FileWrap.DoesFileExist(path))
            {
                info = this.FileInformationGetter.ReadFile(path);
            }
            else
            {
                this.CheckDirectory(path);
            }

            var apiConfig = config as LocalSetUpConfiguration;
            config.ApiConnection.Update(info);
            this.SetConfigFile(path, config.ApiConnection);
        }

        private void SetConfigFile(string path, APIConnectionInformation connectionInformation)
        {
            PrintInformationFiles(path, connectionInformation);
            using (FileStream fs = FileWrap.CreateFile(path))
            {
                FileWriter.Write("token", connectionInformation.Token, fs);
                FileWriter.Write("uri", connectionInformation.ApiUri, fs);
                FileWriter.Write("storage", connectionInformation.StorageUri, fs);
                FileWriter.Write("account-email", connectionInformation.AccountEmail, fs);
                if (connectionInformation.GetForcePathStyle.HasValue)
                {
                    FileWriter.Write("force-path", connectionInformation.ForcePathStyle.ToString(), fs);
                }
                if (connectionInformation.GetDisableBucketPathsSanitization.HasValue)
                {
                    FileWriter.Write("disable-path-sanitization", connectionInformation.DisableBucketPathsSanitization.ToString(), fs);
                }
            }
        }
    }
}
