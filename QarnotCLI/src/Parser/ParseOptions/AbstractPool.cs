namespace QarnotCLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Options Objects of the ParseCommandLine.
    /// </summary>
    public partial class Options
    {
        public class APoolGetOptions : AGetOptions
        {
            [Option('a', "all", Group = "Select", HelpText = "All the pools.")]
            public virtual bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the pool.")]
            public override string Name { get; set; }

            [Option('t', "tags", SetName = "tags", Group = "Select", Required = false, HelpText = "Tags of the pool to retrieve.")]
            public override IEnumerable<string> Tags { get; set; }

            [Option("exclusive-tags", Group = "Select", SetName = "exclusive-tags", Required = false, HelpText = "Tags of the pool to retrieve. The pool should have all the tags given")]
            public override IEnumerable<string> TagsIntersect { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the pool you want.")]
            public override string Id { get; set; }
        }

    }
}
