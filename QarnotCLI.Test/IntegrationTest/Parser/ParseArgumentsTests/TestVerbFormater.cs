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
    public class TestVerbFormater
    {
        [SetUp]
        public void Init()
        {
            CLILogsTest.SurchargeLogs();
            VerbForm = new VerbFormater();
        }

        public IVerbFormater VerbForm { get; set; }

        [Test]
        public void VerifyAllTheBucketPrefix()
        {
            Assert.AreEqual("bucket create", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "create" })[0]);
            Assert.AreEqual("bucket set", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "set" })[0]);
            Assert.AreEqual("bucket sync-from", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "sync-from" })[0]);
            Assert.AreEqual("bucket sync-to", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "sync-to" })[0]);
            Assert.AreEqual("bucket list", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "list" })[0]);
            Assert.AreEqual("bucket delete", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "delete" })[0]);
        }

        [Test]
        public void VerifyAllTheClassicPrefix()
        {
            Assert.AreEqual("task create", VerbForm.ConcatSubverbArgv(new string[] { "task", "create" })[0]);
            Assert.AreEqual("pool list", VerbForm.ConcatSubverbArgv(new string[] { "pool", "list" })[0]);
            Assert.AreEqual("job info", VerbForm.ConcatSubverbArgv(new string[] { "job", "info" })[0]);
            Assert.AreEqual("task wait", VerbForm.ConcatSubverbArgv(new string[] { "task", "wait" })[0]);
            Assert.AreEqual("pool abort", VerbForm.ConcatSubverbArgv(new string[] { "pool", "abort" })[0]);
            Assert.AreEqual("job delete", VerbForm.ConcatSubverbArgv(new string[] { "job", "delete" })[0]);
        }

        [Test]
        public void VerifyThatAnUnknownSubCommandIsNotJoin()
        {
            Assert.AreEqual("notFound", VerbForm.ConcatSubverbArgv(new string[] { "bucket", "notFound" })[0]);
            Assert.AreEqual("delete", VerbForm.ConcatSubverbArgv(new string[] { "classicName", "delete" })[0]);
        }
    }
}