using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace QarnotCLI;

public partial class Options
{
    [Verb("secrets get", HelpText = "Get the value of a secret")]
    public class GetSecretOptions : ASecretsGetOptions
    {
        [Usage(ApplicationAlias = "qarnot")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Get the value of a secret",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new GetSecretOptions { Key = "path/to/secret" }
                );
            }
        }
    }

    [Verb("secrets create", HelpText = "Create a new secret")]
    public class CreateSecretOptions : ASecretsCreateOptions
    {
        [Usage(ApplicationAlias = "qarnot")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Create a new secret",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new CreateSecretOptions { Key = "path/to/secret", Value = "value" }
                );
            }
        }
    }

    [Verb("secrets update", HelpText = "Update an existing secret")]
    public class UpdateSecretOptions : ASecretsUpdateOptions
    {
        [Usage(ApplicationAlias = "qarnot")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Update an existing secret",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new UpdateSecretOptions { Key = "path/to/secret", Value = "value" }
                );
            }
        }
    }

    [Verb("secrets delete", HelpText = "Delete an existing secret")]
    public class DeleteSecretOptions : ASecretsDeleteOptions
    {
        [Usage(ApplicationAlias = "qarnot")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Delete an existing secret",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new DeleteSecretOptions { Key = "path/to/secret" }
                );
            }
        }
    }

    [Verb("secrets list", HelpText = "List secret keys")]
    public class ListSecretsOptions : ASecretsListOptions
    {
        [Usage(ApplicationAlias = "qarnot")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "List all the root secret entries",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new ListSecretsOptions()
                );
                yield return new Example(
                    "List all the secret keys",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new ListSecretsOptions() { Recursive = true }
                );
                yield return new Example(
                    "List all the secret keys starting with a prefix",
                    UnParserSettings.WithGroupSwitchesOnly(),
                    new ListSecretsOptions() { Prefix = "pre/fix", Recursive = true }
                );
            }
        }
    }
}
