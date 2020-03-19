namespace QarnotCLI
{
    using CommandLine;

    public interface IArgumentsParser
    {
        IConfiguration Parse(string[] argv);
    }

    /// <summary>
    /// The parser of the CLI.
    /// </summary>
    public class CommandLineParser : IArgumentsParser
    {
        // TODO: build a interface class for the command line,
        // TODO: build a interface for the Config connection.
        private readonly CommandLine.Parser Parser;

        private readonly ParserUsage Usage;

        private readonly IOptionToConfigConverter CreateConfig;

        private readonly IVerbFormater SubVerbFormater;

        public CommandLineParser(IOptionToConfigConverter createConfig, CommandLine.Parser parser, ParserUsage usage, IVerbFormater verbFormater)
        {
            this.Parser = parser;
            this.CreateConfig = createConfig;
            this.Usage = usage;
            this.SubVerbFormater = verbFormater;
        }

        public IConfiguration Parse(string[] argv)
        {
            string firstWord = (argv.Length > 0) ? argv[0] : string.Empty;

            IConfiguration result = this.ParseSubCommand(firstWord, argv);

            CLILogs.CreateLoggers();
            return result;
        }

        /// <summary>
        /// The check the verbs if it's a verb with subverb send it to the appropriate method.
        /// </summary>
        /// <param name="firstWord">First argv elem : Verb word.</param>
        /// <param name="argv">list of the rest of argv values.</param>
        /// <returns>Configuration object.</returns>
        private IConfiguration ParseSubCommand(string firstWord, string[] argv)
        {
            switch (firstWord)
            {
                case "pool":
                    return this.ParsePool(argv);
                case "task":
                    return this.ParseTask(argv);
                case "job":
                    return this.ParseJob(argv);
                case "bucket":
                    return this.ParseBucket(argv);
                default:
                    return this.ParseGeneral(argv);
            }
        }

        /// <summary>
        /// The General parser verb, it's used if there are not sous verbs.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        /// <returns>Configuration Object used to run the command.</returns>
        private IConfiguration ParseGeneral(string[] argv)
        {
            IConfiguration result = null;

            var parser = this.Parser.ParseArguments<
                                            Options.DefaultTask,
                                            Options.DefaultPool,
                                            Options.DefaultJob,
                                            Options.DefaultBucket,
                                            Options.AllObjectsOptions,
                                            Options.ConfigOptions,
                                            Options.AccountOptions
                                            >(argv);

            parser.MapResult(
                (Options.AllObjectsOptions o) => result = this.CreateConfig.ConvertAllGetterOption(ConfigType.All, o),
                (Options.ConfigOptions o) => result = this.CreateConfig.ConvertGenericSetterOption(ConfigType.Config, o),
                (Options.AccountOptions o) => result = this.CreateConfig.ConvertAccountOption(ConfigType.Account, o),
                err => throw new ParseException(this.Usage.PrintHelp(parser, err)));

            return result;
        }

        public static bool CheckTask(IConfiguration config)
        {
            if (config is CreateConfiguration && ((CreateConfiguration)config).Type == ConfigType.Task)
            {
                CreateConfiguration createConfig = (CreateConfiguration)config;
                if (string.IsNullOrEmpty(createConfig.Name) ||
                    (string.IsNullOrEmpty(createConfig.Profile) && string.IsNullOrEmpty(createConfig.PoolUuid) && string.IsNullOrEmpty(createConfig.JobUuid)) ||
                    (createConfig.InstanceCount == 0 && string.IsNullOrEmpty(createConfig.Range)) ||
                    (createConfig.InstanceCount != 0 && !string.IsNullOrEmpty(createConfig.Range)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the task subCommand.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        /// <returns>Configuration Object used to run the command.</returns>
        private IConfiguration ParseTask(string[] argv)
        {
            IConfiguration result = null;
            argv = this.SubVerbFormater.ConcatSubverbArgv(argv);

            // TODO: find a smart way to inject the Types to do a generic mapper ?
            var parser = this.Parser.ParseArguments<
                                                    Options.CreateTaskOptions,
                                                    Options.ListTaskOptions,
                                                    Options.InfoTaskOptions,
                                                    Options.WaitTaskOptions,
                                                    Options.AbortTaskOptions,
                                                    Options.DeleteTaskOptions,
                                                    Options.UpdateTaskResourcesOptions
                                                    >(argv);

            parser.MapResult(
                (Options.CreateTaskOptions o) => result = this.CreateConfig.ConvertGenericCreationOption(ConfigType.Task, o),
                (Options.ListTaskOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.List, o),
                (Options.InfoTaskOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.Info, o),
                (Options.WaitTaskOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.Wait, o),
                (Options.AbortTaskOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.Abort, o),
                (Options.DeleteTaskOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.Delete, o),
                (Options.UpdateTaskResourcesOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Task, CommandApi.UpdateResources, o),
                err => throw new ParseException(this.Usage.PrintHelp(parser, err)));

            if (!CheckTask(result))
            {
                this.Usage.PrintHelp(parser, null);
            }

            return result;
        }

        public static bool CheckPool(IConfiguration config)
        {
            if (config is CreateConfiguration && ((CreateConfiguration)config).Type == ConfigType.Pool)
            {
                CreateConfiguration createConfig = (CreateConfiguration)config;
                if (string.IsNullOrEmpty(createConfig.Name) ||
                    string.IsNullOrEmpty(createConfig.Profile) ||
                    createConfig.InstanceCount == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the Pools subCommand.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        /// <returns>Configuration Object used to run the command.</returns>
        private IConfiguration ParsePool(string[] argv)
        {
            IConfiguration result = null;

            argv = this.SubVerbFormater.ConcatSubverbArgv(argv);

            var parser = this.Parser.ParseArguments<Options.CreatePoolOptions,
                                                    Options.ListPoolOptions,
                                                    Options.InfoPoolOptions,
                                                    Options.SetPoolOptions,
                                                    Options.DeletePoolOptions,
                                                    Options.UpdatePoolResourcesOptions>(argv);

            parser.MapResult(
                (Options.CreatePoolOptions o) => result = this.CreateConfig.ConvertGenericCreationOption(ConfigType.Pool, o),
                (Options.ListPoolOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Pool, CommandApi.List, o),
                (Options.InfoPoolOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Pool, CommandApi.Info, o),
                (Options.DeletePoolOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Pool, CommandApi.Delete, o),
                (Options.SetPoolOptions o) => result = this.CreateConfig.ConvertElasticPoolSetterOption(ConfigType.Pool, CommandApi.Set, o),
                (Options.UpdatePoolResourcesOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Pool, CommandApi.UpdateResources, o),
                err => throw new ParseException(this.Usage.PrintHelp(parser, err)));

            if (!CheckPool(result))
            {
                this.Usage.PrintHelp(parser, null);
            }

            return result;
        }

        public static bool CheckJob(IConfiguration config)
        {
            if (config is CreateConfiguration && ((CreateConfiguration)config).Type == ConfigType.Job)
            {
                CreateConfiguration createConfig = (CreateConfiguration)config;
                if (string.IsNullOrWhiteSpace(createConfig.Name))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the Jobs subCommand.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        /// <returns>Configuration Object used to run the command.</returns>
        private IConfiguration ParseJob(string[] argv)
        {
            IConfiguration result = null;

            argv = this.SubVerbFormater.ConcatSubverbArgv(argv);

            var parser = this.Parser.ParseArguments<Options.CreateJobOptions,
                                                    Options.ListJobOptions,
                                                    Options.InfoJobOptions,
                                                    Options.WaitJobOptions,
                                                    Options.AbortJobOptions,
                                                    Options.DeleteJobOptions
                                                    >(argv);

            parser.MapResult(
                (Options.CreateJobOptions o) => result = this.CreateConfig.ConvertGenericCreationOption(ConfigType.Job, o),
                (Options.ListJobOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Job, CommandApi.List, o),
                (Options.InfoJobOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Job, CommandApi.Info, o),
                (Options.WaitJobOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Job, CommandApi.Wait, o),
                (Options.AbortJobOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Job, CommandApi.Abort, o),
                (Options.DeleteJobOptions o) => result = this.CreateConfig.ConvertGenericGetterOption(ConfigType.Job, CommandApi.Delete, o),
                err => throw new ParseException(this.Usage.PrintHelp(parser, err)));

            if (!CheckJob(result))
            {
                this.Usage.PrintHelp(parser, null);
            }

            return result;
        }

        /// <summary>
        /// Parse the Bucket subCommand.
        /// </summary>
        /// <param name="argv">Command-line arguments passed to the program.</param>
        /// <returns>Configuration Object used to run the command.</returns>
        private IConfiguration ParseBucket(string[] argv)
        {
            IConfiguration result = null;

            argv = this.SubVerbFormater.ConcatSubverbArgv(argv);

            var parser = this.Parser.ParseArguments<Options.CreateBucketOptions,
                                                    Options.ListBucketOptions,
                                                    Options.GetBucketOptions,
                                                    Options.SetBucketOptions,
                                                    Options.SyncFromBucketOptions,
                                                    Options.SyncToBucketOptions,
                                                    Options.TerminateBucketOptions>(argv);

            parser.MapResult(
                (Options.CreateBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.Create, o),
                (Options.GetBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.Download, o),
                (Options.SetBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.Upload, o),
                (Options.SyncFromBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.SyncFrom, o),
                (Options.SyncToBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.SyncTo, o),
                (Options.ListBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.List, o),
                (Options.TerminateBucketOptions o) => result = this.CreateConfig.ConvertBucketOption(ConfigType.Bucket, CommandApi.Delete, o),
                err => throw new ParseException(this.Usage.PrintHelp(parser, err)));

            return result;
        }
    }
}
