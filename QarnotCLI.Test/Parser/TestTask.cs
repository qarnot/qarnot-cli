using Moq;
using NUnit.Framework;
using QarnotSDK;

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
        var projectUuid = Guid.NewGuid();
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
                "--project-uuid", projectUuid.ToString(), "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
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
            model.ProjectUuid == projectUuid &&
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

    [Test]
    public async Task CreateTaskWithDependsOn()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var uuid2 = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask",
                "--profile", "docker-batch",
                "--instance", "1",
                "--depends-on", uuid1, uuid2,
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.DependsOn.Count == 2 &&
            model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
            model.DependsOn[0].TaskFinalStateCondition == null &&
            model.DependsOn[1].TaskUuid == Guid.Parse(uuid2) &&
            model.DependsOn[1].TaskFinalStateCondition == null
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskWithDependsOnFromJson()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var uuid2 = Guid.NewGuid().ToString();
        var jsonFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(jsonFile, $$"""
                {
                  "Name": "MyTask",
                  "Profile": "docker-batch",
                  "InstanceCount": 1,
                  "DependsOn": [{"TaskUuid": "{{uuid1}}"}, {"TaskUuid": "{{uuid2}}"}]
                }
                """);

            var res = await mock.Parser.InvokeAsync(
                new[] { "task", "create", "--file", jsonFile }
            );

            Assert.That(res, Is.EqualTo(0), "parsing should succeed");

            mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
                model.DependsOn.Count == 2 &&
                model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
                model.DependsOn[0].TaskFinalStateCondition == null &&
                model.DependsOn[1].TaskUuid == Guid.Parse(uuid2) &&
                model.DependsOn[1].TaskFinalStateCondition == null
            )), Times.Once);
        }
        finally
        {
            File.Delete(jsonFile);
        }
    }

    // Tests for advaced dependencies UUID:Condition syntax

    [Test]
    public async Task CreateTaskWithAdvancedDependsOn()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var uuid2 = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask",
                "--profile", "docker-batch",
                "--instance", "1",
                "--depends-on", $"{uuid1}:Failure,Cancelled", $"{uuid2}:Success",
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.DependsOn.Count == 2 &&
            model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
            model.DependsOn[0].TaskFinalStateCondition != null &&
            model.DependsOn[0].TaskFinalStateCondition!.Count() == 2 &&
            model.DependsOn[0].TaskFinalStateCondition!.Contains(TaskFinalState.Failure) &&
            model.DependsOn[0].TaskFinalStateCondition!.Contains(TaskFinalState.Cancelled) &&
            model.DependsOn[1].TaskUuid == Guid.Parse(uuid2) &&
            model.DependsOn[1].TaskFinalStateCondition != null &&
            model.DependsOn[1].TaskFinalStateCondition!.Contains(TaskFinalState.Success)
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskWithMixedDependsOn()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var uuid2 = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask",
                "--profile", "docker-batch",
                "--instance", "1",
                "--depends-on", $"{uuid1}:Failure", uuid2,
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.DependsOn.Count == 2 &&
            model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
            model.DependsOn[0].TaskFinalStateCondition != null &&
            model.DependsOn[0].TaskFinalStateCondition!.Count() == 1 &&
            model.DependsOn[0].TaskFinalStateCondition!.Contains(TaskFinalState.Failure) &&
            model.DependsOn[1].TaskUuid == Guid.Parse(uuid2) &&
            model.DependsOn[1].TaskFinalStateCondition == null
        )), Times.Once);
    }


    [Test]
    public async Task CreateTaskWithAdvancedDependsOnFromJson()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var jsonFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(jsonFile, $$"""
                {
                  "Name": "MyTask",
                  "Profile": "docker-batch",
                  "InstanceCount": 1,
                  "DependsOn": [{"TaskUuid": "{{uuid1}}", "TaskFinalStateCondition": ["Failure", "Cancelled"]}]
                }
                """);

            var res = await mock.Parser.InvokeAsync(
                new[] { "task", "create", "--file", jsonFile }
            );

            Assert.That(res, Is.EqualTo(0), "parsing should succeed");

            mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
                model.DependsOn.Count == 1 &&
                model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
                model.DependsOn[0].TaskFinalStateCondition != null &&
                model.DependsOn[0].TaskFinalStateCondition!.Count() == 2 &&
                model.DependsOn[0].TaskFinalStateCondition!.Contains(TaskFinalState.Failure) &&
                model.DependsOn[0].TaskFinalStateCondition!.Contains(TaskFinalState.Cancelled)
            )), Times.Once);
        }
        finally
        {
            File.Delete(jsonFile);
        }
    }

    [Test]
    public async Task CreateTaskWithMixedAdvancedDependsOnFromJson()
    {
        var mock = new MockParser();

        var uuid1 = Guid.NewGuid().ToString();
        var uuid2 = Guid.NewGuid().ToString();
        var jsonFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(jsonFile, $$"""
                {
                  "Name": "MyTask",
                  "Profile": "docker-batch",
                  "InstanceCount": 1,
                  "DependsOn": [
                    {"TaskUuid": "{{uuid1}}"},
                    {"TaskUuid": "{{uuid2}}", "TaskFinalStateCondition": ["Success"]}
                  ]
                }
                """);

            var res = await mock.Parser.InvokeAsync(
                new[] { "task", "create", "--file", jsonFile }
            );

            Assert.That(res, Is.EqualTo(0), "parsing should succeed");

            mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
                model.DependsOn.Count == 2 &&
                model.DependsOn[0].TaskUuid == Guid.Parse(uuid1) &&
                model.DependsOn[0].TaskFinalStateCondition == null &&
                model.DependsOn[1].TaskUuid == Guid.Parse(uuid2) &&
                model.DependsOn[1].TaskFinalStateCondition != null &&
                model.DependsOn[1].TaskFinalStateCondition!.Count() == 1 &&
                model.DependsOn[1].TaskFinalStateCondition!.Contains(TaskFinalState.Success)
            )), Times.Once);
        }
        finally
        {
            File.Delete(jsonFile);
        }
    }

    [Test]
    public async Task DependenciesStateTask()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "task", "dependencies-state", "--id", uuid }
        );

        mock.TaskUseCases.Verify(useCases => useCases.DependenciesState(It.Is<GetPoolsOrTasksModel>(model =>
            model.Id == uuid
        )), Times.Once);
    }
}


// A few tests checking what happens if we give bad DependsOn arguments. We want to fail with a human-readable
// hint as to why.
// NOTE: the failure mode is not great, we display an exception to the end user, which is why here we just look at
// NOTE: the exception message. That is not the greatest UI, but it's not specific to dependencies and we'll live
// NOTE: it for the time being.
[TestFixture]
public class TestAdvancedDependencyValidation
{
    [Test]
    // AMEND: make this test more "out"
    public void ParseAdvancedDependency_InvalidUuid_ThrowsWithHelpfulMessage()
    {
        var ex = Assert.Throws<Exception>(() => Helpers.ParseAdvancedDependency("not-a-uuid"));

        Assert.That(ex!.Message, Does.Contain("not-a-uuid"),
            "message should quote the bad value");
        Assert.That(ex!.Message, Does.Contain("UUID"),
            "message should mention UUID so the user understands what was expected");
    }

    [Test]
    public void ParseAdvancedDependency_InvalidState_ThrowsWithHelpfulMessage()
    {
        var uuid = Guid.NewGuid().ToString();
        var ex = Assert.Throws<Exception>(() => Helpers.ParseAdvancedDependency($"{uuid}:BadCondition"));

        Assert.That(ex!.Message, Does.Contain("BadCondition"),
            "message should quote the bad value");
        Assert.That(ex!.Message, Does.Contain("Success"),
            "message should list the accepted values");
        Assert.That(ex!.Message, Does.Contain("Failure"),
            "message should list the accepted values");
        Assert.That(ex!.Message, Does.Contain("Cancelled"),
            "message should list the accepted values");
    }

    // These verify that invalid input causes a non-zero exit code (not a silent success).

    [Test]
    public async Task CreateTask_DependsOn_InvalidUuid_FailsWithError()
    {
        var mock = new MockParser();

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask", "--profile", "docker-batch", "--instance", "1",
                "--depends-on", "not-a-uuid",
            }
        );

        Assert.That(res, Is.Not.EqualTo(0), "parsing should fail when --depends-on value is not a valid UUID");
    }

    [Test]
    public async Task CreateTask_DependsOn_InvalidState_FailsWithError()
    {
        var mock = new MockParser();
        var uuid = Guid.NewGuid().ToString();

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask", "--profile", "docker-batch", "--instance", "1",
                "--depends-on", $"{uuid}:BadCondition",
            }
        );

        Assert.That(res, Is.Not.EqualTo(0), "parsing should fail when --depends-on condition is not a valid value");
    }

    // --- Integration tests via JSON file ---

    [Test]
    public async Task CreateTask_JsonFile_DependsOn_InvalidUuid_FailsWithError()
    {
        var mock = new MockParser();
        var jsonFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(jsonFile, """
                {
                  "Name": "MyTask",
                  "Profile": "docker-batch",
                  "InstanceCount": 1,
                  "DependsOn": [{"TaskUuid": "not-a-uuid"}]
                }
                """);

            var res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--file", jsonFile });

            Assert.That(res, Is.Not.EqualTo(0), "parsing should fail when DependsOn contains a non-UUID TaskUuid");
        }
        finally
        {
            File.Delete(jsonFile);
        }
    }

    [Test]
    public async Task CreateTask_JsonFile_DependsOn_InvalidState_FailsWithError()
    {
        var mock = new MockParser();
        var uuid = Guid.NewGuid().ToString();
        var jsonFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(jsonFile, $$"""
                {
                  "Name": "MyTask",
                  "Profile": "docker-batch",
                  "InstanceCount": 1,
                  "DependsOn": [{"TaskUuid": "{{uuid}}", "TaskFinalStateCondition": ["BadCondition"]}]
                }
                """);

            var res = await mock.Parser.InvokeAsync(new[] { "task", "create", "--file", jsonFile });

            Assert.That(res, Is.Not.EqualTo(0), "parsing should fail when DependsOn condition is not a valid value");
        }
        finally
        {
            File.Delete(jsonFile);
        }
    }
}


// A few tests checking what happens if we give invalid arguments. We want to fail with a human-readable
// hint as to why.
[TestFixture]
public class TestTaskInvalidArguments
{

    [Test]
    public async Task CreateTaskListOptionsAcceptMultipleTokens()
    {
        var mock = new MockParser();

        var tags = new[] { "TAG1", "TAG2", "TAG3" };
        var constants = new[] { "KEY1=VAL1", "KEY2=VAL2" };
        var resources = new[] { "bucket-a", "bucket-b" };

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "task", "create",
                "--name", "MyTask", "--instance", "1", "--profile", "docker-batch",
                "--tags", tags[0], tags[1], tags[2],
                "--constants", constants[0], constants[1],
                "--resources", resources[0], resources[1],
            }
        );

        Assert.That(res, Is.EqualTo(0), "multiple tokens per list option should succeed");

        mock.TaskUseCases.Verify(useCases => useCases.Create(It.Is<CreateTaskModel>(model =>
            model.Tags.SequenceEqual(tags) &&
            model.Constants.SequenceEqual(constants) &&
            model.Resources.SequenceEqual(resources)
        )), Times.Once);
    }

    [Test]
    public async Task CreateTaskInvalidArgAfterListOptionIsRejected()
    {
        try
        {
            using var sw = new StringWriter();
            Console.SetError(sw);

            var mock = new MockParser();

            // --invalid-arg after a list option (--tags) must be rejected, not silently consumed as a tag value
            var res = await mock.Parser.InvokeAsync(
                new[] {
                    "task", "create",
                    "--name", "MyTask", "--instance", "1", "--profile", "docker-batch",
                    "--tags", "TAG1", "--invalid-arg",
                }
            );

            Assert.That(res, Is.Not.EqualTo(0), "--invalid-arg after a list option must be rejected");
            mock.TaskUseCases.Verify(useCases => useCases.Create(It.IsAny<CreateTaskModel>()), Times.Never);

            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument '--invalid-arg'"));
        }
        finally 
        {
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
        }
    }

    [Test]
    public async Task CreateTaskMultipleInvalidArgsAfterListOptionAreAllRejected()
    {
        try
        {
            using var sw = new StringWriter();
            Console.SetError(sw);

            var mock = new MockParser();

            // reproduces the exact scenario from the bug report:
            // task create -n Task -i 1 --profile docker-batch -t tag1 --invalid-argument --invalid-argument-2 invalid-value2 --shortname my-task --invalid-argument-3 invalid-value-3
            var res = await mock.Parser.InvokeAsync(
                new[] {
                    "task", "create",
                    "-n", "Task", "-i", "1", "--profile", "docker-batch",
                    "-t", "tag1", "--invalid-argument", "--invalid-argument-2", "invalid-value-2",
                    "--shortname", "my-task", "--invalid-argument-3", "invalid-value-3",
                }
            );

            Assert.That(res, Is.Not.EqualTo(0), "invalid arguments after list option must all be rejected");
            mock.TaskUseCases.Verify(useCases => useCases.Create(It.IsAny<CreateTaskModel>()), Times.Never);

            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument '--invalid-argument'"));
            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument '--invalid-argument-2'"));
            // 'invalid-value2' doesn't start with '-', so it is silently consumed as a tag value — unavoidable
            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument '--invalid-argument-3'"));
            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument 'invalid-value-3'"));
            Assert.That(sw.ToString(), Does.Not.Contain("Unrecognized command or argument '--shortname'"));
        }
        finally 
        {
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
        }
    }

    [Test]
    public async Task CreateTaskFlagLikeValueInListOptionIsRejected() // BREAKING CHANGE
    {
        try
        {
            using var sw = new StringWriter();
            Console.SetError(sw);

            var mock = new MockParser();

            // a value that starts with '-' but is not a known option should be rejected, not silently stored as a tag
            var res = await mock.Parser.InvokeAsync(
                new[] {
                    "task", "create",
                    "--name", "MyTask", "--instance", "1", "--profile", "docker-batch",
                    "--constants", "-not-a-flag",
                }
            );

            Assert.That(res, Is.Not.EqualTo(0), "a value starting with '-' in a list option must be rejected");
            mock.TaskUseCases.Verify(useCases => useCases.Create(It.IsAny<CreateTaskModel>()), Times.Never);
            Assert.That(sw.ToString(), Does.Contain("Unrecognized command or argument '-not-a-flag'"));
        }
        finally 
        {
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
        }
    }
}
