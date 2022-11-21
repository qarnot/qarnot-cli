namespace QarnotCLI
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    // TODO: what is the best way to split that
    public interface IOptionToConfigConverter
    {
        IConfiguration ConvertAccountOption(ConfigType type, Options.AccountOptions option);

        DefaultRunConfiguration ConvertAllGetterOption(ConfigType type, Options.AllObjectsOptions option);

        BucketConfiguration ConvertBucketOption(ConfigType type, CommandApi command, Options.IBucketGetOptions option);

        PoolSetElasticSettingsConfiguration ConvertElasticPoolSetterOption(ConfigType type, CommandApi command, Options.PoolElasticSettingsOptions option);

        PoolSetScalingConfiguration ConvertScalingSetterOption(ConfigType type, CommandApi command, Options.SetPoolScalingOptions option);

        DefaultRunConfiguration ConvertGenericGetterOption(ConfigType type, CommandApi command, Options.IGetOptions option);

        StdConfiguration ConvertStdOption(ConfigType type, CommandApi command, Options.AStdOptions option);

        ConstantUpdateConfiguration ConvertConstantOption(ConfigType type, CommandApi command, Options.IConstantOptions option);

        SnapshotConfiguration ConvertSnapshotOption(ConfigType type, CommandApi command, Options.SnapshotTaskOptions option);

        LocalSetUpConfiguration ConvertGenericSetterOption(ConfigType type, Options.ConfigOptions option);

        CreateConfiguration ConvertGenericCreationOption(ConfigType type, Options.ICreateOptions option);

        CreateConfiguration ConvertTaskCreationOption(ConfigType type, Options.CreateTaskOptions option);
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
            config.UnsafeSslCertificate = option.UnsafeSslCertificate;
            config.CustomSslCertificate = option.CustomSslCertificate;
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

            if (option is Options.TerminateBucketOptions bucketDelete)
            {
                if (bucketDelete.BucketPath.Count() > 0)
                {
                    config.Command = CommandApi.Remove;
                    config.RemoteRelativePaths = bucketDelete.BucketPath.ToList();
                }
            }
            else if (option is Options.CreateBucketOptions bucketCreate)
            {
                config.LocalPathFolders = bucketCreate.FolderList.ToList();
                config.LocalPathFiles = bucketCreate.FileList.ToList();
                config.CacheTTL = bucketCreate.CacheTTL;
            }
            else if (option is Options.SetBucketOptions bucketSet)
            {
                config.LocalPathFolders = bucketSet.LocalFoldersPath.ToList();
                config.LocalPathFiles = bucketSet.LocalFilesPath.ToList();
                config.RemoteRelativePath = bucketSet.RemoteFolderPath;
                config.StringPath = bucketSet.RemoteFolderPath;
                config.String = bucketSet.String;
            }
            else if (option is Options.PutBucketOptions bucketPut)
            {
                config.LocalPathFolders = bucketPut.LocalFoldersPath.ToList();
                config.LocalPathFiles = bucketPut.LocalFilesPath.ToList();
                config.RemoteRelativePath = bucketPut.RemoteFolderPath;
                config.StringPath = bucketPut.RemoteFolderPath;
                config.String = bucketPut.String;
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
            config = SetDefaultRunConfigurationOption(config, type, command, option);
            return config;
        }

        private DefaultRunConfiguration SetDefaultRunConfigurationOption(DefaultRunConfiguration config, ConfigType type, CommandApi command, Options.IGetOptions option)
        {
            GetDefaultOptions(config, option);

            ConfigGetGlobalOptions(option);
            ConfigPrintInformation(option, type, Enum.GetName(typeof(CommandApi), command));
            config.Name = option.Name;
            config.Id = option.Id;
            config.Summaries = false;

            if (option.TagsIntersect != null && option.TagsIntersect.Count() > 0)
            {
                config.Tags = option.TagsIntersect?.ToList();
                config.TagsIntersect = true;
            }
            else
            {
                config.Tags = option.Tags?.ToList();
            }

            return config;
        }

        /// <summary>
        /// upgrade of the ConvertGenericGetterOption
        /// to have the stdout and error options.
        /// </summary>
        /// <param name="type">object type.</param>
        /// <param name="command">object command to launch.</param>
        /// <param name="option">object options and values.</param>
        /// <returns>a new object command to be launch.</returns>
        public StdConfiguration ConvertStdOption(ConfigType type, CommandApi command, Options.AStdOptions option)
        {
            StdConfiguration config = new StdConfiguration(type, command);

            SetDefaultRunConfigurationOption(config, type, command, option);
            config.Stdout = option.Stdout;
            config.Stderr = option.Stderr;
            config.Fresh = option.Fresh;
            config.InstanceId = option.InstanceId;

            return config;
        }

        private ISnapshotConfiguration HydrateSnapOption(ISnapshotConfiguration config, Options.ISnapshotOptions option)
        {
            config.SnapshotPeriodicSec = option.SnapshotPeriodicSec;
            config.Whitelist = option.Whitelist;
            config.Blacklist = option.Blacklist;

            return config;
        }

        /// <summary>
        /// upgrade of the ConvertGenericGetterOption
        /// to have the snap options
        /// </summary>
        /// <param name="type">object type</param>
        /// <param name="command">object command to launch</param>
        /// <param name="option">object options and values</param>
        /// <returns>a new object command to be launch</returns>
        public SnapshotConfiguration ConvertSnapshotOption(ConfigType type, CommandApi command, Options.SnapshotTaskOptions option)
        {
            SnapshotConfiguration config = new SnapshotConfiguration(type, command);

            SetDefaultRunConfigurationOption(config, type, command, option);
            HydrateSnapOption(config, option);
            return config;
        }

        public IPrivilegesConfiguration ConvertPrivilegesOption(IPrivilegesConfiguration config, Options.IPrivilegesOptions option)
        {
            config.ExportApiAndStorageCredentialsInEnvironment = option.ExportApiAndStorageCredentialsInEnvironment ?? config.ExportApiAndStorageCredentialsInEnvironment;
            return config;
        }

        public QarnotSDK.Scaling ConvertScalingOption(string scalingOption)
        {
            if (string.IsNullOrWhiteSpace(scalingOption))
            {
                return null;
            }
            var serializer = new JsonDeserializer(new TimePeriodSpecificationJsonConverter(), new ScalingPolicyConverter());
            if (scalingOption.Trim().StartsWith("@"))
            {
                return serializer.GetObjectFromFile<QarnotSDK.Scaling>(scalingOption.Trim().TrimStart('@'));
            }
            return serializer.Deserialize<QarnotSDK.Scaling>(scalingOption);
        }

        /// <summary>
        /// upgrade of the ConvertGenericGetterOption
        /// to have the options for constant update.
        /// </summary>
        /// <param name="type">object type.</param>
        /// <param name="command">object command to launch.</param>
        /// <param name="option">constant update options and values.</param>
        /// <returns>a new object command to be launch.</returns>
        public ConstantUpdateConfiguration ConvertConstantOption(ConfigType type, CommandApi command, Options.IConstantOptions option)
        {
            var config = new ConstantUpdateConfiguration(type, command);

            SetDefaultRunConfigurationOption(config, type, command, option);
            config.ConstantName = option.ConstantName;
            config.ConstantValue = option.ConstantValue;

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
            config.ApiConnection.AccountEmail = option.AccountEmail;
            config.ApiConnection.SetForcePathStyleString(option.ForcePathStyle);
            config.ApiConnection.DisableBucketPathsSanitization = option.NoSanitizeBucketPaths;
            config.ApiConnection.UnsafeSsl = option.UnsafeSslCertificate;
            config.ApiConnection.StorageUnsafeSsl = option.StorageUnsafeSsl;

            config.ShowConnectionInfo = option.ShowConnectionInfo;
            if (option.ShowConnectionInfo)
            {
                config.Command = CommandApi.Info;
            }

            return config;
        }

        public PoolSetElasticSettingsConfiguration ConvertElasticPoolSetterOption(ConfigType type, CommandApi command, Options.PoolElasticSettingsOptions option)
        {
            PoolSetElasticSettingsConfiguration config = new PoolSetElasticSettingsConfiguration(type, CommandApi.Set);
            SetDefaultRunConfigurationOption(config, type, command, option);

            config.ElasticMinimumTotalSlots = option.ElasticMinimumTotalSlots;
            config.ElasticMaximumTotalSlots = option.ElasticMaximumTotalSlots;
            config.ElasticMinimumIdlingSlots = option.ElasticMinimumIdlingSlots;
            config.ElasticResizePeriod = option.ElasticResizePeriod;
            config.ElasticResizeFactor = option.ElasticResizeFactor;
            config.ElasticMinimumIdlingTime = option.ElasticMinimumIdlingTime;
            return config;
        }


        public PoolSetScalingConfiguration ConvertScalingSetterOption(ConfigType type, CommandApi command, Options.SetPoolScalingOptions option)
        {
            PoolSetScalingConfiguration config = new PoolSetScalingConfiguration(type, CommandApi.SetScaling);
            SetDefaultRunConfigurationOption(config, type, command, option);
            if (option.Scaling != default)
            {
                config.Scaling = ConvertScalingOption(option.Scaling);
            }

            return config;
        }

        private TimeSpan? ConvertStringToTimeSpan(string stringToParse)
        {
            Regex regex_day = new Regex(@"^([0-9]+)((\.)([0-9]*)((\:)([0-9]*))?((\:)([0-9]*))?)?$", RegexOptions.Compiled);
            Regex regex_hour = new Regex(@"([0-9]*)(\:([0-9]*))(\:([0-9]*))?$", RegexOptions.Compiled);

            Match match_day = regex_day.Match(stringToParse);
            if (match_day.Success)
            {
                GroupCollection groups = match_day.Groups;
                string day = string.IsNullOrEmpty(groups[1].Value) ? "0" : groups[1].Value;
                string hour = string.IsNullOrEmpty(groups[4].Value) ? "0" : groups[4].Value;
                string minute = string.IsNullOrEmpty(groups[7].Value) ? "0" : groups[7].Value;
                string second = string.IsNullOrEmpty(groups[10].Value) ? "0" : groups[10].Value;
                return new TimeSpan(int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second));
            }

            Match match_hour = regex_hour.Match(stringToParse);
            if (match_hour.Success)
            {
                GroupCollection groups = match_hour.Groups;
                string hour = string.IsNullOrEmpty(groups[1].Value) ? "0" : groups[1].Value;
                string minute = string.IsNullOrEmpty(groups[3].Value) ? "0" : groups[3].Value;
                string second = string.IsNullOrEmpty(groups[5].Value) ? "0" : groups[5].Value;
                return new TimeSpan(0, int.Parse(hour), int.Parse(minute), int.Parse(second));
            }

            return null;
        }

        private TimeSpan? ConvertDateStringToTimeSpan(string stringToParse)
        {
            DateTime dateValue;

            if (DateTime.TryParseExact(stringToParse, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dateValue))
            {
                return TimeSpan.FromTicks(dateValue.Ticks - DateTime.Now.Ticks);
            }
            else if (DateTime.TryParseExact(stringToParse, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dateValue))
            {
                return TimeSpan.FromTicks(dateValue.Ticks - DateTime.Now.Ticks);
            }

            return null;
        }

        private TimeSpan ParseTimeSpanString(string stringToParse)
        {
            TimeSpan? maxWallTime = ConvertDateStringToTimeSpan(stringToParse);
            if (!maxWallTime.HasValue)
            {
                maxWallTime = ConvertStringToTimeSpan(stringToParse);
            }

            if (maxWallTime.HasValue)
            {
                return maxWallTime.Value;
            }

            throw new ParseException("Max wall time wrong format fond.");
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
            config.Constraints = option.Constraints?.ToList().Count > 0 ? option.Constraints?.ToList() : config.Constraints;
            config.Labels = (option.Labels?.Any() ?? false) ? option.Labels.ToList() : config.Labels;
            config.Resources = option.Resources?.ToList().Count > 0 ? option.Resources?.ToList() : config.Resources;
            config.InstanceCount = option.InstanceCount > 0 ? option.InstanceCount : config.InstanceCount;
            config.JobUuidOrShortname = option.Job ?? config.JobUuidOrShortname;
            config.PoolUuidOrShortname = option.Pool ?? config.PoolUuidOrShortname;
            config.IsElastic = option.IsElastic ? option.IsElastic : config.IsElastic;
            config.IsDependents = option.IsDependents ? option.IsDependents : config.IsDependents;
            config.Result = option.Result ?? config.Result;
            config.Dependents = option.Dependents?.ToList().Count > 0 ? option.Dependents?.ToList() : config.Dependents;
            Options.IElasticityOptions elasticOption = option as Options.IElasticityOptions;
            var MaximumWallTime = string.IsNullOrEmpty(option.MaximumWallTime) ? config.MaximumWallTime : (TimeSpan?)ParseTimeSpanString(option.MaximumWallTime);
            config.MaximumWallTime = MaximumWallTime == default(TimeSpan) ? null : MaximumWallTime;
            config.TasksDefaultWaitForPoolResourcesSynchronization = option.TasksDefaultWaitForPoolResourcesSynchronization ?? config.TasksDefaultWaitForPoolResourcesSynchronization;
            config.MaxTotalRetries = option.MaxTotalRetries ?? config.MaxTotalRetries;
            config.MaxRetriesPerInstance = option.MaxRetriesPerInstance ?? config.MaxRetriesPerInstance;
            config.WaitForPoolResourcesSynchronization = option.WaitForPoolResourcesSynchronization ?? config.WaitForPoolResourcesSynchronization;
            config.DefaultResourcesCacheTTLSec = option.DefaultResourcesCacheTTLSec ?? config.DefaultResourcesCacheTTLSec;
            if (elasticOption != null)
            {
                config.ElasticMinimumTotalSlots = elasticOption.ElasticMinimumTotalSlots;
                config.ElasticMaximumTotalSlots = elasticOption.ElasticMaximumTotalSlots;
                config.ElasticMinimumIdlingSlots = elasticOption.ElasticMinimumIdlingSlots;
                config.ElasticResizePeriod = elasticOption.ElasticResizePeriod;
                config.ElasticResizeFactor = elasticOption.ElasticResizeFactor;
                config.ElasticMinimumIdlingTime = elasticOption.ElasticMinimumIdlingTime;
            }

            Options.IPrivilegesOptions privilegesOption = option as Options.IPrivilegesOptions;
            if (privilegesOption != null)
            {
                ConvertPrivilegesOption(config, privilegesOption);
            }

            if (option.Scaling != default)
            {
                config.Scaling = ConvertScalingOption(option.Scaling);
            }

            return config;
        }

        public CreateConfiguration ConvertTaskCreationOption(ConfigType type, Options.CreateTaskOptions option)
        {
            var config = ConvertGenericCreationOption(type, option);
            HydrateSnapOption(config, option);
            return config;
        }
    }
}
