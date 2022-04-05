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
    public class TestParseArgValidBucket
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void CreateBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            uint ttl = 36000;
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "create", "--name", name, "--ttl", ttl.ToString() };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(ttl, confset.CacheTTL);

            argv = new string[] { "bucket", "create", "-n", name };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Name, name);
            Assert.IsNull(confset.CacheTTL);
            commandLineParser.Dispose();
        }

        [Test]
        public void CreateBucketMissArgTestParsArg()
        {
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            ParseException ex = null;

            argv = new string[] { "bucket", "create" };
            ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
            Assert.IsNotNull(ex);
            commandLineParser.Dispose();
        }

        [Test]
        public void SetBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string folder = "/my/folder/";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "set", "--name", name, "--local-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Upload);
            Assert.AreEqual(confset.Name, name);

            argv = new string[] { "bucket", "set", "-n", name, "--local-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Upload);
            Assert.AreEqual(confset.Name, name);
            commandLineParser.Dispose();
        }

        [Test]
        public void PutBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string folder = "/my/folder/";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "put", "--name", name, "--local-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Upload);
            Assert.AreEqual(confset.Name, name);

            argv = new string[] { "bucket", "put", "-n", name, "--local-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Upload);
            Assert.AreEqual(confset.Name, name);
            commandLineParser.Dispose();
        }

        [Test]
        public void GetBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string folder = "/my/folder/";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "get", "--name", name, "--bucket-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Download);
            Assert.AreEqual(confset.Name, name);

            argv = new string[] { "bucket", "get", "-n", name, "--bucket-folder", folder };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Download);
            Assert.AreEqual(confset.Name, name);
            commandLineParser.Dispose();
        }

        [Test]
        public void SyncFromBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string local = "Local-path";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "sync-from", "--name", name, "--local-path", local };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.SyncFrom);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.LocalPathGet, local);

            argv = new string[] { "bucket", "sync-from", "-n", name, "-l", local };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.SyncFrom);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.LocalPathGet, local);
            commandLineParser.Dispose();
        }

        [Test]
        public void SyncToBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string local = "Local-path";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "sync-to", "--name", name, "--local-path", local };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.SyncTo);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.LocalPathGet, local);

            argv = new string[] { "bucket", "sync-to", "-n", name, "-l", local };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.SyncTo);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.LocalPathGet, local);
            commandLineParser.Dispose();
        }

        [Test]
        public void ListBucketOptionsCheckTestParsArg()
        {
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "list"};
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.List);

            argv = new string[] { "bucket", "list" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.List);
            commandLineParser.Dispose();
        }

        [Test]
        public void InfoBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "list", "--name", name, "-f", "folder1" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.InfoBucket);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.RemoteRelativePath, "folder1");

            argv = new string[] { "bucket", "list", "-n", name, "-f", "folder1" };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.InfoBucket);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.RemoteRelativePath, "folder1");
            commandLineParser.Dispose();
        }

        [Test]
        public void TerminateBucketOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "delete", "--name", name };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = iConfSet as BucketConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);

            argv = new string[] { "bucket", "delete", "-n", name };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Delete);
            Assert.AreEqual(confset.Name, name);
            commandLineParser.Dispose();
        }

        [Test]
        public void RemoveBucketFileOptionsCheckTestParsArg()
        {
            string name = "NAME";
            string path = "path";
            string[] argv = null;
            var commandLineParser = new CommandLine.Parser();
            CommandLineParser parser = new CommandLineParser(new OptionConverter(new JsonDeserializer()), commandLineParser, new ParserUsage(), new VerbFormater());
            IConfiguration iConfSet = null;
            BucketConfiguration confset = null;

            argv = new string[] { "bucket", "delete", "--name", name, path };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = iConfSet as BucketConfiguration;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Remove);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.RemoteRelativePaths[0], path);

            argv = new string[] { "bucket", "delete", "-n", name, path };
            iConfSet = parser.Parse(argv);

            if (!(iConfSet is BucketConfiguration))
            {
                throw new Exception("return value is not ConfigurationBucket ");
            }

            confset = (BucketConfiguration)iConfSet;
            Assert.AreEqual(confset.Type, ConfigType.Bucket);
            Assert.AreEqual(confset.Command, CommandApi.Remove);
            Assert.AreEqual(confset.Name, name);
            Assert.AreEqual(confset.RemoteRelativePaths[0], path);
            commandLineParser.Dispose();
        }
    }
}