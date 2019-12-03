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
    using static CommandValues;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestLauncherFactory
    {
        public LauncherFactory Factory { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            this.Factory = new LauncherFactory(null, null);
            CLILogsTest.SurchargeLogs();
        }

        [Test]
        public void PoolCreate()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.Create) is CreateCommandLauncher);
        }

        [Test]
        public void PoolInfo()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.Info) is CommandGeneric<QPool, QPool>);
        }

        [Test]
        public void PoolList()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.List) is CommandGeneric<QPool, PoolCommandValue>);
        }

        [Test]
        public void PoolDelete()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.Delete) is CommandGeneric<QPool, GenericInfoCommandValue>);
        }

        [Test]
        public void PoolSet()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.Set) is CommandGeneric<QPool, GenericInfoCommandValue>);
        }

        [Test]
        public void PoolInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Pool, CommandApi.Abort));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void TaskCreate()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Create) is CreateCommandLauncher);
        }

        [Test]
        public void TaskInfo()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Info) is CommandGeneric<QTask, QTask>);
        }

        [Test]
        public void TaskList()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.List) is CommandGeneric<QTask, TaskCommandValue>);
        }

        [Test]
        public void TaskWait()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Wait) is CommandGeneric<QTask, GenericInfoCommandValue>);
        }

        [Test]
        public void TaskAbort()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Abort) is CommandGeneric<QTask, GenericInfoCommandValue>);
        }

        [Test]
        public void TaskDelete()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Delete) is CommandGeneric<QTask, GenericInfoCommandValue>);
        }

        [Test]
        public void TaskInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Task, CommandApi.Set));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void JobCreate()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Job, CommandApi.Create) is CreateCommandLauncher);
        }

        [Test]
        public void JobInfo()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Job, CommandApi.Info) is CommandGeneric<QJob, QJob>);
        }

        [Test]
        public void JobList()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Job, CommandApi.List) is CommandGeneric<QJob, JobCommandValue>);
        }

        [Test]
        public void JobAbort()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Job, CommandApi.Abort) is CommandGeneric<QJob, GenericInfoCommandValue>);
        }

        [Test]
        public void JobDelete()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Job, CommandApi.Delete) is CommandGeneric<QJob, GenericInfoCommandValue>);
        }

        [Test]
        public void JobInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Job, CommandApi.Set));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void BucketCreate()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.Create) is CreateCommandLauncher);
        }

        [Test]
        public void BucketList()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.List) is CommandGeneric<QBucket, BucketCommandValue>);
        }

        [Test]
        public void BucketInfoBucket()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.InfoBucket) is CommandGenericOneElem<QBucket, BucketInfoCommandValue>);
        }

        [Test]
        public void BucketDelete()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.Delete) is CommandGeneric<QBucket, GenericInfoCommandValue>);
        }

        [Test]
        public void BucketSyncFrom()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.SyncFrom) is CommandGeneric<QBucket, GenericInfoCommandValue>);
        }

        [Test]
        public void BucketSyncTo()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.SyncTo) is CommandGeneric<QBucket, GenericInfoCommandValue>);
        }

        [Test]
        public void BucketUpload()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.Upload) is CommandGeneric<QBucket, GenericInfoCommandValue>);
        }

        [Test]
        public void BucketDownload()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.Download) is CommandGeneric<QBucket, GenericInfoCommandValue>);
        }

        [Test]
        public void BucketInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Bucket, CommandApi.Set));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void AllList()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.All, CommandApi.List) is CommandAll);
        }

        [Test]
        public void AllAbort()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.All, CommandApi.Abort) is CommandAll);
        }

        [Test]
        public void AllDelete()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.All, CommandApi.Delete) is CommandAll);
        }

        [Test]
        public void AllInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.All, CommandApi.Set));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void ConfigSetCantCreateALauncher()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Config, CommandApi.Set));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void ConfigInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Config, CommandApi.List));
            Assert.IsNotNull(ex);
        }

        [Test]
        public void AccountSet()
        {
            Assert.IsTrue(this.Factory.CreateLauncher(ConfigType.Account, CommandApi.Info) is CommandGenericOneElem<UserInformation, UserInformationCommandValue>);
        }

        [Test]
        public void AccountInvalidThrowError()
        {
            var ex = Assert.Throws<NotImplementedException>(() => this.Factory.CreateLauncher(ConfigType.Account, CommandApi.List));
            Assert.IsNotNull(ex);
        }
    }
}