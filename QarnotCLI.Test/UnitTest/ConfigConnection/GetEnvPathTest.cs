namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using QarnotCLI;
    using Moq;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class EnvironmentPathGetterTest
    {
        private static string testFolder = Path.Join(Path.GetTempPath(), "QarnotCli", Guid.NewGuid().ToString());
        private static string testFile = Path.Join(testFolder, Guid.NewGuid().ToString() + ".tmp");

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

        // integration tests
        [Test]
        public void CheckFileValid()
        {
            string validFile = testFile;
            using (var file = System.IO.File.Create(validFile))
            {
            }

            bool result = ConfigurationFileGetter.DoesFileExist(validFile);
            System.IO.File.Delete(validFile);
            Assert.IsTrue(result);
        }

        // integration tests
        [Test]
        public void GetConfigOneEnvTest()
        {
            string name = "UnitTestEnvVariableName" + Guid.NewGuid().ToString();
            string value = "UnitTestEnvVariableValue";
            Environment.SetEnvironmentVariable(name, value);
            ConfigurationFileGetter envPath = new ConfigurationFileGetter();
            string returnVal = envPath.GetEnvironmentVariableValue(name);
            Environment.SetEnvironmentVariable(name, null);
            Assert.AreEqual(returnVal, value);
        }

        // integration tests
        [Test]
        public void CheckFileInvalid()
        {
            string invalidFile = testFile;
            System.IO.File.Delete(invalidFile);
            Assert.IsFalse(ConfigurationFileGetter.DoesFileExist(invalidFile));
        }

        [Test]
        public void CheckFileEmpty()
        {
            Assert.IsFalse(ConfigurationFileGetter.DoesFileExist(string.Empty));
        }

        [Test]
        public void CheckFileNull()
        {
            Assert.IsFalse(ConfigurationFileGetter.DoesFileExist(null));
        }
    }
}
