namespace QarnotCLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    /// <content>
    /// Options Objects of the ParseCommandLine.
    /// </content>
    public partial class Options
    {
        /// <summary>
        /// Send a command to all the Pools, Tasks and Jobs.
        /// </summary>
        [Verb("all", HelpText = "List, abort or delete all the objects (Task, Job, Pool, Bucket) from your profile.")]
        public class AllObjectsOptions : AOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Delete all the tasks, jobs and pools  ", new[] { UnParserSettings.WithUseEqualTokenOnly() }, new AllObjectsOptions { Delete = true });
                    yield return new Example("Terminate all the tasks, jobs and pools  ", new[] { UnParserSettings.WithUseEqualTokenOnly() }, new AllObjectsOptions { Abort = true });
                    yield return new Example("List all the tasks, jobs and pools  ", new[] { UnParserSettings.WithUseEqualTokenOnly() }, new AllObjectsOptions { List = true });
                    yield return new Example("(Default) List all the tasks, jobs and pools ", new AllObjectsOptions { });
                    yield return new Example("Error: only one rule can be set at time", new AllObjectsOptions { Delete = true, Abort = true, List = true });
                }
            }

            [Option('d', "delete", SetName = "rule List", HelpText = "Delete all the tasks, pools, jobs and buckets.")]
            public bool Delete { get; set; }

            [Option('a', "abort", SetName = "rule Abort", HelpText = "Abort all the tasks and jobs.")]
            public bool Abort { get; set; }

            [Option('l', "list", SetName = "rule Delete", HelpText = "List all the tasks, pools, jobs and buckets.")]
            public bool List { get; set; }
        }
    }
}