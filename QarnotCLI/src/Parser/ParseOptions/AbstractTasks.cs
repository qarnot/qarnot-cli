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
        public abstract class ATaskGetOptions : AGetOptions
        {
            [Option('a', "all", Group = "Select", HelpText = "All the Tasks.")]
            public virtual bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the task.")]
            public override string Name { get; set; }

            [Option('t', "tags", SetName = "tags", Group = "Select", Required = false, HelpText = "Tags of the task to retrieve. The task should have any the tags given")]
            public override IEnumerable<string> Tags { get; set; }

            [Option("exclusive-tags", SetName = "exclusive-tags", Group = "Select", Required = false, HelpText = "Tags of the task to retrieve. The task should have all the tags given")]
            public override IEnumerable<string> TagsIntersect { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the task you want.")]
            public override string Id { get; set; }
        }

        public abstract class AStdOptions : ATaskGetOptions
        {
            public virtual bool Fresh { get; set; } = false;
            public virtual bool Stdout { get; set; } = false;
            public virtual bool Stderr { get; set; } = false;
        }
    }
}
