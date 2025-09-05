using Microsoft.VisualBasic;
using Moq;
using NUnit.Framework;
using QarnotSDK;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestTaskCommand
{
    [Test]
    public async Task CreateTask()
    {
        var mock = new MockParser();

        var name1 = "NAME1";
        var shortname = "SHORT";
        var range = "1-5";
        var instance = 42;
        var profile = "PROFILE";
        var tags = new[] { "TAG1", "TAG2", "TAG3" };
        var constants = new[] { "CONSTANT" };
        var constraints = new[] { "CONSTRAINTS" };
        var periodic = 5;
        var whitelist = "white*";
        var blacklist = "black*";
        var maxRetriesPerInstance = 23;
        var maxTimeQueueSeconds = 10;
        var maxTotalRetries = 24;
        var defaultTTL = 36000;
        var resultTTL = 12345;
        var reservedMachine = "some-reserved-machine";
        var reservationName = "some-reservation-name";

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create", "--name", name1, "--shortname", shortname, "--instance", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--wait-for-resources-synchronization", "true" , "--periodic", periodic.ToString(), "--whitelist",  whitelist,
                "--max-time-queue", maxTimeQueueSeconds.ToString(), 
                "--blacklist", blacklist, "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString()
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.Name == name1 &&
            model.ShortName == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            model.Constraints.Zip(constraints).All(pair => pair.First == pair.Second) &&
            model.Instance == instance &&
            model.WaitForResourcesSynchronization &&
            model.Periodic == periodic &&
            model.Whitelist == whitelist &&
            model.Blacklist == blacklist &&
            model.MaxRetriesPerInstance == maxRetriesPerInstance &&
            model.MaxTimeQueueSeconds == maxTimeQueueSeconds &&
            model.MaxTotalRetries == maxTotalRetries &&
            model.ExportCredentialsToEnv == null &&
            model.Ttl == null &&
            model.ResultTtl == null &&
            model.SchedulingType == null &&
            model.MachineTarget == null &&
            model.ReservationTarget == null
        )), Times.Once);

        var name2 = "NAME2";
        res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create", "--name", name2, "--shortname", shortname, "--range", range, "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--wait-for-resources-synchronization", "false" ,
                "--periodic", periodic.ToString(), "--whitelist",  whitelist, "--blacklist", blacklist, "--max-retries-per-instance", maxRetriesPerInstance.ToString(),
                "--max-time-queue", maxTimeQueueSeconds.ToString(),
                "--max-total-retries", maxTotalRetries.ToString(), "--export-credentials-to-env", "true", "--ttl", defaultTTL.ToString(),
                "--result-ttl", resultTTL.ToString(), "--scheduling-type", "Flex", "--machine-target", reservedMachine, "--reservation-target", reservationName
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.Name == name2 &&
            model.ShortName == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            !model.Constraints.Any() &&
            model.Range == range &&
            !model.WaitForResourcesSynchronization &&
            model.Periodic == periodic &&
            model.Whitelist == whitelist &&
            model.Blacklist == blacklist &&
            model.MaxRetriesPerInstance == maxRetriesPerInstance &&
            model.MaxTimeQueueSeconds == maxTimeQueueSeconds &&
            model.MaxTotalRetries == maxTotalRetries &&
            model.ExportCredentialsToEnv == true &&
            model.Ttl == defaultTTL &&
            model.ResultTtl == resultTTL &&
            model.SchedulingType == "Flex" &&
            model.MachineTarget == reservedMachine &&
            model.ReservationTarget == reservationName
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskWithHardwareConstraints()
    {
        var mock = new MockParser();

        var name1 = "NAME1";
        var shortname = "SHORT";
        var instance = 42;
        var profile = "PROFILE";
        var tags = new[] { "TAG1", "TAG2", "TAG3" };
        var constants = new[] { "CONSTANT" };
        var constraints = new[] { "CONSTRAINTS" };
        var periodic = 5;
        var whitelist = "white*";
        var blacklist = "black*";
        var maxRetriesPerInstance = 23;
        var maxTotalRetries = 24;
        var hardwareConstraints = new HardwareConstraints()
        {
            new MinimumCoreHardware(2),
            new MaximumCoreHardware(4),
            new MinimumRamCoreRatioHardware(0.1m),
            new MaximumRamCoreRatioHardware(1.5m),
            new SpecificHardware("the-hardware-key"),
            new MinimumRamHardware(0.2m),
            new MaximumRamHardware(3.1m),
            new GpuHardware(),
            new CpuModelHardware("the-cpu-model"),
        };

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create", "--name", name1, "--shortname", shortname, "--instance", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--wait-for-resources-synchronization", "true" , "--periodic", periodic.ToString(), "--whitelist",  whitelist,

                "--min-core-count", "2", "--max-core-count", "4",
                "--min-ram-core-ratio", "0.1", "--max-ram-core-ratio", "1.5",
                "--specific-hardware-constraints", "the-hardware-key",
                "--cpu-model", "the-cpu-model", "--gpu-hardware", "--max-ram" , "3.1", "--min-ram", "0.2",

                "--blacklist", blacklist, "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString()
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.Name == name1 &&
            model.ShortName == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            model.Constraints.Zip(constraints).All(pair => pair.First == pair.Second) &&
            model.Instance == instance &&
            model.WaitForResourcesSynchronization &&
            model.Periodic == periodic &&
            model.Whitelist == whitelist &&
            model.Blacklist == blacklist &&
            model.MaxRetriesPerInstance == maxRetriesPerInstance &&
            model.MaxTotalRetries == maxTotalRetries &&
            model.ExportCredentialsToEnv == null &&
            model.Ttl == null &&
            model.ResultTtl == null &&
            model.SchedulingType == null &&
            model.MachineTarget == null &&
            model.ReservationTarget == null &&
            model.HardwareConstraints != null &&
            model.HardwareConstraints.Count == hardwareConstraints.Count &&
            hardwareConstraints.All(constraint => model.HardwareConstraints.Contains(constraint))
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskWithBothSsdAndNoSsd()
    {
        try
        {
            using var sw = new StringWriter();
            Console.SetError(sw);
            var mock = new MockParser();

            var name1 = "NAME1";
            var shortname = "SHORT";
            var instance = 42;
            var profile = "PROFILE";
            var tags = new[] { "TAG1", "TAG2", "TAG3" };
            var constants = new[] { "CONSTANT" };
            var constraints = new[] { "CONSTRAINTS" };
            var periodic = 5;
            var whitelist = "white*";
            var blacklist = "black*";
            var maxRetriesPerInstance = 23;
            var maxTotalRetries = 24;

            var res = await mock.Parser.InvokeAsync(
                new[] {
                "task", "create", "--name", name1, "--shortname", shortname, "--instance", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--wait-for-resources-synchronization", "true" , "--periodic", periodic.ToString(), "--whitelist",  whitelist,

                "--ssd-hardware", "--no-ssd-hardware",

                "--blacklist", blacklist, "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString()
                }
            );

            Assert.That(res, Is.EqualTo(1), "parsing should fail");


            Assert.That(sw.ToString(), Does.Contain("--ssd-hardware and --no-ssd-hardware are mutually exclusive."));
        }
        finally 
        {
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
        }
    }

    [Test]
    public async Task ListTasks()
    {
        var mock = new MockParser();

        await mock.Parser.InvokeAsync(new[] { "task", "list" });
        mock.TaskUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == null &&
            model.Id == null &&
            !model.Tags.Any() &&
            !model.ExclusiveTags.Any() &&
            !model.NoPaginate
        )), Times.Once);


        var name = "NAME1";
        var shortname = "shortname1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        await mock.Parser.InvokeAsync(
            new[] { "task", "list", "--name", name, "--shortname", shortname, "--id", uuid, "--tags", tags[0], tags[1] }
        );

        mock.TaskUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Shortname == shortname &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task ListTasksPage()
    {
        var mock = new MockParser();

        await mock.Parser.InvokeAsync(new[] { "task", "list", "--no-paginate" });
        mock.TaskUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == null &&
            model.Id == null &&
            !model.Tags.Any() &&
            !model.ExclusiveTags.Any() &&
            model.NoPaginate
        )), Times.Once);

        var token = "zefabuloustoken";
        await mock.Parser.InvokeAsync(new[] { "task", "list", "--next-page-token", token });
        mock
            .TaskUseCases
            .Verify(
                useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model => model.NextPageToken == token)),
                Times.Once);

        var maxPageSize = 42;
        var namePrefix = "zeprefix";
        var createdBefore = "01-01-2022";
        var createdAfter = "02-01-2022";
        await mock.Parser.InvokeAsync(new[] { "task", "list", "--max-page-size", $"{maxPageSize}", "--next-page", "--name-prefix", namePrefix, "--created-before", createdBefore, "--created-after", createdAfter });
        mock
            .TaskUseCases
            .Verify(
                useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
                    model.MaxPageSize == maxPageSize
                    && model.NextPage
                    && model.NamePrefix == namePrefix
                    && model.CreatedAfter == createdAfter
                    && model.CreatedBefore == createdBefore)),
                Times.Once);
    }

    [TestCase("list")]
    [TestCase("info")]
    [TestCase("wait")]
    [TestCase("abort")]
    [TestCase("update-resources")]
    [TestCase("delete")]
    [TestCase("stdout")]
    [TestCase("stderr")]
    public async Task CantHaveTagsAndExclusiveTags(string subcommand)
    {
        var mock = new MockParser();

        var name = "NAME";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", subcommand, "-i", uuid, "-n", name, "-t", tags[0], tags[1],
                "--exclusive-tags", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.Not.EqualTo(0), "can't have both --tags and --exclusive-tags");

        res = await mock.Parser.InvokeAsync(
            new[] {
                "task", subcommand, "-i", uuid, "-n", name, "-t", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.EqualTo(0), "should be able to have only --tags");

        res = await mock.Parser.InvokeAsync(
            new[] {
                "task", subcommand, "-i", uuid, "-n", name, "--exclusive-tags", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.EqualTo(0), "shold be able to have only --exclusive-tags");
    }

    [Test]
    public async Task TaskInfo()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        await mock.Parser.InvokeAsync(
            new[] { "task", "info", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Info(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task TaskStdout()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(
            new[] { "task", "stdout", "--name", name, "--id", uuid, "--tags", tags[0], tags[1], "--instance-id", "0", "--fresh" }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Stdout(It.Is<GetTasksOutputModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.InstanceId == 0 &&
            model.Fresh
        )), Times.Once);
    }

    [Test]
    public async Task TaskStderr()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(
            new[] { "task", "stderr", "--name", name, "--id", uuid, "--tags", tags[0], tags[1], "--instance-id", "0", "--fresh" }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Stderr(It.Is<GetTasksOutputModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.InstanceId == 0 &&
            model.Fresh
        )), Times.Once);
    }

    [Test]
    public async Task UpdateTaskResources()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        await mock.Parser.InvokeAsync(
            new[] { "task", "update-resources", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] }
        );

        mock.TaskUseCases.Verify(useCases => useCases.UpdateResources(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task WaitTask()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(
            new[] { "task", "wait", "--name", name, "--id", uuid, "--tags", tags[0], tags[1], "--stdout", "--stderr" }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Wait(It.Is<WaitTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Stdout &&
            model.Stderr
        )), Times.Once);
    }

    [Test]
    public async Task AbortTask()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        await mock.Parser.InvokeAsync(
            new[] { "task", "abort", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Abort(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task DeleteTask()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        await mock.Parser.InvokeAsync(
            new[] { "task", "delete", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Delete(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task SnapshotTask()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        var periodic = 5;
        var whitelist = "white*";
        var blacklist = "black*";
        var bucket = "snapshotbucket";

        await mock.Parser.InvokeAsync(
            new[] {
                "task", "snapshot", "--name", name, "--id", uuid, "--tags", tags[0], tags[1], "--periodic", periodic.ToString(),
                "--whitelist", whitelist, "--blacklist", blacklist, "--bucket", bucket
            }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Snapshot(It.Is<SnapshotTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Periodic == periodic &&
            model.Whitelist == whitelist &&
            model.Blacklist == blacklist &&
            model.Bucket == bucket
        ), true), Times.Once);
    }

    [Test]
    public async Task SnapshotCreateTask()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        var periodic = 5;
        var whitelist = "white*";
        var blacklist = "black*";
        var bucket = "snapshotbucket";

        await mock.Parser.InvokeAsync(
            new[] {
                "task", "snapshot", "create", "--name", name, "--id", uuid, "--tags", tags[0], tags[1], "--periodic", periodic.ToString(),
                "--whitelist", whitelist, "--blacklist", blacklist, "--bucket", bucket
            }
        );

        mock.TaskUseCases.Verify(useCases => useCases.Snapshot(It.Is<SnapshotTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Periodic == periodic &&
            model.Whitelist == whitelist &&
            model.Blacklist == blacklist &&
            model.Bucket == bucket
        ), false), Times.Once);
    }

    [Test]
    public async Task UpdateTaskConstant()
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        var constantName = "SOME_CONSTANT";
        var constantValue = "some-new-value";

        await mock.Parser.InvokeAsync(
            new[] {
                "task", "update-constant", "--name", name, "--id", uuid, "--tags", tags[0], tags[1],
                "--constant-name",  constantName, "--constant-value", constantValue
            }
        );

        mock.TaskUseCases.Verify(useCases => useCases.UpdateConstant(It.Is<UpdatePoolsOrTasksConstantModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.ConstantName == constantName &&
            model.ConstantValue == constantValue
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskRequiresNameInstanceAndProfile()
    {
        var name = "NAME";
        var instance = "42";
        var profile = "PROFILE";

        var mock = new MockParser();

        var res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--name", name, "--instance", instance });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing profile");

        res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--name", name, "--profile", profile });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing instance count");

        res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--profile", profile, "--instance", instance });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing name");
    }

    [Test]
    public async Task CreateTaskCantHaveBothRangeAndInstanceCount()
    {
        var mock = new MockParser();
        var res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--name", "name", "--profile", "profile", "--instance", "5", "--range", "2-5" });

        Assert.That(res, Is.Not.EqualTo(0));
    }

    [Test]
    public async Task SnapshotStatusTask()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var snapshotId = "snap_52c10b2d-0687-41e1-985e-7279f6dd543a_20251228234559";

        await mock.Parser.InvokeAsync(
            new[] { "task", "snapshot", "get", "--id", uuid, "--snapshot-id", snapshotId }
        );

        mock.TaskUseCases.Verify(useCases => useCases.SnapshotStatus(It.Is<GetSnapshotStatusModel>(model =>
            model.Id == uuid &&
            model.SnapshotId == snapshotId
        )), Times.Once);
    }

    [Test]
    public async Task SnapshotStatusTaskRequiresSnapshotId()
    {
        var mock = new MockParser();
        var uuid = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] { "task", "snapshot", "get", "--id", uuid }
        );

        Assert.That(res, Is.Not.EqualTo(0), "parsing should fail because --snapshot-id is required");
    }

    [Test]
    public async Task WaitSnapshotTask()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var snapshotId = "snap_52c10b2d-0687-41e1-985e-7279f6dd543a_20251228234559";
        var timeout = 120;
        var updateInterval = 5;

        await mock.Parser.InvokeAsync(
            new[] { "task", "snapshot", "wait", "--id", uuid, "--snapshot-id", snapshotId,
                    "--timeout", timeout.ToString(), "--update-interval", updateInterval.ToString() }
        );

        mock.TaskUseCases.Verify(useCases => useCases.WaitSnapshot(It.Is<WaitSnapshotModel>(model =>
            model.Id == uuid &&
            model.SnapshotId == snapshotId &&
            model.TimeoutSeconds == timeout &&
            model.UpdateIntervalSeconds == updateInterval
        )), Times.Once);
    }

    [Test]
    public async Task WaitSnapshotTaskWithDefaults()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var snapshotId = "snap_52c10b2d-0687-41e1-985e-7279f6dd543a_20251228234559";

        await mock.Parser.InvokeAsync(
            new[] { "task", "snapshot", "wait", "--id", uuid, "--snapshot-id", snapshotId }
        );

        mock.TaskUseCases.Verify(useCases => useCases.WaitSnapshot(It.Is<WaitSnapshotModel>(model =>
            model.Id == uuid &&
            model.SnapshotId == snapshotId &&
            model.TimeoutSeconds == -1 &&
            model.UpdateIntervalSeconds == 10
        )), Times.Once);
    }

    [Test]
    public async Task WaitSnapshotTaskRequiresSnapshotId()
    {
        var mock = new MockParser();
        var uuid = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] { "task", "snapshot", "wait", "--id", uuid }
        );

        Assert.That(res, Is.Not.EqualTo(0), "parsing should fail because --snapshot-id is required");
    }

    [TestCase("-d")]
    [TestCase("--datacenter")]
    public async Task GetTaskCarbonFacts(string datacenterOption)
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        var datacenterName = "SOME_DATACENTER";

        await mock.Parser.InvokeAsync(
            new[] {
                "task", "carbon-facts", "--name", name, "--id", uuid, "--tags", tags[0], tags[1],
                datacenterOption,  datacenterName
            }
        );

        mock.TaskUseCases.Verify(useCases => useCases.CarbonFacts(It.Is<GetCarbonFactsModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.EquivalentDataCenterName == datacenterName
        )), Times.Once);
    }
}
