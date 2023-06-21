namespace QarnotCLI
{
    using System.Collections.Generic;

    /// <summary>
    /// Options Objects of the ParseCommandLine.
    /// </summary>
    public partial class Options
    {
        public abstract class AOptions : IOptions
        {
            public virtual string Token { get; set; }

            public bool Verbose { get; set; }

            public bool Quiet { get; set; }

            public bool NoColor { get; set; }

            public string ResultFormat { get; set; }

            public bool HumanReadable { get; set; }

            public bool UnsafeSslCertificate { get; set; }

            public string CustomSslCertificate { get; set; }
        }

        public abstract class ACreateOptions : AOptions, ICreateOptions
        {
            public virtual string Shortname { get; set; }

            public virtual string Name { get; set; }

            public virtual string Range { get; set; }

            public virtual string Profile { get; set; }

            public virtual string FileConf { get; set; }

            public virtual IEnumerable<string> Tags { get; set; }

            public virtual IEnumerable<string> Constants { get; set; }

            public virtual IEnumerable<string> Constraints { get; set; }

            public virtual IEnumerable<string> Labels { get; set; }

            public virtual IEnumerable<string> Dependents { get; set; }

            public virtual IEnumerable<string> Resources { get; set; }

            public virtual string Result { get; set; }

            public virtual uint InstanceCount { get; set; }

            public virtual bool IsElastic { get; set; }

            public virtual string Scaling { get; set; }

            public virtual bool IsDependents { get; set; }

            public virtual string Job { get; set; }

            public virtual string Pool { get; set; }

            public virtual string MaximumWallTime { get; set; }

            public virtual bool? WaitForPoolResourcesSynchronization { get; set; }

            public virtual bool? TasksDefaultWaitForPoolResourcesSynchronization { get; set; }

            public virtual uint? MaxTotalRetries { get; set; }

            public virtual uint? MaxRetriesPerInstance { get; set; }

            public virtual uint? DefaultResourcesCacheTTLSec { get; set; }

            public virtual IEnumerable<string> SecretsAccessRightsByKey { get; set; }

            public virtual IEnumerable<string> SecretsAccessRightsByPrefix { get; set; }
        }

        public abstract class AGetOptions : AOptions, IGetOptions
        {
            public virtual string Name { get; set; }

            public virtual IEnumerable<string> Tags { get; set; }

            public virtual IEnumerable<string> TagsIntersect { get; set; }

            public virtual string Id { get; set; }

            public virtual string Summaries { get; set; }
        }

        public abstract class ABucketGetOptions : AOptions, IBucketGetOptions
        {
            public virtual string Name { get; set; }
        }

        public abstract class ASecretsGetOptions : AOptions, ISecretsGetOptions
        {
            public virtual string Key { get; set; }
        }

        public abstract class ASecretsCreateOptions : AOptions, ISecretsCreateOptions
        {
            public virtual string Key { get; set; }
            public virtual string Value { get; set; }
        }

        public abstract class ASecretsUpdateOptions : AOptions, ISecretsUpdateOptions
        {
            public virtual string Key { get; set; }
            public virtual string Value { get; set; }
        }

        public abstract class ASecretsDeleteOptions : AOptions, ISecretsDeleteOptions
        {
            public virtual string Key { get; set; }
        }

        public abstract class ASecretsListOptions : AOptions, ISecretsListOptions
        {
            public virtual string Prefix { get; set; }
            public virtual bool Recursive { get; set; }
        }
    }
}
