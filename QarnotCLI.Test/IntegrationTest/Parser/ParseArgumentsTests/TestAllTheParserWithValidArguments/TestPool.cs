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
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "create", "--name", name, "--shortname", shortname, "--instanceNodes", instance, "--profile", profile, "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0] };
            argv = new string[] { "pool", "create", "--name=" + name, "--shortname=" + shortname, "--instanceNodes=" + instance, "--profile=" + profile, "--tags=" + tags[0], tags[1], tags[2], "--constants=" + constants[0], "--constraints=" + constraints[0] };
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
            commandLineParser.Dispose();
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

            argv = new string[] { "pool", "list", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
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

            argv = new string[] { "pool", "info", "--name", name, "--id", poolUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);

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

        [Test]
        public void SetPoolCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "pool", "set", "--name", name, "--id", poolUuid, "--min-node", "1", "--max-node", "2", "--min-ling-node", "3", "--resize-periode", "4", "--resize-factor", "5", "--min-ling-time", "6" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is PoolSetConfiguration))
            {
                throw new Exception("return value is not PoolSetConfiguration ");
            }

            PoolSetConfiguration confset = (PoolSetConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Pool);
            Assert.AreEqual(confset.Command, CommandApi.Set);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, poolUuid);
            Assert.AreEqual(confset.ElasticMinimumTotalNodes, 1);
            Assert.AreEqual(confset.ElasticMaximumTotalNodes, 2);
            Assert.AreEqual(confset.ElasticMinimumIdlingNodes, 3);
            Assert.AreEqual(confset.ElasticResizePeriod, 4);
            Assert.AreEqual(confset.ElasticResizeFactor, 5);
            Assert.AreEqual(confset.ElasticMinimumIdlingTime, 6);

            argv = new string[] { "pool", "set", "-n", name, "-i", poolUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is PoolSetConfiguration))
            {
                throw new Exception("return value is not PoolSetConfiguration ");
            }

            confset = (PoolSetConfiguration)iConfSet;
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
    }
}