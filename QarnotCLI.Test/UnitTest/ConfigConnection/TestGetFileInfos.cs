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
    public class TestGetFileInfo
    {
        [Test]
        public void GetValueTestSplitATokenLine()
        {
            APIConnectionInformation apiInformation = new APIConnectionInformation();
            string line = "token=TOKEN123";

            var fileinfo = new ConfigurationFileReader();
            fileinfo.GetValue(line, apiInformation);
            Assert.AreEqual(apiInformation.Token, "TOKEN123");
        }

        [Test]
        public void GetValueTestSplitAUriLine()
        {
            APIConnectionInformation apiInformation = new APIConnectionInformation();
            string line = "uri=TOKEN123";

            var fileinfo = new ConfigurationFileReader();
            fileinfo.GetValue(line, apiInformation);
            Assert.AreEqual(apiInformation.ApiUri, "TOKEN123");
        }

        [Test]
        public void GetValueTestSplitAStorageLine()
        {
            APIConnectionInformation apiInformation = new APIConnectionInformation();
            string line = "storage=TOKEN123";

            var fileinfo = new ConfigurationFileReader();
            fileinfo.GetValue(line, apiInformation);
            Assert.AreEqual(apiInformation.StorageUri, "TOKEN123");
        }

        [Test]
        public void ParseLinesTestMoreThanRequiredVariablesSendNoError()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = new string[]
            {
                "token=TOKENTOEARLY",
                "token=TOKEN",
                "test=NOT1",
                "token123=NOT2",
                "abctoken=NOT3",
                "tokenTOKEN=",
                "uri=URI",
                "storage=BUCKET",
            };
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, "TOKEN");
            Assert.AreEqual(apiInformation.ApiUri, "URI");
            Assert.AreEqual(apiInformation.StorageUri, "BUCKET");
        }

        [Test]
        public void ParseLinesNoLinesReturnAnEmptyObject()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = Array.Empty<string>();
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, null);
            Assert.AreEqual(apiInformation.ApiUri, null);
            Assert.AreEqual(apiInformation.StorageUri, null);
        }

        [Test]
        public void ParseLinesEmptyListReturnAnEmptyObject()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = null;
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, null);
            Assert.AreEqual(apiInformation.ApiUri, null);
            Assert.AreEqual(apiInformation.StorageUri, null);
        }

        [Test]
        public void ParseLinesTestMissingInformationReturnAPartialObject()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = new string[]
            {
                "token=TOKEN",
            };
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, "TOKEN");
            Assert.AreEqual(apiInformation.ApiUri, null);
            Assert.AreEqual(apiInformation.StorageUri, null);
        }

        [Test]
        public void ParseLinesTestNonOrededReturnANormalObject()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = new string[]
            {
                "storage=BUCKET",
                "token=TOKEN",
                "uri=URI",
            };
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, "TOKEN");
            Assert.AreEqual(apiInformation.ApiUri, "URI");
            Assert.AreEqual(apiInformation.StorageUri, "BUCKET");
        }

        [Test]
        public void ParseLinesTestSimpleValidList()
        {
            var fileinfo = new ConfigurationFileReader();
            string[] lines = new string[]
            {
                "token=TOKEN",
                "uri=URI",
                "storage=BUCKET",
            };
            QarnotCLI.APIConnectionInformation apiInformation = fileinfo.ParseLines(lines);
            Assert.AreEqual(apiInformation.Token, "TOKEN");
            Assert.AreEqual(apiInformation.ApiUri, "URI");
            Assert.AreEqual(apiInformation.StorageUri, "BUCKET");
        }
    }
}
