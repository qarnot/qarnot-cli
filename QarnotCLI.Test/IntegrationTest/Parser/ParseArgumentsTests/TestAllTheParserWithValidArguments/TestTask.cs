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
    public class TestParsArgValidTasks
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateTaskCheckTestParsArg()
        {
            string name = "NAME";
            string shortname = "SHORT";
            string range = "1-5";
            string instance = "42";
            string profile = "PROFILE";
            string[] tags = new[] { "TAG1", "TAG2", "TAG3" };
            string[] constants = new[] { "CONSTANT" };
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "create", "--name", name, "--shortname", shortname, "--instance", instance, "--profile", profile, "--tags", tags[0], tags[1], tags[2], "--constants", constants[0] };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.Profile, profile);
            CollectionAssert.AreEqual(confset.Tags, tags);
            CollectionAssert.AreEqual(confset.Constants, constants);
            Assert.AreEqual(confset.InstanceCount, int.Parse(instance));

            argv = new string[] { "task", "create", "--name", name, "--shortname", shortname, "--range", range, "--profile", profile, "--tags", tags[0], tags[1], tags[2], "--constants", constants[0] };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.Profile, profile);
            CollectionAssert.AreEqual(confset.Tags, tags);
            CollectionAssert.AreEqual(confset.Constants, constants);
            Assert.AreEqual(confset.Range, range);

            argv = new string[] { "task", "create", "-n", name, "-s", shortname, "-i", instance, "-p", profile, "-t", tags[0], tags[1], tags[2], "-c", constants[0] };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.Profile, profile);
            CollectionAssert.AreEqual(confset.Tags, tags);
            CollectionAssert.AreEqual(confset.Constants, constants);
            Assert.AreEqual(confset.InstanceCount, int.Parse(instance));
            commandLineParser.Dispose();
        }

        [Test]
        public void ListTaskCheckTestParsArg()
        {
            string taskUuid = "TaskUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "list", "--name", name, "--id", taskUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);

            argv = new string[] { "task", "list", "-n", name, "-i", taskUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void InfoTaskCheckTestParsArg()
        {
            string taskUuid = "TaskUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "info", "--name", name, "--id", taskUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);

            argv = new string[] { "task", "info", "-n", name, "-i", taskUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void WaitTaskCheckTestParsArg()
        {
            string taskUuid = "TaskUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "wait", "--name", name, "--id", taskUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Wait);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);

            argv = new string[] { "task", "wait", "-n", name, "-i", taskUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Wait);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void AbortTaskCheckTestParsArg()
        {
            string taskUuid = "TaskUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "abort", "--name", name, "--id", taskUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Abort);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);

            argv = new string[] { "task", "abort", "-n", name, "-i", taskUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Abort);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void DeleteTaskCheckTestParsArg()
        {
            string taskUuid = "TaskUUID";
            string name = "NAME";
            string tags = "TAG1,TAG2";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "task", "delete", "--name", name, "--id", taskUuid, "--tags", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);

            argv = new string[] { "task", "delete", "-n", name, "-i", taskUuid, "-t", tags };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Task);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, taskUuid);
            commandLineParser.Dispose();
        }
    }
}