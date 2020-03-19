namespace QarnotCLI
{
    using System;
    using System.IO;

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

        public ApiConnectionConfigurationWritter(
            IConfigurationFileGetter getEnvPath = null,
            IConfigurationFileReader getFileInformation = null,
            IFileStreamWriter writer = null,
            IFileWrapper fileWrapper = null)
        {
            FileWriter = writer ?? new FileStreamWriter();
            EnvironmentPathGetter = getEnvPath ?? new ConfigurationFileGetter();
            FileInformationGetter = getFileInformation ?? new ConfigurationFileReader();
            FileWrap = fileWrapper ?? new FileWrapper();
        }

        public IConfigurationFileReader FileInformationGetter { get; }

        private void PrintTraceOfTheValueFinds(string path, string token, string uri, string bucketUri)
        {
            CLILogs.Debug("Path " + path);
            CLILogs.Debug("Set the token " + token);
            CLILogs.Debug("Set the api uri " + uri);
            CLILogs.Debug("Set the storage uri " + bucketUri);
        }

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
            string token = iconfig.ApiConnection.Token ?? default(string);
            string uri = iconfig.ApiConnection.ApiUri ?? default(string);
            string bucketUri = iconfig.ApiConnection.StorageUri ?? default(string);

            this.SetConfigInformation(path, token, uri, bucketUri);
        }

        private void PrintInformationFiles(string path, string token, string uri, string bucketUri)
        {
            string lines = $"Configuration set on the file '{path}'" + Environment.NewLine + "token:{token}" + Environment.NewLine + "uri:{uri}" + Environment.NewLine + "storage:{bucketUri}";
            CLILogs.Info(lines);
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

        public void SetConfigInformation(string path, string token, string uri, string bucketUri)
        {
            if (FileWrap.DoesFileExist(path))
            {
                APIConnectionInformation info = this.FileInformationGetter.ReadFile(path);

                token = token ?? info.Token;
                uri = uri ?? info.ApiUri;
                bucketUri = bucketUri ?? info.StorageUri;
            }
            else
            {
                this.CheckDirectory(path);
            }

            this.CheckUriValuesAndSetConfigFile(path, token, uri, bucketUri);
        }

        public void CheckUriValuesAndSetConfigFile(string path, string token, string uri, string bucketUri)
        {
            uri = string.IsNullOrWhiteSpace(uri) ? null : uri;

            bucketUri = string.IsNullOrWhiteSpace(bucketUri) ? null : bucketUri;

            this.SetConfigFile(path, token, uri, bucketUri);
        }

        private void SetConfigFile(string path, string token, string uri, string bucketUri)
        {
            PrintInformationFiles(path, token, uri, bucketUri);
            using (FileStream fs = FileWrap.CreateFile(path))
            {
                FileWriter.Write("token", token, fs);
                FileWriter.Write("uri", uri, fs);
                FileWriter.Write("storage", bucketUri, fs);
            }
        }
    }
}
