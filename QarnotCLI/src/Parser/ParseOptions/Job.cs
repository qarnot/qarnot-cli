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
        [Verb("job create", HelpText = "Create and launch a new job.")]
        public class CreateJobOptions : ACreateOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new CreateJobOptions { Name = "Job name" });
                    yield return new Example("File config usage", UnParserSettings.WithGroupSwitchesOnly(), new CreateJobOptions { FileConf = "FileName.json" });
                    yield return new Example("Logging errors (missing name)", new CreateJobOptions { });
                }
            }

            [Option("max-wall-time", Required = false, HelpText = "Wall time limit for the job execution. Once this time duration exceeded, the whole job will terminate. The wall time format can be a date in the 'yyyy/MM/dd HH:mm:ss', 'yyyy/MM/dd' date format or a TimeStamp format 'd', 'd.hh', 'd.hh:mm', 'd.hh:mm:ss', 'hh:mm', 'hh:mm:ss'")]
            public override string MaximumWallTime { get; set; }

            [Option("pool", Required = false, HelpText = "Uuid of the pool attached to the job.")]
            public override string Pool { get; set; }

            [Option('n', "name", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('s', "shortname", Required = false, HelpText = "ShortName of the job.")]
            public override string Shortname { get; set; }

            [Option('f', "file", Required = false, HelpText = "File with a json configuration of the job. (example : echo '{\"IsDependents\":true, \"Shortname\": \"SN\",\"Name\": \"JobName\" }' > CreateJob.json)")]
            public override string FileConf { get; set; }

            [Option('d', "is-dependant", Required = false, HelpText = "Job can have job dependant from other ones to run.")]
            public override bool IsDependents { get; set; }
        }

        [Verb("job list", HelpText = "list the running jobs.")]
        public class ListJobOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new ListJobOptions { Name = "Job name" });
                }
            }

            [Option('n', "name", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('i', "id", Required = false, HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }

        [Verb("job info", HelpText = "Detail info of the job selected.")]
        public class InfoJobOptions : AJobGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new InfoJobOptions { Id = "JobUuid" });
                }
            }
        }

        [Verb("job wait", HelpText = "Wait for the end of the job selected.")]
        public class WaitJobOptions : AJobGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new WaitJobOptions { Id = "JobUuid" });
                }
            }
        }

        [Verb("job abort", HelpText = "Terminate the job selected.")]
        public class AbortJobOptions : AJobGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new AbortJobOptions { Id = "JobUuid" });
                }
            }
       }

        [Verb("job delete", HelpText = "Delete the job selected.")]
        public class DeleteJobOptions : AJobGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new DeleteJobOptions { Id = "JobUuid" });
                }
            }
        }
    }
}