namespace QarnotCLI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    /// <content>
    /// Options Objects of the ParseCommandLine.
    /// </content>
    public partial class Options
    {
        [Verb("account", HelpText = "Account commands.")]
        public class AccountOptions : AOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Classic usage ", new[] { new UnParserSettings { PreferShortName = true, ShowHidden = true } }, new AccountOptions { });
                }
            }
        }
    }
}