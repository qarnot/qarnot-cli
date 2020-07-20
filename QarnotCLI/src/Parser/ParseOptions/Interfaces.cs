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
            [Option("format", Required = false, HelpText = "Change the result format. (format available: TABLE, JSON, XML)(Default TABLE)")]
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

            IEnumerable<string> Dependents { get; set; }

            bool IsDependents { get; set; }

            IEnumerable<string> Resources { get; set; }

            string Result { get; set; }

            uint InstanceCount { get; set; }

            bool IsElastic { get; set; }

            string Job { get; set; }

            string Pool { get; set; }

            string MaximumWallTime { get; set; }
        }

        public interface IElasticityOptions
        {
            [Option("min-node", Required = false, HelpText = "Minimum node number for the pool in elastic mode.")]
            uint ElasticMinimumTotalNodes { get; set; }

            [Option("max-node", Required = false, HelpText = "Maximum node number for the pool in elastic mode.")]
            uint ElasticMaximumTotalNodes { get; set; }

            [Option("min-ling-node", Required = false, HelpText = "Minimum idling node number.")]
            uint ElasticMinimumIdlingNodes { get; set; }

            [Option("resize-periode", Required = false, HelpText = "Elastic Resize Period.")]
            uint ElasticResizePeriod { get; set; }

            [Option("resize-factor", Required = false, HelpText = "Elastic Resize Factor.")]
            float ElasticResizeFactor { get; set; }

            [Option("min-ling-time", Required = false, HelpText = "Minimum idling time.")]
            uint ElasticMinimumIdlingTime { get; set; }
        }

        public interface IGetOptions : IOptions
        {
            string Name { get; set; }

            IEnumerable<string> Tags { get; set; }

            string Id { get; set; }

            string Summaries { get; set; }
        }

        public interface IBucketGetOptions : IOptions
        {
            [Option('n', "name", Required = true, HelpText = "Name of the bucket to get.")]
            string Name { get; set; }
        }
    }
}