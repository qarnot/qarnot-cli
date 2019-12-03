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

            [Option("pool", Required = false, HelpText = "Uuid of the pool attached to the job.")]
            public override string Pool { get; set; }

            [Option('n', "name", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('s', "shortname", Required = false, HelpText = "ShortName of the job.")]
            public override string Shortname { get; set; }

            [Option('f', "file", Required = false, HelpText = "File with a json configuration of the job. (example : echo '{\"IsDependents\":true, \"Shortname\": \"SN\",\"Name\": \"JobName\"}' > CreateJob.json)")]
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
        public class InfoJobOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the jobs.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }

        [Verb("job wait", HelpText = "Wait for the end of the job selected.")]
        public class WaitJobOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the jobs.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }

        [Verb("job abort", HelpText = "Terminate the job selected.")]
        public class AbortJobOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the jobs.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", HelpText = "Name of the jobs to abort.")]
            public override string Name { get; set; }

            [Option('i', "id", Group = "Select", HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }

        [Verb("job delete", HelpText = "Delete the job selected.")]
        public class DeleteJobOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the jobs.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", HelpText = "Name of the jobs to delete.")]
            public override string Name { get; set; }

            [Option('i', "id", Group = "Select", HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }
    }
}