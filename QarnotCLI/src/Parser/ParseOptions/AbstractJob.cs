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
        public class AJobGetOptions : AGetOptions
        {
            [Option('a', "all", Group = "Select", HelpText = "All the jobs.")]
            public bool All { get; set; }

            [Option('n', "name", Group = "Select", Required = false, HelpText = "Name of the job.")]
            public override string Name { get; set; }

            [Option('i', "id", Group = "Select", Required = false, HelpText = "Shortname or Uuid of the job you want.")]
            public override string Id { get; set; }
        }

    }
}
