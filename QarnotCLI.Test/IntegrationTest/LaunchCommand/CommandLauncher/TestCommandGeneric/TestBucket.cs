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
    using QarnotSDK;
    using FizzWare.NBuilder;
    using Moq;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestCommandsBucket
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
        }

        public class FakeBucket : QBucket
        {

            public override string Shortname { get; protected set; } = "NewShortname";

            public override int FileCount { get; } = 156;

            public override long UsedSpaceBytes { get; } = 54213;

            public FakeBucket(): base (null, null, false) {}

            public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
            {
                await Task.Delay(20);
                return;
            }

            public override async Task SyncRemoteToLocalAsync(string localFolderPath, CancellationToken cancellationToken, bool dontDelete = true, string remoteFolderRelativePath = "")
            {
                await Task.Delay(20);
                return;
            }

            public override async Task SyncLocalToRemoteAsync(string localFolderPath, CancellationToken cancellationToken, bool dontDelete = true, string remoteFolderRelativePath = "")
            {
                await Task.Delay(20);
                return ;
            }

            public override async Task UpdateAsync(CancellationToken cancellationToken)
            {
                await Task.Delay(20);
                return ;
            }

            public override async Task<List<QAbstractStorageEntry>> ListEntriesAsync(string folder, CancellationToken cancellationToken)
            {
                Mock<QAbstractStorageEntry> mockAStorage = new Mock<QAbstractStorageEntry>();
                string path = "test/path";
                mockAStorage.Setup(foo => foo.Size).Returns(123456);
                mockAStorage.Setup(foo => foo.Name).Returns(path);

                QAbstractStorageEntry entity = mockAStorage.Object;

                await Task.Delay(20);
                return new List<QAbstractStorageEntry>(){ entity };
            }

            public override async Task UploadStringAsync(string content, string remoteFile, Encoding encoding = null, CancellationToken cancellationToken = default(CancellationToken))
            {
                await Task.Delay(20);
                return;
            }

            public override async Task<string> DownloadStringAsync(string remoteFile, Encoding encoding = null, CancellationToken cancellationToken = default(CancellationToken))
            {
                await Task.Delay(20);
                return "string";
            }

            public override async Task DeleteEntryAsync(string remoteFile, CancellationToken cancellationToken = default(CancellationToken))
            {
                await Task.Delay(20);
                return ;
            }
        }

        [Test]
        public async Task TestToGetABucketListAndCheckTheReturnStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new BucketListResumeCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>() {"folder1", "folder2"},
                DeleteFiles = true,
            };
            CommandValues.BucketCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.FileCount, "156");
            Assert.AreEqual(ret.UsedSpaceBytes, "54213");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task TestToGetABucketListAndCheckTheHumanReadableReturnStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new BucketListResumeCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>() {"folder1", "folder2"},
                DeleteFiles = true,
                HumanReadable = true,
            };
            CommandValues.BucketCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.FileCount, "156");
            Assert.AreEqual(ret.UsedSpaceBytes, " 52.9 KB");
        }

        [Test]
        public async Task TestToGetFilesInfoFromABucketAndCheckTheReturnStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new ListBucketEntriesCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>() {"folder1", "folder2"},
                DeleteFiles = true,
            };
            CommandValues.BucketInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketInfoCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.BucketFilesDetail[0], " 123456 - test/path");
            Assert.AreEqual(ret.TotalFileCount, "156");
            Assert.AreEqual(ret.TotalUsedSpaceBytes, "54213");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task TestToGetFilesInfoFromABucketAndCheckTheReturnHumanReadableStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new ListBucketEntriesCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>() {"folder1", "folder2"},
                DeleteFiles = true,
                HumanReadable = true,
            };
            CommandValues.BucketInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketInfoCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.BucketFilesDetail[0], "120.56 KB - test/path");
            Assert.AreEqual(ret.TotalFileCount, "156");
            Assert.AreEqual(ret.TotalUsedSpaceBytes, "52.94 KB");
        }

        [Test]
        public async Task TestToGetAllInfoFromABucketWithNoFolderSpecifiedAndCheckTheReturnStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new ListBucketEntriesCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>(),
                DeleteFiles = true,
            };
            CommandValues.BucketInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketInfoCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.BucketFilesDetail[0], " 123456 - test/path");
            Assert.AreEqual(ret.TotalFileCount, "156");
            Assert.AreEqual(ret.TotalUsedSpaceBytes, "54213");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task TestToGetAllInfoFromABucketWithNoFolderSpecifiedAndCheckTheHumanReadableReturnStringInfo()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new ListBucketEntriesCommand(new BytesFormatter());
            var config = new BucketConfiguration() {
                Name = "myName",
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                RemoteRelativePathFolders = new List<string>(),
                DeleteFiles = true,
                HumanReadable = true,
            };
            CommandValues.BucketInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.BucketInfoCommandValue);
            Assert.AreEqual(ret.Shortname, "Shortname1");
            Assert.AreEqual(ret.BucketFilesDetail[0], "120.56 KB - test/path");
            Assert.AreEqual(ret.TotalFileCount, "156");
            Assert.AreEqual(ret.TotalUsedSpaceBytes, "52.94 KB");
        }

        [Test]
        public async Task TestCommandSynchronizeLocalFolderToBucket()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new SynchronizeLocalFolderToBucketCommand();
            var config = new BucketConfiguration() {
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }

        [Test]
        public async Task TestCommandDeleteBucket()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new DeleteBucketCommand();
            var config = new BucketConfiguration() {
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }

        [Test]
        public async Task TestCommandSynchronizeLocalFolderFromBucket()
        {

            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new SynchronizeLocalFolderFromBucketCommand();
            var config = new BucketConfiguration() {
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }

        [Test]
        public async Task TestCommandUploadBucket()
        {

            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new UploadBucketCommand();
            var config = new BucketConfiguration() {
                LocalPathFiles = new List<string>(),
                LocalPathFolders = new List<string>(),
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }

        [Test]
        public async Task TestCommandDownloadBucket()
        {

            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new DownloadBucketCommand();
            var config = new BucketConfiguration() {
                RemoteRelativePathFiles = new List<string>(),
                RemoteRelativePathFolders = new List<string>(),
                LocalPathGet = "a",
                RemoteRelativePath = "b",
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }

        [Test]
        public async Task TestCommandRemoveBucketEntity()
        {
            var bucket = Builder<FakeBucket>.CreateNew().Build();
            CancellationToken ct = default(CancellationToken);
            var command = new RemoveEntityBucketCommand();
            var config = new BucketConfiguration()
            {
                LocalPathGet = "a",
                RemoteRelativePaths = new List<string>{"b"},
                DeleteFiles = true,
            };
            CommandValues.GenericInfoCommandValue ret = await command.ExecuteAsync(bucket, config, ct);

            Assert.IsTrue(ret is CommandValues.GenericInfoCommandValue);
            Assert.AreEqual(ret.Uuid, "Shortname1");
        }
    }
}