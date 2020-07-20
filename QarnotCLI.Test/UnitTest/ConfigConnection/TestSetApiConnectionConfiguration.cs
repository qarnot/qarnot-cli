namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestApiConnectionConfigurationSetter
    {
        private static string testFolder = Path.Join(Path.GetTempPath(), "QarnotCli", Guid.NewGuid().ToString());

        private static string testFile = Path.Join(testFolder, Guid.NewGuid().ToString() + ".tmp");

        public ApiConnectionConfigurationWritter Set { get; set; }

        public FakeFileWrapper FileWrap { get; set; }

        public FakeSetWriter Writer { get; set; }

        [OneTimeSetUp]
        public void CreateFolder()
        {
            System.IO.Directory.CreateDirectory(testFolder);
        }

        [OneTimeTearDown]
        public void DeleteFolder()
        {
            System.IO.Directory.Delete(testFolder, true);
        }

        public class FakeSetWriter : IFileStreamWriter
        {
            public Dictionary<string, string> Checker { get; set; } = new Dictionary<string, string>();

            public List<string> Prefix { get; set; } = new List<string>();

            public List<string> Value { get; set; } = new List<string>();

            public List<FileStream> Fs { get; set; } = new List<FileStream>();

            public void Write(string prefix, string value, FileStream fs)
            {
                Checker[prefix] = value;
                Prefix.Add(prefix);
                Value.Add(value);
                Fs.Add(fs);
            }
        }

        public class FakeEnvironmentPathGetter : IConfigurationFileGetter
        {
            public static string FakePath { get; set; }

            public FakeEnvironmentPathGetter()
            {
                FakePath = Path.Join(testFolder, "cli.config");
            }

            public string ConfigurationFilePath
            {
                get
                {
                    return FakePath;
                }
            }

            public string NewConfigurationFilePath
            {
                get
                {
                    return FakePath;
                }
            }

            public bool CanConfigurationFileBeCreated()
            {
                return true;
            }

            public bool DoesConfigurationFileExist()
            {
                return true;
            }
        }

        public class FakeFileInformationGetter : IConfigurationFileReader
        {
            public static string Token { get; set; } = "Token";

            public static string ApiUri { get; set; } = "ApiUri";

            public static string StorageUri { get; set; } = "StorageUri";

            public static string AccountEmail { get; set; } = "Email";

            public static bool? ForcePath { get; set; } = true;

            public APIConnectionInformation ReadFile(string filePath)
            {
                var api = new APIConnectionInformation();
                api.Token = Token;
                api.ApiUri = ApiUri;
                api.StorageUri = StorageUri;
                api.AccountEmail = AccountEmail;
                api.ForcePathStyle = ForcePath.Value;
                return api;
            }
        }

        public class FakeFileWrapper : IFileWrapper
        {
            public string PathFind { get; set; } = null;

            public FileStream CreateFile(string path)
            {
                PathFind = path;
                return File.Create(testFile);
            }

            public void DeleteFile(string path)
            {
                path = null;
            }

            public bool IsFileExist { get; set; } = true;

            public bool DoesFileExist(string path)
            {
                return IsFileExist;
            }

            public bool CreateDirectoryAsk { get; set; } = false;

            public void CreateDirectory(string path)
            {
                path = null;
                CreateDirectoryAsk = true;
            }

            public bool IsDirectoryExist { get; set; } = true;

            public bool DirectoryExist(string path)
            {
                return IsDirectoryExist;
            }

            public string DirectoryName { get; set; } = "directory";

            public string GetDirectoryName(string path)
            {
                return DirectoryName;
            }
        }

        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            FileWrap = new FakeFileWrapper();
            Writer = new FakeSetWriter();
            Set = new ApiConnectionConfigurationWritter(
                new FakeEnvironmentPathGetter(),
                new FakeFileInformationGetter(),
                Writer,
                FileWrap);
        }

        [Test]
        public void TestGetPathPathGiven()
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration(ConfigType.Config, CommandApi.Set)
            {
                Path = "/tmp/path",
            };
            Assert.AreEqual(Set.GetPath(config), "/tmp/path");
        }

        [Test]
        public void TestGetPathGlobalPathAsk()
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration(ConfigType.Config, CommandApi.Set)
            {
                GlobalPath = true,
            };
            Assert.AreEqual(Set.GetPath(config), QarnotCLI.ConfigurationFileGetter.DefaultPathName);
        }

        [Test]
        public void TestNoPathAskEnvironmentPathGetter()
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration(ConfigType.Config, CommandApi.Set);
            Assert.AreEqual(Set.GetPath(config), FakeEnvironmentPathGetter.FakePath);
        }

        [Test]
        public void CreateDirectoryIfNotExist()
        {
            FileWrap.CreateDirectoryAsk = false;
            FileWrap.IsDirectoryExist = true;
            Set.CheckDirectory("test");
            Assert.IsFalse(FileWrap.CreateDirectoryAsk);
        }

        [Test]
        public void DoCreateDirectoryIfExist()
        {
            FileWrap.CreateDirectoryAsk = false;
            FileWrap.IsDirectoryExist = false;
            Set.CheckDirectory("test");
            Assert.IsTrue(FileWrap.CreateDirectoryAsk);
        }

        [Test]
        public void CheckIfYouCAnGetInfoFromFile()
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration()
            {
                ApiConnection = new APIConnectionInformation()
            };
            Set.SetConfigInformation("path", config);
            Assert.AreEqual(Writer.Checker["token"], FakeFileInformationGetter.Token);
            Assert.AreEqual(Writer.Checker["uri"], FakeFileInformationGetter.ApiUri);
            Assert.AreEqual(Writer.Checker["storage"], FakeFileInformationGetter.StorageUri);
            Assert.AreEqual(Writer.Checker["account-email"], FakeFileInformationGetter.AccountEmail);
            Assert.AreEqual(Writer.Checker["force-path"], FakeFileInformationGetter.ForcePath.ToString());
        }

        [Test]
        public void CheckInfoWellSendToTheWriter()
        {
            LocalSetUpConfiguration config = new LocalSetUpConfiguration();
            APIConnectionInformation connectionInformation = new APIConnectionInformation();
            connectionInformation.Token = "token123";
            connectionInformation.ApiUri = "uri123";
            connectionInformation.StorageUri = "bucket123";
            connectionInformation.AccountEmail = "email123";
            connectionInformation.ForcePathStyle = false;
            config.ApiConnection = connectionInformation;

            Set.SetConfigInformation("path123", config);

            Assert.AreEqual(FileWrap.PathFind, "path123");
            Assert.AreEqual(Writer.Checker["token"], "token123");
            Assert.AreEqual(Writer.Checker["uri"], "uri123");
            Assert.AreEqual(Writer.Checker["account-email"], "email123");
            Assert.AreEqual(Writer.Checker["force-path"], "False");
        }
    }
}