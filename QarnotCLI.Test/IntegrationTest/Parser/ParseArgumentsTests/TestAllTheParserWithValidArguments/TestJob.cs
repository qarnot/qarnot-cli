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
    public class TestParseArgValidJob
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateJobSimpleTestParsArg()
        {
            string[] argv = new string[] { "job", "create", "-n", "name" };
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            commandLineParser.Dispose();
        }

        [Test]
        [TestCase("12", 12, 0, 0, 0)]
        [TestCase("12.18", 12, 18, 0, 0)]
        [TestCase("12.00:46", 12, 0, 46, 0)]
        [TestCase("12.02:03:10", 12, 2, 3, 10)]
        [TestCase("12.00:00:10", 12, 0, 0, 10)]
        [TestCase("1:2:3", 0, 1, 2, 3)]
        [TestCase("01:00:10", 0, 1, 0, 10)]
        [TestCase("0:10", 0, 0, 10, 0)]
        [TestCase(".::10", 0, 0, 0, 10)]
        [TestCase(":10", 0, 0, 10, 0)]
        public void CreatejobCheckWallTime(string walltime, int day, int hour, int minute, int second)
        {
            TimeSpan waitValue = new TimeSpan(day, hour, minute, second);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;

            Assert.AreEqual(confset.MaximumWallTime, waitValue);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(45620)]
        [TestCase(24968746)]
        public void CreatejobCheckWallTimeWithFullDate(int secondToAdd)
        {
            string walltime = DateTime.Now.AddSeconds(secondToAdd).ToString("yyyy/MM/dd HH:mm:ss");
            TimeSpan waitValue = new TimeSpan(0, 0, 0, secondToAdd);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;

            Assert.AreEqual(Math.Ceiling(confset.MaximumWallTime.Value.TotalSeconds), Math.Ceiling(waitValue.TotalSeconds));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CreatejobCheckWallTimeWithPartMDate(int dayToAdd)
        {
            string walltime = DateTime.Now.AddDays(dayToAdd).ToString("yyyy/MM/dd");
            TimeSpan waitValue = new TimeSpan(dayToAdd, 0, 0, 0);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            Console.WriteLine(walltime);
            Console.WriteLine(waitValue.TotalDays);
            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;
            Console.WriteLine(confset.MaximumWallTime.Value.TotalDays);
            Assert.AreEqual(Math.Ceiling(confset.MaximumWallTime.Value.TotalDays), Math.Ceiling(waitValue.TotalDays));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CreatejobCheckWallTimeWithOnlyDay(int dayToAdd)
        {
            string walltime = dayToAdd.ToString();
            TimeSpan waitValue = new TimeSpan(dayToAdd, 0, 0, 0);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            Console.WriteLine(walltime);
            Console.WriteLine(waitValue.TotalHours);
            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;
            Console.WriteLine(confset.MaximumWallTime.Value.TotalHours);
            Assert.AreEqual(Math.Ceiling(confset.MaximumWallTime.Value.TotalHours), Math.Ceiling(waitValue.TotalHours));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CreatejobCheckWallTimeWithOnlyHours(int hourToAdd)
        {
            string walltime = "0." + hourToAdd.ToString();
            TimeSpan waitValue = new TimeSpan(0, hourToAdd, 0, 0);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            Console.WriteLine(walltime);
            Console.WriteLine(waitValue.TotalHours);
            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;
            Console.WriteLine(confset.MaximumWallTime.Value.TotalHours);
            Assert.AreEqual(confset.MaximumWallTime.Value.TotalHours, waitValue.TotalHours);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CreatejobCheckWallTimeWithOnlyMinutes(int minuteToAdd)
        {
            string walltime = "0.0:" + minuteToAdd.ToString();
            TimeSpan waitValue = new TimeSpan(0, 0, minuteToAdd, 0);
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), new CommandLine.Parser(), new ParserUsage(), new VerbFormater());

            Console.WriteLine(walltime);
            Console.WriteLine(waitValue.TotalMinutes);
            string[] argv = new string[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime };
            CreateConfiguration confset = parser.Parse(argv) as CreateConfiguration;
            Console.WriteLine(confset.MaximumWallTime.Value.TotalMinutes);
            Assert.AreEqual(Math.Ceiling(confset.MaximumWallTime.Value.TotalMinutes), Math.Ceiling(waitValue.TotalMinutes));
        }

        [Test]
        public void CreatejobCheckTestParsArg()
        {
            string poolUuid = "PoolUUID";
            string name = "NAME";
            string shortname = "SHORT";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "create", "--name", name, "--pool", poolUuid, "--shortname", shortname};
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            CreateConfiguration confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.PoolUuid, poolUuid);

            argv = new string[] { "job", "create", "-n", name, "--pool", poolUuid, "-s", shortname };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is CreateConfiguration))
            {
                throw new Exception("return value is not CreateConfiguration ");
            }

            confset = (CreateConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Shortname, shortname);
            Assert.AreEqual(confset.PoolUuid, poolUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void ListJobCheckTestParsArg()
        {
            string jobUuid = "JobUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "list", "--name", name, "--id", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);

            argv = new string[] { "job", "list", "-n", name, "-i", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.List);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void InfoJobCheckTestParsArg()
        {
            string jobUuid = "JobUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "info", "--name", name, "--id", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);

            argv = new string[] { "job", "info", "-n", name, "-i", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Info);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void WaitJobCheckTestParsArg()
        {
            string jobUuid = "JobUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "wait", "--name", name, "--id", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Wait);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);

            argv = new string[] { "job", "wait", "-n", name, "-i", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Wait);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void AbortJobCheckTestParsArg()
        {
            string jobUuid = "JobUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "abort", "--name", name, "--id", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Abort);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);

            argv = new string[] { "job", "abort", "-n", name, "-i", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Abort);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);
            commandLineParser.Dispose();
        }

        [Test]
        public void DeleteJobCheckTestParsArg()
        {
            string jobUuid = "JobUUID";
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;

            argv = new string[] { "job", "delete", "--name", name, "--id", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            DefaultRunConfiguration confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);

            argv = new string[] { "job", "delete", "-n", name, "-i", jobUuid };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is DefaultRunConfiguration))
            {
                throw new Exception("return value is not DefaultRunConfiguration ");
            }

            confset = (DefaultRunConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Job);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.Id, jobUuid);
            commandLineParser.Dispose();
        }
    }
}