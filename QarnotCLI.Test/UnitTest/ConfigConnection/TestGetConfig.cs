namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class GetConfigTest
    {
        public class FakeGetEnvConnectionInformation : IEnvironmentVariableReader
        {
            public bool EnvToken { get; set; }

            public bool EnvApi { get; set; }

            public bool EnvStorage { get; set; }

            public bool EnvEmail { get; set; }

            public bool EnvForce { get; set; }

            public FakeGetEnvConnectionInformation(bool envToken, bool envApi, bool envStorage, bool envForce = false)
            {
                EnvToken = envToken;
                EnvApi = envApi;
                EnvStorage = envStorage;
                EnvForce = envForce;
            }

            public bool GetEnvironmentVariableBoolOrElse(string envName, bool elseValue = false)
            {
                return elseValue;
            }

            public void RetrieveEnvironmentInformation(APIConnectionInformation api)
            {
                if (EnvToken)
                {
                    api.SetToken = "token_env";
                }

                if (EnvApi)
                {
                    api.SetApiUri = "api_env";
                }

                if (EnvStorage)
                {
                    api.SetStorageUri = "storage_env";
                }

                if (EnvEmail)
                {
                    api.SetAccountEmail = "email_env";
                }

                if (EnvStorage)
                {
                    api.SetForcePathStyle = true;
                }
            }
        }

        public class FakeEnvironmentPathGetter : IConfigurationFileGetter
        {
            public string ConfigurationFilePath
            {
                get
                {
                    return "FakeEnvironmentPathGetter";
                }
            }

            public string NewConfigurationFilePath
            {
                get
                {
                    return "FakeEnvironmentPathGetter";
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
            public APIConnectionInformation ReadFile(string filePath)
            {
                var api = new APIConnectionInformation()
                {
                    Token = "token_file",
                    ApiUri = "api_file",
                    StorageUri = "storage_file",
                    AccountEmail = "email_file",
                };
                return api;
            }
        }

        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void TestTokenSetBeforeNoEnvVarAddFileUris()
        {
            FakeGetEnvConnectionInformation fakeGetEnvConnectionInformation = new FakeGetEnvConnectionInformation(false, false, false);
            FakeEnvironmentPathGetter fakeEnvironmentPathGetter = new FakeEnvironmentPathGetter();
            FakeFileInformationGetter fakeFileInformationGetter = new FakeFileInformationGetter();
            var ret = new ApiConnectionConfigurationRetriever(fakeEnvironmentPathGetter, fakeGetEnvConnectionInformation, fakeFileInformationGetter);
            var api = new APIConnectionInformation()
            {
                Token = "token0",
                ApiUri = null,
                StorageUri = null,
                SetForcePathStyle = null,
            };
            ret.RetrieveConfigurationInformation(api);
            Assert.AreEqual(api.Token, "token0");
            Assert.AreEqual(api.ApiUri, "api_file");
            Assert.AreEqual(api.StorageUri, "storage_file");
            Assert.AreEqual(api.ForcePathStyle, false);
            Assert.AreEqual(api.GetForcePathStyle, null);
        }

        [Test]
        public void TestAllEnvVarSet()
        {
            var fakeGetEnvConnectionInformation = new FakeGetEnvConnectionInformation(true, true, true);
            FakeEnvironmentPathGetter fakeEnvironmentPathGetter = new FakeEnvironmentPathGetter();
            FakeFileInformationGetter fakeFileInformationGetter = new FakeFileInformationGetter();
            var ret = new ApiConnectionConfigurationRetriever(fakeEnvironmentPathGetter, fakeGetEnvConnectionInformation, fakeFileInformationGetter);
            var api = new APIConnectionInformation()
            {
                Token = null,
                ApiUri = null,
                StorageUri = null,
                SetForcePathStyle = null,
            };
            ret.RetrieveConfigurationInformation(api);
            Assert.AreEqual(api.Token, "token_env");
            Assert.AreEqual(api.ApiUri, "api_env");
            Assert.AreEqual(api.StorageUri, "storage_env");
            Assert.AreEqual(api.ForcePathStyle, true);
        }

        [Test]
        public void TestUriEnvVarSet()
        {
            var fakeGetEnvConnectionInformation = new FakeGetEnvConnectionInformation(false, true, false);
            FakeEnvironmentPathGetter fakeEnvironmentPathGetter = new FakeEnvironmentPathGetter();
            FakeFileInformationGetter fakeFileInformationGetter = new FakeFileInformationGetter();
            var ret = new ApiConnectionConfigurationRetriever(fakeEnvironmentPathGetter, fakeGetEnvConnectionInformation, fakeFileInformationGetter);
            var api = new APIConnectionInformation()
            {
                Token = null,
                ApiUri = null,
                StorageUri = null,
                AccountEmail = null,
                ForcePathStyle = false,
            };
            ret.RetrieveConfigurationInformation(api);
            Assert.AreEqual(api.Token, "token_file");
            Assert.AreEqual(api.ApiUri, "api_env");
            Assert.AreEqual(api.StorageUri, "storage_file");
            Assert.AreEqual(api.AccountEmail, "email_file");
            Assert.AreEqual(api.ForcePathStyle, false);
            Assert.AreEqual(api.GetForcePathStyle, false);
        }

        [Test]
        public void TestNoEnvVarSet()
        {
            var fakeGetEnvConnectionInformation = new FakeGetEnvConnectionInformation(false, false, false);
            FakeEnvironmentPathGetter fakeEnvironmentPathGetter = new FakeEnvironmentPathGetter();
            FakeFileInformationGetter fakeFileInformationGetter = new FakeFileInformationGetter();
            var ret = new ApiConnectionConfigurationRetriever(fakeEnvironmentPathGetter, fakeGetEnvConnectionInformation, fakeFileInformationGetter);
            var api = new APIConnectionInformation()
            {
                Token = null,
                ApiUri = null,
                StorageUri = null,
            };
            ret.RetrieveConfigurationInformation(api);
            Assert.AreEqual(api.Token, "token_file");
            Assert.AreEqual(api.ApiUri, "api_file");
            Assert.AreEqual(api.StorageUri, "storage_file");
        }
    }
}
