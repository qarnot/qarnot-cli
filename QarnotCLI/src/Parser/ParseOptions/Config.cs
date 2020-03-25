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
        /// Set the config options to connect to the API.
        /// </summary>
        [Verb("config", HelpText = "Config the cli options.")]
        public class ConfigOptions : AOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage ", new[] { UnParserSettings.WithGroupSwitchesOnly() }, new ConfigOptions { Token = "___TOKEN___" });
                    yield return new Example("Classic usage with a personal api uri ", new[] { UnParserSettings.WithUseEqualTokenOnly() }, new ConfigOptions { Token = "___TOKEN___", Uri = "https://api.qarnot.com", Storage = "https://storage.qarnot.com" });
                }
            }

            [Option('g', "global", Required = false, HelpText = "Set the configuration in the global default file ($HOME/.Qarnot/) to use it outside the binary path.")]
            public bool Global { get; set; }

            [Option('w', "show", HelpText = "Display the connection information that will be used in the connection (check also the environment variables).")]
            public bool ShowConnectionInfo { get; set; }

            [Option('t', "token", HelpText = "The API Token to be use.")]
            public override string Token { get; set; }

            [Option('u', "api-uri",Required = false, HelpText = "The api uri to be use.")]
            public string Uri { get; set; }

            [Option('s', "storage-uri",Required = false, HelpText = "The bucket api uri to be use.")]
            public string Storage { get; set; }
        }
    }
}