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
        [Verb("task create", HelpText = "Create and launch a new task.")]
        public class CreateTaskOptions : ACreateOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new CreateTaskOptions { Name = "Task name", Profile = "docker-batch", InstanceCount = 4, Constants = new string[] {"DOCKER_CMD=echo hello world"} });
                    yield return new Example("Usage with a set of constants", UnParserSettings.WithGroupSwitchesOnly(), new CreateTaskOptions { Name = "Task name", Profile = "docker-batch", InstanceCount = 4, Constants = new string[] {"DOCKER_CMD=echo hello world", "DOCKER_REPO=library/ubuntu", "DOCKER_TAG=latest" } });
                    yield return new Example("File config usage (see documentation)", UnParserSettings.WithGroupSwitchesOnly(), new CreateTaskOptions { FileConf = "FileName.json" });
                    yield return new Example("Logging errors (missing instance)", new CreateTaskOptions { Pool = "POOL-UUID", Name = "Task name", Constants = new string[] {"DOCKER_CMD=echo hello world"} });
                }
            }

            [Option("job", Required = false, HelpText = "Uuid of the job attached to the task.")]
            public override string Job { get; set; }

            [Option("pool", Required = false, HelpText = "Uuid of the pool attached to the task.")]
            public override string Pool { get; set; }

            [Option('n', "name", HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('s', "shortname", Required = false, HelpText = "ShortName of the task.")]
            public override string Shortname { get; set; }

            [Option('p', "profile", Required = false, HelpText = "Name of the profile used for the task.")]
            public override string Profile { get; set; }

            [Option("range", Required = false, HelpText = "Range of the task.")]
            public override string Range { get; set; }

            [Option('i', "instance", HelpText = "Instance count of the task.")]
            public override uint InstanceCount { get; set; }

            [Option('f', "file", Required = false, HelpText = "File with a json configuration of the task. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"TaskName\",\"Profile\": \"docker-batch\",\"Constants\": [ \"DOCKER_CMD=echo hello world\", ],\"InstanceCount\": 1}' > CreateTask.json)")]
            public override string FileConf { get; set; }

            [Option('t', "tags", Required = false, HelpText = "Tags of the task.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('c', "constants", Required = false, HelpText = "Constants of the task.")]
            public override IEnumerable<string> Constants { get; set; }

            [Option('r', "resources", Required = false, HelpText = "Name of the buckets of the task.")]
            public override IEnumerable<string> Resources { get; set; }

            [Option("result", Required = false, HelpText = "Name of the bucket result of the task.")]
            public override string Result { get; set; }

            [Option('d', "dependents", Required = false, HelpText = "List of Uuid the task need to wait before start running.(must be use with a job with \"is-dependent\" set)")]
            public override IEnumerable<string> Dependents { get; set; }
        }

        [Verb("task list", HelpText = "List the running tasks.")]
        public class ListTaskOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } }, new ListTaskOptions { Name = "Task name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('n', "name", Required = false, HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('t', "tags", Required = false, HelpText = "Tags of the task.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Required = false, HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        [Verb("task info", HelpText = "Detail info of the task selected.")]
        public class InfoTaskOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new InfoTaskOptions { Name = "Task name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the task.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        [Verb("task update-resources", HelpText = "Update resources for a running task.")]
        public class UpdateTaskResourcesOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new UpdateTaskResourcesOptions { Id = "TaskID" });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the task.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        [Verb("task wait", HelpText = "Wait for the end of the task selected.")]
        public class WaitTaskOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new WaitTaskOptions { Name = "Task name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the task.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        [Verb("task abort", HelpText = "Terminate the task selected.")]
        public class AbortTaskOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new AbortTaskOptions { Name = "Task name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", HelpText = "Name of the tasks to abort.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", HelpText = "Tags of the tasks to abort.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        [Verb("task delete", HelpText = "Delete the task selected.")]
        public class DeleteTaskOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new DeleteTaskOptions { Name = "Task name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", HelpText = "Name of the tasks to delete.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", HelpText = "Tags of the tasks to delete.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }
    }
}