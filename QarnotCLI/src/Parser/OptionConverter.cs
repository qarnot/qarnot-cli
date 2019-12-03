namespace QarnotCLI
{
    using System;
    using System.Linq;

    // TODO: what is the best way to split that
    public interface IOptionToConfigConverter
    {
        IConfiguration ConvertAccountOption(ConfigType type, Options.AccountOptions option);

        DefaultRunConfiguration ConvertAllGetterOption(ConfigType type, Options.AllObjectsOptions option);

        BucketConfiguration ConvertBucketOption(ConfigType type, CommandApi command, Options.IBucketGetOptions option);

        PoolSetConfiguration ConvertElasticPoolSetterOption(ConfigType type, CommandApi command, Options.SetPoolOptions option);

        DefaultRunConfiguration ConvertGenericGetterOption(ConfigType type, CommandApi command, Options.IGetOptions option);

        LocalSetUpConfiguration ConvertGenericSetterOption(ConfigType type, Options.ConfigOptions option);

        CreateConfiguration ConvertGenericCreationOption(ConfigType type, Options.ICreateOptions option);
    }

    public class OptionConverter : IOptionToConfigConverter
    {
        private readonly IDeserializer Deserializer;

        private OptionConverter()
        {
        }

        public OptionConverter(IDeserializer deserializer)
        {
            this.Deserializer = deserializer;
        }

        public void ConfigGetVerboseOptions(Options.IVerboseOptions option)
        {
            if (option.Verbose)
            {
                CLILogs.ChangeVerboseLevel(1);
            }
            else if (option.Quiet)
            {
                CLILogs.ChangeVerboseLevel(5);
            }
        }

        public void ConfigGetLogOptions(Options.ILogOptions option)
        {
            CLILogs.SetNoColor(option.NoColor);

            CLILogs.CreateLoggers();
        }

        private void ConfigPrintInformation(Options.IOptions option, ConfigType type, string command)
        {
            CLILogs.Debug("Command found " + Enum.GetName(typeof(ConfigType), type) + " - " + command + " on format " + option.ResultFormat);
        }

        private void ConfigGetGlobalOptions(Options.IOptions option)
        {
            ConfigGetVerboseOptions(option);
            ConfigGetLogOptions(option);
        }

        public T ConfigReadFile<T>(string filePath)
        {
            string jsonFile = this.Deserializer.GetFile(filePath);

            T config = this.Deserializer.Deserialize<T>(jsonFile);

            return config;
        }

        public T ReadConfigurationFile<T>(string filePath, ConfigType type, CommandApi command) where T : IConfiguration
        {
            T config = ConfigReadFile<T>(filePath);
            var setconfig = config as IConfiguration;
            setconfig.Type = type;
            setconfig.Command = command;
            return config;
        }

        private string SetResultFormat(string resultFormat)
        {
            if (!string.IsNullOrWhiteSpace(resultFormat))
            {
                resultFormat = resultFormat.ToUpper();
                FormatterFactory.CheckFormat(resultFormat);
                return resultFormat;
            }

            return FormatterFactory.DefaultResultFormat();
        }

        public void GetDefaultOptions(IConfiguration config, Options.IOptions option)
        {
            config.ResultFormat = SetResultFormat(option.ResultFormat);
            config.HumanReadable = option.HumanReadable;
            config.ApiConnection.Token = option.Token;
        }

        public IConfiguration ConvertAccountOption(ConfigType type, Options.AccountOptions option)
        {
            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, string.Empty);

            AccountConfiguration config = new AccountConfiguration(type, CommandApi.Info);
            GetDefaultOptions(config, option);
            config.GetBucket = true;

            return config;
        }

        public DefaultRunConfiguration ConvertAllGetterOption(ConfigType type, Options.AllObjectsOptions option)
        {
            ConfigGetGlobalOptions(option);
            CommandApi command = CommandApi.List;
            if (option.Delete)
            {
                command = CommandApi.Delete;
            }
            else if (option.Abort)
            {
                command = CommandApi.Abort;
            }
            else if (option.List)
            {
                command = CommandApi.List;
            }

            DefaultRunConfiguration config = new DefaultRunConfiguration(type, command);
            GetDefaultOptions(config, option);

            ConfigPrintInformation(option, type, Enum.GetName(typeof(CommandApi), command));
            return config;
        }

        public BucketConfiguration ConvertBucketOption(ConfigType type, CommandApi command, Options.IBucketGetOptions option)
        {
            if (command == CommandApi.List && !string.IsNullOrEmpty(option.Name))
            {
                command = CommandApi.InfoBucket;
            }

            BucketConfiguration config = new BucketConfiguration(type, command);
            GetDefaultOptions(config, option);

            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, Enum.GetName(typeof(CommandApi), command));
            config.Name = option.Name;

            if (option is Options.CreateBucketOptions bucketCreate)
            {
                config.LocalPathFolders = bucketCreate.FolderList.ToList();
                config.LocalPathFiles = bucketCreate.FileList.ToList();
            }
            else if (option is Options.SetBucketOptions bucketSet)
            {
                config.LocalPathFolders = bucketSet.LocalFoldersPath.ToList();
                config.LocalPathFiles = bucketSet.LocalFilesPath.ToList();
                config.RemoteRelativePath = bucketSet.RemoteFolderPath;
                config.StringPath = bucketSet.RemoteFolderPath;
                config.String = bucketSet.String;
            }
            else if (option is Options.GetBucketOptions bucketGet)
            {
                config.LocalPathGet = bucketGet.LocalPath;
                config.RemoteRelativePathFolders = bucketGet.RemoteFoldersPath.ToList();
                config.RemoteRelativePathFiles = bucketGet.RemoteFilesPath.ToList();
                config.StringPath = bucketGet.String;
            }
            else if (option is Options.ListBucketOptions bucketInfo)
            {
                config.RemoteRelativePath = bucketInfo.RemoteFolderPath;
            }
            else if (option is Options.SyncFromBucketOptions bucketFrom)
            {
                config.LocalPathGet = bucketFrom.LocalPath;
                config.RemoteRelativePath = bucketFrom?.RemoteFolderPath ?? string.Empty;
            }
            else if (option is Options.SyncToBucketOptions bucketTo)
            {
                config.LocalPathGet = bucketTo.LocalPath;
                config.RemoteRelativePath = bucketTo?.RemoteFolderPath ?? string.Empty;
            }

            return config;
        }

        public DefaultRunConfiguration ConvertGenericGetterOption(ConfigType type, CommandApi command, Options.IGetOptions option)
        {
            DefaultRunConfiguration config = new DefaultRunConfiguration(type, command);
            GetDefaultOptions(config, option);

            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, Enum.GetName(typeof(CommandApi), command));
            config.Name = option.Name;
            config.Id = option.Id;
            config.Summaries = false;
            config.Tags = option.Tags?.ToList();

            return config;
        }

        public LocalSetUpConfiguration ConvertGenericSetterOption(ConfigType type, Options.ConfigOptions option)
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration(type, CommandApi.Set);
            GetDefaultOptions(config, option);

            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, string.Empty);
            config.GlobalPath = option.Global;
            config.ApiConnection.Token = option.Token;
            config.ApiConnection.ApiUri = option.Uri;
            config.ApiConnection.StorageUri = option.Storage;
            return config;
        }

        public PoolSetConfiguration ConvertElasticPoolSetterOption(ConfigType type, CommandApi command, Options.SetPoolOptions option)
        {
            PoolSetConfiguration config = new PoolSetConfiguration(type, CommandApi.Set);
            GetDefaultOptions(config, option);

            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, string.Empty);
            config.Name = option.Name;
            config.Id = option.Id;
            config.Summaries = false;
            config.Tags = option.Tags?.ToList();

            config.ElasticMinimumTotalNodes = option.ElasticMinimumTotalNodes;
            config.ElasticMaximumTotalNodes = option.ElasticMaximumTotalNodes;
            config.ElasticMinimumIdlingNodes = option.ElasticMinimumIdlingNodes;
            config.ElasticResizePeriod = option.ElasticResizePeriod;
            config.ElasticResizeFactor = option.ElasticResizeFactor;
            config.ElasticMinimumIdlingTime = option.ElasticMinimumIdlingTime;
            return config;
        }

        public CreateConfiguration ConvertGenericCreationOption(ConfigType type, Options.ICreateOptions option)
        {
            ConfigGetGlobalOptions(option);
            CreateConfiguration config = new CreateConfiguration(type, CommandApi.Create);
            if (!string.IsNullOrEmpty(option.FileConf))
            {
                try
                {
                    config = this.ReadConfigurationFile<CreateConfiguration>(option.FileConf, type, CommandApi.Create);
                }
                catch(System.IO.FileNotFoundException ex)
                {
                    CLILogs.Error(ex.Message);
                }
            }

            GetDefaultOptions(config, option);
            ConfigPrintInformation(option, type, "create");

            config.Name = option.Name ?? config.Name;
            config.Shortname = option.Shortname ?? config.Shortname;
            config.Range = option.Range ?? config.Range;
            config.Profile = option.Profile ?? config.Profile;
            config.Tags = option.Tags?.ToList().Count > 0 ? option.Tags?.ToList() : config.Tags ;
            config.Constants = option.Constants?.ToList().Count > 0 ? option.Constants?.ToList() : config.Constants;
            config.Resources = option.Resources?.ToList().Count > 0 ? option.Resources?.ToList() : config.Resources;
            config.InstanceCount = option.InstanceCount > 0 ? option.InstanceCount : config.InstanceCount;
            config.JobUuid = option.Job ?? config.JobUuid;
            config.PoolUuid = option.Pool ?? config.PoolUuid;
            config.IsElastic = option.IsElastic ? option.IsElastic : config.IsElastic;
            config.IsDependents = option.IsDependents ? option.IsDependents : config.IsDependents;
            config.Result = option.Result ?? config.Result;
            config.Dependents = option.Dependents?.ToList().Count > 0 ? option.Dependents?.ToList() : config.Dependents;
            Options.IElasticityOptions elasticOption = option as Options.IElasticityOptions;
            if (elasticOption != null)
            {
                config.ElasticMinimumTotalNodes = elasticOption.ElasticMinimumTotalNodes;
                config.ElasticMaximumTotalNodes = elasticOption.ElasticMaximumTotalNodes;
                config.ElasticMinimumIdlingNodes = elasticOption.ElasticMinimumIdlingNodes;
                config.ElasticResizePeriod = elasticOption.ElasticResizePeriod;
                config.ElasticResizeFactor = elasticOption.ElasticResizeFactor;
                config.ElasticMinimumIdlingTime = elasticOption.ElasticMinimumIdlingTime;
            }

            return config;
        }
    }
}