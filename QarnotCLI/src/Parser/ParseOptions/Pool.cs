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
        [Verb("pool create", HelpText = "Create and launch a new pool.")]
        public class CreatePoolOptions : ACreateOptions, IElasticityOptions, IPrivilegesOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new CreatePoolOptions { Name = "Pool name", Profile = "docker-batch", InstanceCount = 4 });
                    yield return new Example("File config usage", UnParserSettings.WithGroupSwitchesOnly(), new CreatePoolOptions { FileConf = "FileName.json" });
                    yield return new Example("Logging errors (missing instanceNodes)", new CreatePoolOptions { Name = "Pool name", Profile = "docker-batch" });
                }
            }

            public override string Range { get; set; }

            [Option('n', "name", HelpText = "(Required) Name of the pool.")]
            public override string Name { get; set; }

            [Option('s', "shortname", Required = false, HelpText = "ShortName of the pool.")]
            public override string Shortname { get; set; }

            [Option('p', "profile", HelpText = "(Required) Name of the profile used for the pool.")]
            public override string Profile { get; set; }

            [Option('i', "instanceNodes", HelpText = "(Required if not elastic) instance count of the pool.")]
            public override uint InstanceCount { get; set; }

            [Option('e', "pool-is-elastic", HelpText = "The pool create is elastic.")]
            public override bool IsElastic { get; set; }

            [Option('f', "file", Required = false, HelpText = "File with a json configuration of the pool. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"PoolName\",\"Profile\": \"docker-batch\",\"InstanceCount\": 1}' > CreatePool.json)")]
            public override string FileConf { get; set; }

            [Option('t', "tags", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('c', "constants", Required = false, HelpText = "Constants of the pool.")]
            public override IEnumerable<string> Constants { get; set; }

            [Option("constraints", Required = false, HelpText = "Constraints of the pool.")]
            public override IEnumerable<string> Constraints { get; set; }

            [Option("labels", Required = false, HelpText = "Labels of the pool.")]
            public override IEnumerable<string> Labels { get; set; }

            [Option('r', "resources", Required = false, HelpText = "Name of the buckets of the pool.")]
            public override IEnumerable<string> Resources { get; set; }

            public virtual uint ElasticMinimumTotalSlots { get; set; }

            public virtual uint ElasticMaximumTotalSlots { get; set; }

            public virtual uint ElasticMinimumIdlingSlots { get; set; }

            public virtual uint ElasticResizePeriod { get; set; }

            public virtual float ElasticResizeFactor { get; set; }

            public virtual uint ElasticMinimumIdlingTime { get; set; }

            [Option("tasks-wait-for-synchronization", Required = false, HelpText = "Have all the pool's tasks wait for the resources to be synchronized before running if the pool resources are updated before the task submission. (set to true or false, default: false)")]
            public override bool? TasksDefaultWaitForPoolResourcesSynchronization { get; set; }
            public virtual bool? ExportApiAndStorageCredentialsInEnvironment { get; set; }

            [Option("ttl", Required = false, HelpText = "Default TTL for the pool resources cache (in seconds). Default is 7776000s.")]
            public override uint? DefaultResourcesCacheTTLSec { get; set; }
        }

        [Verb("pool list", HelpText = "List the running pools.")]
        public class ListPoolOptions : AGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new ListPoolOptions { Name = "Pool name", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('n', "name", Required = false, HelpText = "Name of the pool.")]
            public override string Name { get; set; }

            [Option('t', "tags", SetName = "tags", Required = false, HelpText = "Tags of the pool to retrieve.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option("exclusive-tags", SetName = "exclusive-tags", Required = false, HelpText = "Tags of the pool to retrieve. The pool should have all the tags given")]
            public override IEnumerable<string> TagsIntersect { get; set; }

            [Option('i', "id", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

        [Verb("pool info", HelpText = "Detail info of the pool selected.")]
        public class InfoPoolOptions : APoolGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new InfoPoolOptions { Id = "Pool Uuid", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }
        }

        [Verb("pool update-resources", HelpText = "Update resources for a running pool")]
        public class UpdatePoolResourcesOptions : APoolGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new UpdatePoolResourcesOptions { Id = "Pool Uuid", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }
        }

        [Verb("pool update-constant", HelpText = "Update a constant of a running pool.")]
        public class UpdatePoolConstantOptions : APoolGetOptions, IConstantOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new UpdatePoolConstantOptions { Id = "Pool Uuid", ConstantName = "QARNOT_SECRET__SUPER_TOKEN", ConstantValue = "new-token" });
                }
            }

            public string ConstantName { get; set; }

            public string ConstantValue { get; set; }
        }

        [Verb("pool delete", HelpText = "Delete the pool selected.")]
        public class DeletePoolOptions : APoolGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new DeletePoolOptions { Id = "Pool Uuid", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }
        }

        // TODO: Deprecate this option and remove it as its name is not explicit enough (or group it with SetPoolElasticSettingsOptions using aliases)
        [Verb("pool set", HelpText = "[DEPRECATED] Set the pool elastic options.")]
        public class SetPoolOptions : PoolElasticSettingsOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new SetPoolOptions { Id = "Pool Uuid", ElasticMinimumTotalSlots = 2, ElasticMaximumTotalSlots = 10});
                }
            }
        }

        [Verb("pool set-elastic-settings", HelpText = "Set the pool elastic options.")]
        public class SetPoolElasticSettingsOptions : PoolElasticSettingsOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new SetPoolElasticSettingsOptions { Id = "Pool Uuid", ElasticMinimumTotalSlots = 2, ElasticMaximumTotalSlots = 10});
                }
            }
        }

        public class PoolElasticSettingsOptions : APoolGetOptions, IElasticityOptions
        {
            public virtual uint ElasticMinimumTotalSlots { get; set; }

            public virtual uint ElasticMaximumTotalSlots { get; set; }

            public virtual uint ElasticMinimumIdlingSlots { get; set; }

            public virtual uint ElasticResizePeriod { get; set; }

            public virtual float ElasticResizeFactor { get; set; }

            public virtual uint ElasticMinimumIdlingTime { get; set; }
        }
    }
}
