namespace QarnotCLI
{
    using CommandLine;

    /// <content>
    /// Options Objects of the ParseCommandLine.
    /// </content>
    public partial class Options
    {
        /// <summary>
        /// Default command to be print in the usage.
        /// </summary>
        [Verb("pool", HelpText = "Pool commands.")]
        public class DefaultPool : AOptions
        {
        }

        /// <summary>
        /// Default command to be print in the usage.
        /// </summary>
        [Verb("task", HelpText = "Task commands.")]
        public class DefaultTask : AOptions
        {
        }

        /// <summary>
        /// Default command to be print in the usage.
        /// </summary>
        [Verb("bucket", HelpText = "Bucket commands.")]
        public class DefaultBucket : AOptions
        {
        }

        /// <summary>
        /// Default command to be print in the usage.
        /// </summary>
        [Verb("job", HelpText = "Job commands.")]
        public class DefaultJob : AOptions
        {
        }
    }
}