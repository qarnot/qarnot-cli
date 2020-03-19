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
        public class CreatePoolOptions : ACreateOptions, IElasticityOptions
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

            [Option('i', "instanceNodes", HelpText = "(Required) instance count of the pool.")]
            public override uint InstanceCount { get; set; }

            [Option('e', "pool-is-elastic", HelpText = "The pool create is elastic.")]
            public override bool IsElastic { get; set; }

            [Option('f', "file", Required = false, HelpText = "File with a json configuration of the pool. (example : echo '{\"Shortname\": \"SN\",\"Name\": \"PoolName\",\"Profile\": \"docker-batch\",\"InstanceCount\": 1}' > CreatePool.json)")]
            public override string FileConf { get; set; }

            [Option('t', "tags", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('c', "constants", Required = false, HelpText = "Constants of the pool.")]
            public override IEnumerable<string> Constants { get; set; }

            [Option('r', "resources", Required = false, HelpText = "Name of the buckets of the pool.")]
            public override IEnumerable<string> Resources { get; set; }

            public virtual uint ElasticMinimumTotalNodes { get; set; }

            public virtual uint ElasticMaximumTotalNodes { get; set; }

            public virtual uint ElasticMinimumIdlingNodes { get; set; }

            public virtual uint ElasticResizePeriod { get; set; }

            public virtual float ElasticResizeFactor { get; set; }

            public virtual uint ElasticMinimumIdlingTime { get; set; }
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

            [Option('t', "tags", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

        [Verb("pool info", HelpText = "Detail info of the pool selected.")]
        public class InfoPoolOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the pools.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the pool.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

        [Verb("pool update-resources", HelpText = "Update resources for a running pool")]
        public class UpdatePoolResourcesOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the pools.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the pool.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

        [Verb("pool delete", HelpText = "Delete the pool selected.")]
        public class DeletePoolOptions : AGetOptions
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

            [Option('a', "all", Group = "Select", HelpText = "All the pool.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", HelpText = "Name of the pools to delete.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", HelpText = "Tags of the pools to delete.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

        [Verb("pool set", HelpText = "Set the pool elastic options.")]
        public class SetPoolOptions : AGetOptions, IElasticityOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example(
                        "Classic usage",
                        new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly(), new UnParserSettings() { PreferShortName = true } },
                        new SetPoolOptions { Id = "Pool Uuid", Tags = new string[] { "TAG1", "TAG2" } });
                }
            }

            [Option('a', "all", Group = "Select", HelpText = "All the pool.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the pool.")]
            public override string Name { get; set; }

            [Option('t', "tags", Group = "Select", Required = false, HelpText = "Tags of the pool.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }

            public virtual uint ElasticMinimumTotalNodes { get; set; }

            public virtual uint ElasticMaximumTotalNodes { get; set; }

            public virtual uint ElasticMinimumIdlingNodes { get; set; }

            public virtual uint ElasticResizePeriod { get; set; }

            public virtual float ElasticResizeFactor { get; set; }

            public virtual uint ElasticMinimumIdlingTime { get; set; }
        }
    }
}
