namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestParseArgValidPool
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreatePoolCheckTestParsArg()
        {
            string name = "NAME";
            string shortname = "SHORT";
            string instance = "42";
            string profile = "PROFILE";
            string[] tags = new[] { "TAG1", "TAG2", "TAG3" };
            string[] constants = new[] { "CONSTANT" };
            string[] constraints = new[] { "CONSTRAINTS" };
            uint defaultTTL = 36000;
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "create", "--name", name, "--shortname", shortname, "--instanceNodes", instance, "--profile", profile, "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0], "--tasks-wait-for-synchronization", "false"  };
            argv = new string[] { "pool", "create", "--name=" + name, "--shortname=" + shortname, "--instanceNodes=" + instance, "--profile=" + profile, "--tags=" + tags[0], tags[1], tags[2], "--constants=" + constants[0], "--constraints=" + constraints[0], "--tasks-wait-for-synchronization", "true", "--export-credentials-to-env", "true", "--ttl", defaultTTL.ToString()  };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.Profile, profile);
            CollectionAssert.AreEqual(confset.Tags, tags);
            CollectionAssert.AreEqual(confset.Constants, constants);
            CollectionAssert.AreEqual(confset.Constraints, constraints);
            Assert.AreEqual(confset.InstanceCount, 42);
            Assert.AreEqual(confset.TasksDefaultWaitForPoolResourcesSynchronization, true);
            Assert.AreEqual(true, confset.ExportApiAndStorageCredentialsInEnvironment);
            Assert.AreEqual(defaultTTL, confset.DefaultResourcesCacheTTLSec);

            argv = new string[] { "pool", "create", "-n", name, "-s", shortname, "-i", instance, "-p", profile, "-t", tags[0], tags[1], tags[2], "-c", constants[0] };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.Profile, profile);
            CollectionAssert.AreEqual(confset.Tags, tags);
            CollectionAssert.AreEqual(confset.Constants, constants);
            Assert.AreEqual(confset.InstanceCount, 42);
            Assert.AreEqual(confset.TasksDefaultWaitForPoolResourcesSynchronization, false);
            Assert.IsNull(confset.ExportApiAndStorageCredentialsInEnvironment);
            Assert.IsNull(confset.DefaultResourcesCacheTTLSec);
            commandLineParser.Dispose();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreatePoolWithElasticSettingsAndNoInstanceNodesShouldNotFail(bool useLegacyNodeTerm)
        {
            string name = "NAME";
            string shortname = "SHORT";
            string minimumElasticSlots = "2";
            string profile = "PROFILE";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());

            argv = new string[] { "pool", "create", "--name", name, "--shortname", shortname, "--profile", profile, "--pool-is-elastic", useLegacyNodeTerm ? "--min-node": "--min-slot", minimumElasticSlots};
            var iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }
        }

        [Test]
        public void ListPoolCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "list" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.List);

            argv = new string[] { "pool", "list", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);

            argv = new string[] { "pool", "list", "-n", name, "-i", poolUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void InfoPoolCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            DefaultRunConfiguration confset = null;

            argv = new string[] { "pool", "info", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            Assert.IsFalse(confset.TagsIntersect);
            CollectionAssert.Contains(confset.Tags, tags);

            argv = new string[] { "pool", "info", "-n", name, "-i", poolUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            commandLineParser.Dispose();
        }

        [Test]
        [TestCase("list", CommandApi.List)]
        [TestCase("info", CommandApi.Info)]
        [TestCase("update-resources", CommandApi.UpdateResources)]
        [TestCase("update-constant", CommandApi.UpdateConstant)]
        [TestCase("set", CommandApi.Set)]
        [TestCase("set-elastic-settings", CommandApi.Set)]
        [TestCase("delete", CommandApi.Delete)]
        public void PoolBasicSubverbCanHaveAllTheBasicFlags(string subverb, CommandApi commandEnum)
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", subverb, "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);
            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, commandEnum);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            Assert.IsFalse(confset.TagsIntersect);
            CollectionAssert.Contains(confset.Tags, tags);

            argv = new string[] { "pool", subverb, "--name", name, "--id", poolUuid, "--exclusive-tags", tags };
            iConfSet = parser.Parse(argv);
            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, commandEnum);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            Assert.IsTrue(confset.TagsIntersect);
            CollectionAssert.Contains(confset.Tags, tags);
        }

        [Test]
        [TestCase("list")]
        [TestCase("info")]
        [TestCase("update-resources")]
        [TestCase("set")]
        [TestCase("set-elastic-settings")]
        [TestCase("delete")]
        public void PoolSubverbCannotHaveTagsAndTagIntersect(string subverb)
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            argv = new string[] { "pool", subverb, "--name", name, "--id", poolUuid, "--tags", tags, "--exclusive-tags", tags };
            ParseException ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.NotNull(ex);

            // controle
            parser.Parse(new string[] { "pool", subverb, "--name", name, "--id", poolUuid, "--tags", tags });
            parser.Parse(new string[] { "pool", subverb, "--name", name, "--id", poolUuid, "--exclusive-tags", tags });
        }

        [Test]
        public void UpdatePoolResourcesCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            using var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "update-resources", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.UpdateResources);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);

            argv = new string[] { "pool", "update-resources", "-n", name, "-i", poolUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.UpdateResources);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
        }

        [TestCase("set", "node")] // Legacy
        [TestCase("set-elastic-settings", "node")]
        [TestCase("set-elastic-settings", "slot")]
        public void SetPoolElasticSettingsCheckTestParsArg(string verb, string elasticSlotTerm)
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", verb, "--name", name, "--id", poolUuid, $"--min-{elasticSlotTerm}", "1", $"--max-{elasticSlotTerm}", "2", $"--min-idling-{elasticSlotTerm}", "3", "--resize-period", "4", "--resize-factor", "5", "--min-idling-time", "6" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is PoolSetElasticSettingsConfiguration))
            {
                throw new Exception("return value is not PoolSetConfiguration ");
            }

            PoolSetElasticSettingsConfiguration confset = (PoolSetElasticSettingsConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Set);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            Assert.AreEqual(confset.ElasticMinimumTotalSlots, 1);
            Assert.AreEqual(confset.ElasticMaximumTotalSlots, 2);
            Assert.AreEqual(confset.ElasticMinimumIdlingSlots, 3);
            Assert.AreEqual(confset.ElasticResizePeriod, 4);
            Assert.AreEqual(confset.ElasticResizeFactor, 5);
            Assert.AreEqual(confset.ElasticMinimumIdlingTime, 6);

            argv = new string[] { "pool", verb, "-n", name, "-i", poolUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is PoolSetElasticSettingsConfiguration))
            {
                throw new Exception("return value is not PoolSetConfiguration ");
            }

            confset = (PoolSetElasticSettingsConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Set);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void DeletePoolCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "delete", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);

            argv = new string[] { "pool", "delete", "-n", name, "-i", poolUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolMinimumTestParsArg()
        {
            string[] argv = new string[] { "pool", "create", "-n", "name", "-p", "profile", "-i", "5" };
            string[] argvFull = new string[] { "pool", "create", "--name", "name", "--profile", "profile", "--instanceNodes", "5" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = parser.Parse(argv);
            IConfiguration iConfSetFull = parser.Parse(argvFull);

            if (!(iConfSet is CreateConfiguration) || !(iConfSetFull is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            confset = (CreateConfiguration)iConfSetFull;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreatePoolFullTestParsArg()
        {
            string[] argv = new string[] { "pool", "create", "-n", "name", "-s", "shortname", "-f", string.Empty, "-t", "tag1,tag2", "-c", "constants", "-p", "profile", "-i", "5" };
            string[] argvFull = new string[] { "pool", "create", "--name", "name", "--shortname", "shortname", "--file", string.Empty, "--tags", "tag1,tag2", "--constants", "constants", "--profile", "profile", "--instanceNodes", "5" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = parser.Parse(argv);
            IConfiguration iConfSetFull = parser.Parse(argvFull);

            if (!(iConfSet is CreateConfiguration) || !(iConfSetFull is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            confset = (CreateConfiguration)iConfSetFull;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            commandLineParser.Dispose();
        }

        [Test]
        public void UpdatePoolConstantTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string constantName = "SOME_CONSTANT";
            string constantValue = "some-new-value";
            string[] argv = null;
            using var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "update-constant", "--name", name, "--id", poolUuid, "--tags", tags, "--constant-name", constantName, "--constant-value", constantValue };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is ConstantUpdateConfiguration))
            {
                throw new Exception("return value is not ConstantUpdateConfiguration ");
            }

            ConstantUpdateConfiguration confset = (ConstantUpdateConfiguration)iConfSet;
            Assert.AreEqual(ConfigType.Pool, confset.Type);
            Assert.AreEqual(CommandApi.UpdateConstant, confset.Command);
            Assert.AreEqual(name, confset.Name);
            Assert.AreEqual(poolUuid, confset.Id);
            Assert.AreEqual(constantName, confset.ConstantName);
            Assert.AreEqual(constantValue, confset.ConstantValue);

            argv = new string[] { "pool", "update-constant", "-n", name, "-i", poolUuid, "-t", tags, "--constant-name", constantName, "--constant-value", constantValue };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is ConstantUpdateConfiguration))
            {
                throw new Exception("return value is not ConstantUpdateConfiguration ");
            }

            confset = (ConstantUpdateConfiguration)iConfSet;
            Assert.AreEqual(ConfigType.Pool, confset.Type);
            Assert.AreEqual(CommandApi.UpdateConstant, confset.Command);
            Assert.AreEqual(name, confset.Name);
            Assert.AreEqual(poolUuid, confset.Id);
            Assert.AreEqual(constantName, confset.ConstantName);
            Assert.AreEqual(constantValue, confset.ConstantValue);
        }
    }
}