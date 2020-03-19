namespace QarnotCLI
{
    using System.Linq;

    public interface IVerbFormater
    {
        string[] ConcatSubverbArgv(string[] argv);
    }

    /// <summary>
    /// Concatenat the 2 first arguments of argv
    /// improve the usage printed readability
    /// create because CommandLine do not integrate domino subverbs.
    /// </summary>
    public class VerbFormater : IVerbFormater
    {
        private bool VerifySubcommandPrefix(string command, string subCommand)
        {
            string[] whiteListCommand =
            {
                "task",
                "pool",
                "job",
                "bucket",
            };

            string[] whiteListSubCommand =
            {
                "create",
                "info",
                "wait",
                "abort",
                "list",
                "delete",
                "set",
                "get",
                "sync-from",
                "sync-to",
                "update-resources",
            };

            if (whiteListCommand.Contains(command) && whiteListSubCommand.Contains(subCommand))
            {
                return true;
            }

            return false;
        }

        private string AddSubcommandPrefix(string prefix, string command)
        {
            if (VerifySubcommandPrefix(prefix, command))
            {
                command = prefix + " " + command;
            }

            return command;
        }

        public string[] ConcatSubverbArgv(string[] argv)
        {
            if (argv.Length > 1)
            {
                var newArgv = argv.ToList().Skip(1).ToArray();
                newArgv[0] = AddSubcommandPrefix(argv[0], argv[1]);
                return newArgv;
            }

            return argv;
        }
    }
}
