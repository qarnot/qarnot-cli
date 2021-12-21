namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface of the configuration type with all the needed info.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets or sets Type of the object to run.
        /// </summary>
        ConfigType Type { get; set; }

        /// <summary>
        /// Gets or sets Command of the object to run.
        /// </summary>
        CommandApi Command { get; set; }

        /// <summary>
        /// Gets or sets Api Connection info.
        /// </summary>
        APIConnectionInformation ApiConnection { get; set; }

        /// <summary>
        /// Gets or sets Result format value.
        /// </summary>
        string ResultFormat { get; set; }

        /// <summary>
        /// Print sizes in human readable format.
        /// </summary>
        bool HumanReadable { get; set; }

        /// <summary>
        /// Don't check the ssl certificate validity of the API connection.
        /// </summary>
        bool UnsafeSslCertificate { get; set; }

        /// <summary>
        /// Give your own self-signed ssl certificate validity to the API connection.
        /// </summary>
        string CustomSslCertificate { get; set; }
    }

    /// <summary>
    /// The snap Configuration innterface
    /// </summary>
    public interface ISnapshotConfiguration
    {
        /// <summary>
        /// Add a snap period
        /// </summary>
        /// <value>Time in second of the snap period.</value>
        uint SnapshotPeriodicSec { get; set; }

        /// <summary>
        /// Add a snap Whitelist
        /// </summary>
        /// <value>The whitelist list of the folders and files to get from the result folder.</value>
        string Whitelist { get; set; }

        /// <summary>
        /// Add a snap Blacklist
        /// </summary>
        /// <value>The blacklist list of the folders and files to get from the result folder.</value>
        string Blacklist { get; set; }
    }

    /// <summary>
    /// Configuration for the set of the info files.
    /// </summary>
    public class LocalSetUpConfiguration : IConfiguration
    {
        public LocalSetUpConfiguration()
        {
            this.ApiConnection = new APIConnectionInformation();
        }

        public LocalSetUpConfiguration(ConfigType type, CommandApi command)
        {
            this.Type = type;
            this.Command = command;
            this.ApiConnection = new APIConnectionInformation();
        }

        public ConfigType Type { get; set; }

        public CommandApi Command { get; set; } = CommandApi.Set;

        public string Path { get; set; }

        public bool GlobalPath { get; set; }

        public APIConnectionInformation ApiConnection { get; set; }

        public string ResultFormat { get; set; }

        public bool HumanReadable { get; set; }

        public bool UnsafeSslCertificate { get; set; }

        public string CustomSslCertificate { get; set; }

        public bool ShowConnectionInfo { get; set; }
    }

    /// <summary>
    /// Configuration for the Accont info.
    /// </summary>
    public class AccountConfiguration : IConfiguration
    {
        public AccountConfiguration()
        {
            this.ApiConnection = new APIConnectionInformation();
        }

        public AccountConfiguration(ConfigType type, CommandApi command)
        {
            this.Type = type;
            this.Command = command;
            this.ApiConnection = new APIConnectionInformation();
        }

        public ConfigType Type { get; set; }

        public CommandApi Command { get; set; } = CommandApi.Info;

        public string Balance { get; set; }

        public string Core { get; set; }

        public string Quotas { get; set; }

        public string Storage { get; set; }

        public virtual bool GetBucket { get; set; }

        public APIConnectionInformation ApiConnection { get; set; }

        public string ResultFormat { get; set; }

        public bool HumanReadable { get; set; }

        public bool UnsafeSslCertificate { get; set; }

        public string CustomSslCertificate { get; set; }

    }

    /// <summary>
    /// Configuration for the QBucket info.
    /// </summary>
    public class BucketConfiguration : IConfiguration
    {
        public BucketConfiguration()
        {
            this.ApiConnection = new APIConnectionInformation();
        }

        public BucketConfiguration(ConfigType type, CommandApi command)
        {
            this.Type = type;
            this.Command = command;
            this.ApiConnection = new APIConnectionInformation();
        }

        public APIConnectionInformation ApiConnection { get; set; }

        public string ResultFormat { get; set; }

        public bool HumanReadable { get; set; }

        public bool UnsafeSslCertificate { get; set; }

        public string CustomSslCertificate { get; set; }

        public ConfigType Type { get; set; } = ConfigType.Bucket;

        public virtual CommandApi Command { get; set; }

        public virtual string Name { get; set; }

        public virtual string LocalPathGet { get; set; }

        public virtual List<string> LocalPathFiles { get; set; }

        public virtual List<string> LocalPathFolders { get; set; }

        public virtual string RemoteRelativePath { get; set; }

        public virtual List<string> RemoteRelativePaths { get; set; }

        public virtual List<string> RemoteRelativePathFiles { get; set; }

        public virtual List<string> RemoteRelativePathFolders { get; set; }

        public virtual bool DeleteFiles { get; set; }

        public virtual string ReturnString { get; set; }

        public virtual string String { get; set; }

        public virtual string StringPath { get; set; }
    }

    /// <summary>
    /// Configuration for the QObjects info.
    /// </summary>
    public class DefaultRunConfiguration : IConfiguration
    {
        public DefaultRunConfiguration()
        {
            this.ApiConnection = new APIConnectionInformation();
        }

        public DefaultRunConfiguration(ConfigType type, CommandApi command)
        {
            this.Type = type;
            this.Command = command;
            this.ApiConnection = new APIConnectionInformation();
        }

        public APIConnectionInformation ApiConnection { get; set; }

        public string ResultFormat { get; set; }

        public bool HumanReadable { get; set; }

        public bool UnsafeSslCertificate { get; set; }

        public string CustomSslCertificate { get; set; }

        public virtual ConfigType Type { get; set; }

        public virtual CommandApi Command { get; set; }

        public virtual string Name { get; set; }

        public virtual string Id { get; set; }

        public virtual bool Summaries { get; set; }

        public virtual List<string> Tags { get; set; }

        public virtual bool TagsIntersect { get; set; }
    }


    /// <summary>
    /// Configuration for the snapshot.
    /// </summary>
    public class SnapshotConfiguration : DefaultRunConfiguration, ISnapshotConfiguration
    {
        public SnapshotConfiguration(ConfigType type, CommandApi command):base(type, command)
        {
        }

        public uint SnapshotPeriodicSec { get; set; }

        public string Whitelist { get; set; }

        public string Blacklist { get; set; }
    }

    /// <summary>
    /// Configuration for the QObjects info.
    /// </summary>
    public class StdConfiguration : DefaultRunConfiguration
    {
        public StdConfiguration(ConfigType type, CommandApi command): base(type, command)
        {
        }

        public virtual bool Stdout { get; set; }
        public virtual bool Stderr { get; set; }
        public virtual bool Fresh { get; set; }

    }

    /// <summary>
    /// Configuration for the QPool Elastic info.
    /// </summary>
    public class PoolSetConfiguration : DefaultRunConfiguration
    {
        public PoolSetConfiguration(ConfigType type, CommandApi command)
            : base(type, command)
        {
        }

        public uint ElasticMinimumTotalNodes { get; set; }

        public uint ElasticMaximumTotalNodes { get; set; }

        public uint ElasticMinimumIdlingNodes { get; set; }

        public uint ElasticResizePeriod { get; set; }

        public float ElasticResizeFactor { get; set; }

        public uint ElasticMinimumIdlingTime { get; set; }
    }

    /// <summary>
    /// Configuration for the QObject create info.
    /// </summary>
    public class CreateConfiguration : IConfiguration, ISnapshotConfiguration
    {
        public CreateConfiguration()
        {
            this.ApiConnection = new APIConnectionInformation();
            this.Resources = new List<string>();
            this.Constants = new List<string>();
            this.Constraints = new List<string>();
            this.Dependents = new List<string>();
            this.Tags = new List<string>();
            this.Labels = new List<string>();
        }

        public CreateConfiguration(ConfigType type, CommandApi command)
            : this()
        {
            this.Type = type;
            this.Command = command;
        }

        public APIConnectionInformation ApiConnection { get; set; }

        public string ResultFormat { get; set; }

        public bool HumanReadable { get; set; }

        public bool UnsafeSslCertificate { get; set; }

        public string CustomSslCertificate { get; set; }

        public ConfigType Type { get; set; }

        public CommandApi Command { get; set; } = CommandApi.Create;

        public string Shortname { get; set; }

        public string Name { get; set; }

        public string Range { get; set; }

        public string Profile { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Constants { get; set; }

        public List<string> Constraints { get; set; }

        public List<string> Resources { get; set; }

        public string Result { get; set; }

        public List<string> Dependents { get; set; }

        public uint SnapshotPeriodicSec { get; set; }

        public string Whitelist { get; set; }

        public string Blacklist { get; set; }

        public bool IsDependents { get; set; }

        public uint InstanceCount { get; set; }

        public bool IsElastic { get; set; }

        public string JobUuid { get; set; }

        public string PoolUuid { get; set; }

        public uint ElasticMinimumTotalNodes { get; set; }

        public uint ElasticMaximumTotalNodes { get; set; }

        public uint ElasticMinimumIdlingNodes { get; set; }

        public uint ElasticResizePeriod { get; set; }

        public float ElasticResizeFactor { get; set; }

        public uint ElasticMinimumIdlingTime { get; set; }

        public TimeSpan? MaximumWallTime { get; set; }

        public bool TasksDefaultWaitForPoolResourcesSynchronization { get; set; } = false;

        public bool? WaitForPoolResourcesSynchronization { get; set; }

        public List<string> Labels { get; set; }
    }
}
