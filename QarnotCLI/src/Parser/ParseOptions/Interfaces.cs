namespace QarnotCLI
{
    using System.Collections.Generic;
    using CommandLine;

    /// <content>
    /// Options Objects of the ParseCommandLine.
    /// </content>
    public partial class Options
    {
        public interface IVerboseOptions
        {
            [Option('v', "verbose", Required = false, HelpText = "Set the max verbose messages.")]
            bool Verbose { get; set; }

            [Option('q', "quiet", Required = false, HelpText = "Set no verbose messages.")]
            bool Quiet { get; set; }
        }

        public interface IResultOptions
        {
            [Option("format", Required = false, HelpText = "Change the result format. (format available: TABLE, JSON)(Default TABLE)")]
            string ResultFormat { get; set; }

            [Option('h', "human-readable", Required = false, HelpText = "Print sizes in human readable format (e.g., 1K 234M 2G).")]
            bool HumanReadable { get; set; }
        }

        public interface ILogOptions
        {
            [Option("no-color", Required = false, HelpText = "Remove the color on the console.")]
            bool NoColor { get; set; }
        }

        public interface IOptions : IVerboseOptions, ILogOptions, IResultOptions
        {
            [Option("token", Required = false, HelpText = "Connection API Token.")]
            string Token { get; set; }

            [Option("unsafe-ssl", Required = false, HelpText = "Don't check the ssl certificate.")]
            bool UnsafeSslCertificate { get; set; }

            [Option("api-ca-certificate", Required = false, HelpText = "Path of your custom Ssl certificate.")]
            string CustomSslCertificate { get; set; }
        }

        public interface ICreateOptions : IOptions
        {
            string Shortname { get; set; }

            string Name { get; set; }

            string Range { get; set; }

            string Profile { get; set; }

            string FileConf { get; set; }

            IEnumerable<string> Tags { get; set; }

            IEnumerable<string> Constants { get; set; }

            IEnumerable<string> Constraints { get; set; }

            IEnumerable<string> Labels { get; set; }

            IEnumerable<string> Dependents { get; set; }

            bool IsDependents { get; set; }

            IEnumerable<string> Resources { get; set; }

            string Result { get; set; }

            uint InstanceCount { get; set; }

            bool IsElastic { get; set; }

            string Scaling { get; set; }

            string Job { get; set; }

            string Pool { get; set; }

            string MaximumWallTime { get; set; }

            bool? WaitForPoolResourcesSynchronization { get; set; }

            bool? TasksDefaultWaitForPoolResourcesSynchronization { get; set; }

            uint? MaxTotalRetries { get; set; }

            uint? MaxRetriesPerInstance { get; set; }

            uint? DefaultResourcesCacheTTLSec { get; set; }

            IEnumerable<string> SecretsAccessRightsByKey { get; set; }

            IEnumerable<string> SecretsAccessRightsByPrefix { get; set; }
        }

        public interface IElasticityOptions
        {
            [Option("min-slot", Required = false, HelpText = "Minimum slot number for the pool in elastic mode.")]
            uint? ElasticMinimumTotalSlots { get; set; }

            [Option("max-slot", Required = false, HelpText = "Maximum slot number for the pool in elastic mode.")]
            uint? ElasticMaximumTotalSlots { get; set; }

            [Option("min-idling-slot", Required = false, HelpText = "Minimum idling slot number.")]
            uint? ElasticMinimumIdlingSlots { get; set; }

            [Option("resize-period", Required = false, HelpText = "Elastic Resize Period.")]
            uint? ElasticResizePeriod { get; set; }

            [Option("resize-factor", Required = false, HelpText = "Elastic Resize Factor.")]
            float? ElasticResizeFactor { get; set; }

            [Option("min-idling-time", Required = false, HelpText = "Minimum idling time.")]
            uint? ElasticMinimumIdlingTime { get; set; }


            /////// BC area
            // Keep these ones for BC as the CLI was originally launched with these badly named options
            [Option("min-ling-node", Required = false, HelpText = "[DEPRECATED], use --min-idling-node instead")]
            uint? _elasticMinimumIdlingNodes_legacyBadOptionName { get { return ElasticMinimumIdlingSlots; } set { ElasticMinimumIdlingSlots = value; } }

            [Option("min-ling-time", Required = false, HelpText = "[DEPRECATED] Use --min-idling-time instead")]
            uint? _elasticMinimumIdlingTime_legacyBadOptionName { get { return ElasticMinimumIdlingTime; } set { ElasticMinimumIdlingTime = value; } }

            [Option("resize-periode", Required = false, HelpText = "[DEPRECATED] use --resize-period instead")]
            uint? _elasticResizePeriod_legacyBadOptionName { get { return ElasticResizePeriod; } set { ElasticResizePeriod = value; } }

            [Option("min-node", Required = false, HelpText = "[DEPRECATED] Use --min-slot instead")]
            uint? _elasticMinimumTotalNodes { get => ElasticMinimumTotalSlots; set { ElasticMinimumTotalSlots = value; } }

            [Option("max-node", Required = false, HelpText = "[DEPRECATED] Use --max-slot instead")]
            uint? _elasticMaximumTotalNodes  { get => ElasticMaximumTotalSlots; set { ElasticMaximumTotalSlots = value; } }

            [Option("min-idling-node", Required = false, HelpText = "[DEPRECATED] Use --min-idling-slot instead")]
            uint? _elasticMinimumIdlingNodes { get => ElasticMinimumIdlingSlots; set { ElasticMinimumIdlingSlots = value; } }
            /////// BC area
        }

        public interface IPrivilegesOptions
        {
            [Option("export-credentials-to-env", Required = false, HelpText = "Activate the exportation of the api and storage credentials to the task environment. Default is false.")]
            bool? ExportApiAndStorageCredentialsInEnvironment { get; set; }
        }

        public interface IGetOptions : IOptions
        {
            string Name { get; set; }

            IEnumerable<string> Tags { get; set; }

            IEnumerable<string> TagsIntersect{ get; set; }

            string Id { get; set; }

            string Summaries { get; set; }
        }

        public interface ISnapshotOptions
        {
            [Option("periodic", Required = false, HelpText = "Periodic time, in seconds, to synchronize the task files to the output bucket.")]
            uint SnapshotPeriodicSec { get; set; }

            [Option("whitelist", Required = false, HelpText = "Whitelist of task files to be synchronized to the output bucket.")]
            string Whitelist { get; set; }

            [Option("blacklist", Required = false, HelpText = "Blacklist of task files to synchronize to the output bucket.")]
            string Blacklist { get; set; }
        }

        public interface IBucketGetOptions : IOptions
        {
            [Option('n', "name", Required = true, HelpText = "Name of the bucket to get.")]
            string Name { get; set; }
        }

        public interface IConstantOptions : IGetOptions
        {

            [Option("constant-name", Required = false, HelpText = "Name of the constant to update.")]
            string ConstantName { get; set; }

            [Option("constant-value", Required = false, HelpText = "New value for the constant to update.")]
            string ConstantValue { get; set; }
        }

        public interface IScalingOptions : IGetOptions
        {
            [Option("scaling", Required = false, HelpText = "Scaling policies of the pool. Use either direct json format or a file path prefixed by '@'")]
            string Scaling { get; set; }
        }

        public interface ISecretsOptions : IOptions
        {
        }

        public interface ISecretsGetOptions : ISecretsOptions
        {
            [Value(0, MetaName = "key", Required = true, HelpText = "Key of the secret to retrieve.")]
            string Key { get; set; }
        }

        public interface ISecretsCreateOptions : ISecretsOptions
        {
            [Value(0, MetaName = "key", Required = true, HelpText = "Key of the secret to create.")]
            string Key { get; set; }

            [Value(1, MetaName = "value", Required = true, HelpText = "Value of the secret to create.")]
            string Value { get; set; }
        }

        public interface ISecretsUpdateOptions : ISecretsOptions
        {
            [Value(0, MetaName = "key", Required = true, HelpText = "Key of the secret to update.")]
            string Key { get; set; }

            [Value(1, MetaName = "value", Required = true, HelpText = "Value of the secret to update.")]
            string Value { get; set; }
        }

        public interface ISecretsDeleteOptions : ISecretsOptions
        {
            [Value(0, MetaName = "key", Required = true, HelpText = "Key of the secret to delete.")]
            string Key { get; set; }
        }

        public interface ISecretsListOptions : ISecretsOptions
        {
            [Value(0, MetaName = "prefix", Default = "", Required = false, HelpText = "Prefix of the secrets to list.")]
            string Prefix { get; set; }

            [Option('r', "recursive", Required = false, HelpText = "Perform a recursive listing. When performing a non-recursive listing, only entries right below `prefix` will be returned: `prefix/a` but not `prefix/a/b`. Subsequent prefixes can be identified by their trailing `/`.")]
            bool Recursive { get; set; }
        }
    }
}
