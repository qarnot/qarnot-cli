using Moq;
using NUnit.Framework;
using QarnotSDK;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestPoolCommand
{
    [Test]
    public async Task CreatePool()
    {
        var mock = new MockParser();

        var name1 = "NAME1";
        var shortname = "SHORT";
        var instance = 42;
        var profile = "PROFILE";
        var tags = new[] { "TAG1", "TAG2", "TAG3" };
        var constants = new[] { "CONSTANT" };
        var constraints = new[] { "CONSTRAINTS" };
        var maxRetriesPerInstance = 23;
        var maxTotalRetries = 24;
        var defaultTTL = 36000;
        var reservedMachine = "some-reserved-machine";

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", "create", "--name", name1, "--shortname", shortname, "--instanceNodes", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--tasks-wait-for-synchronization", "true" , "--export-credentials-to-env", "true", "--ttl", defaultTTL.ToString(),
                "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString(),
                "--scheduling-type", "Flex", "--machine-target", reservedMachine
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.PoolUseCases.Verify(useCases => useCases.Create(It.Is<CreatePoolModel>(model =>
            model.Name == name1 &&
            model.Shortname == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            model.Constraints.Zip(constraints).All(pair => pair.First == pair.Second) &&
            model.InstanceCount == instance &&
            model.TasksWaitForSynchronization &&
            model.MaxRetriesPerInstance == maxRetriesPerInstance &&
            model.MaxTotalRetries == maxTotalRetries &&
            model.ExportCredentialsToEnv == true &&
            model.Ttl == defaultTTL &&
            model.SchedulingType == "Flex" &&
            model.MachineTarget == reservedMachine
        )), Times.Once);

        var name2 = "NAME2";
        res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", "create", "-n", name2, "-s", shortname, "-i", instance.ToString(), "-p", profile, "-t", tags[0], tags[1], tags[2], "-c", constants[0]
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.PoolUseCases.Verify(useCases => useCases.Create(It.Is<CreatePoolModel>(model =>
            model.Name == name2 &&
            model.Shortname == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            !model.Constraints.Any() &&
            model.InstanceCount == instance &&
            !model.TasksWaitForSynchronization &&
            model.MaxRetriesPerInstance == null &&
            model.MaxTotalRetries == null &&
            model.ExportCredentialsToEnv == null &&
            model.Ttl == null &&
            model.SchedulingType == null &&
            model.MachineTarget == null
        )), Times.Once);
    }

    [Test]
    public async Task CreatePoolWithHardwareConstraints()
    {
        var mock = new MockParser();

        var name1 = "NAME1";
        var shortname = "SHORT";
        var instance = 42;
        var profile = "PROFILE";
        var tags = new[] { "TAG1", "TAG2", "TAG3" };
        var constants = new[] { "CONSTANT" };
        var constraints = new[] { "CONSTRAINTS" };
        var maxRetriesPerInstance = 23;
        var maxTotalRetries = 24;
        var defaultTTL = 36000;
        var reservedMachine = "some-reserved-machine";
        var hardwareConstraints = new HardwareConstraints()
        {
            new MinimumCoreHardware(2),
            new MaximumCoreHardware(4),
            new MinimumRamCoreRatioHardware(1m),
            new MaximumRamCoreRatioHardware(1.5m),
            new SpecificHardware("the-hardware-key"),
            new MinimumRamHardware(0.2m),
            new MaximumRamHardware(3.1m),
            new GpuHardware(),
            new CpuModelHardware("the-cpu-model"),
        };


        var res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", "create", "--name", name1, "--shortname", shortname, "--instanceNodes", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--tasks-wait-for-synchronization", "true" , "--export-credentials-to-env", "true", "--ttl", defaultTTL.ToString(),

                "--min-core-count", "2", "--max-core-count", "4",
                "--min-ram-core-ratio", "1", "--max-ram-core-ratio", "1.5",
                "--specific-hardware-constraints", "the-hardware-key",

                "--cpu-model", "the-cpu-model", "--gpu-hardware", "--max-ram" , "3.1", "--min-ram", "0.2",
                "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString(),
                "--scheduling-type", "Flex", "--machine-target", reservedMachine
            }
        );

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.PoolUseCases.Verify(useCases => useCases.Create(It.Is<CreatePoolModel>(model =>
            model.Name == name1 &&
            model.Shortname == shortname &&
            model.Profile == profile &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.Constants.Zip(constants).All(pair => pair.First == pair.Second) &&
            model.Constraints.Zip(constraints).All(pair => pair.First == pair.Second) &&
            model.InstanceCount == instance &&
            model.TasksWaitForSynchronization &&
            model.MaxRetriesPerInstance == maxRetriesPerInstance &&
            model.MaxTotalRetries == maxTotalRetries &&
            model.ExportCredentialsToEnv == true &&
            model.Ttl == defaultTTL &&
            model.SchedulingType == "Flex" &&
            model.MachineTarget == reservedMachine &&
            model.HardwareConstraints != null &&
            model.HardwareConstraints.Count == hardwareConstraints.Count &&
            hardwareConstraints.All(constraint => model.HardwareConstraints.Contains(constraint))
        )), Times.Once);

    }


    [Test]
    public async Task CreatePoolWithBothSsdAndNoSsd()
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
            var maxRetriesPerInstance = 23;
            var maxTotalRetries = 24;
            var defaultTTL = 36000;
            var reservedMachine = "some-reserved-machine";


            var res = await mock.Parser.InvokeAsync(
                new[] {
                "pool", "create", "--name", name1, "--shortname", shortname, "--instanceNodes", instance.ToString(), "--profile", profile,
                "--tags", tags[0], tags[1], tags[2], "--constants", constants[0], "--constraints", constraints[0],
                "--tasks-wait-for-synchronization", "true" , "--export-credentials-to-env", "true", "--ttl", defaultTTL.ToString(),

                "--ssd-hardware", "--no-ssd-hardware",

                "--cpu-model", "the-cpu-model", "--gpu-hardware-constraint", "--max-ram" , "3.1", "--min-ram", "0.2",
                "--max-retries-per-instance", maxRetriesPerInstance.ToString(), "--max-total-retries", maxTotalRetries.ToString(),
                "--scheduling-type", "Flex", "--machine-target", reservedMachine
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
    public async Task CreatePoolWithElasticSettingsAndNoInstanceNodes()
    {
        var mock = new MockParser();

        var name = "NAME";
        var shortname = "SHORT";
        var minimumElasticSlots = 2;
        var profile = "PROFILE";

        await mock.Parser.InvokeAsync(new[] {
            "pool", "create", "--name", name, "--shortname", shortname, "--profile", profile, "--pool-is-elastic", "--min-slot", minimumElasticSlots.ToString()
        });

        mock.PoolUseCases.Verify(useCases => useCases.Create(It.Is<CreatePoolModel>(model =>
            model.Name == name &&
            model.Shortname == shortname &&
            model.ElasticMinSlots == minimumElasticSlots &&
            model.Profile == profile
        )), Times.Once);
    }

    [Test]
    public async Task CreatePoolWithScalingPoliciesShouldBeCorrectlyDeserialized([Values(true, false)] bool useJsonFile)
    {
        var mock = new MockParser();

        var name = "NAME";
        var shortname = "SHORT";
        var profile = "PROFILE";

        var fileName = "";
        if (useJsonFile)
        {
            fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, ScalingPolicy1);
        }

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", "create", "--name", name, "--shortname", shortname, "--profile", profile,
                "--scaling", useJsonFile ? $"@{fileName}" : ScalingPolicy1
            }
        );

        Assert.That(res, Is.EqualTo(0));

        var invocation = mock.PoolUseCases.Invocations.First(invocation => invocation.Method.Name == nameof(IPoolUseCases.Create));
        var model = invocation.Arguments.First(arg => arg.GetType() == typeof(CreatePoolModel)) as CreatePoolModel;
        Assert.That(model, Is.Not.Null, "Did not find a call to `PoolUseCases.Create`");

        Assert.That(model!.Scaling, Is.Not.Null);
        Assert.That(model!.Scaling!.Policies, Has.Count.EqualTo(2));

        var managedPolicy = model.Scaling!.Policies[0] as QarnotSDK.ManagedTasksQueueScalingPolicy;
        Assert.That(managedPolicy, Is.Not.Null);
        Assert.That(managedPolicy!.Name, Is.EqualTo("managed-policy"));
        Assert.That(managedPolicy.EnabledPeriods, Has.Count.EqualTo(2));

        var firstWeekly = managedPolicy.EnabledPeriods[0] as QarnotSDK.TimePeriodWeeklyRecurring;
        Assert.That(firstWeekly, Is.Not.Null);
        Assert.That(firstWeekly!.Name, Is.EqualTo("thursday-evening"));
        Assert.That(firstWeekly!.Days, Contains.Item(DayOfWeek.Thursday));
        Assert.That(firstWeekly.StartTimeUtc, Is.EqualTo("19:30:00"));
        Assert.That(firstWeekly.EndTimeUtc, Is.EqualTo("22:00:00"));
        Assert.That(managedPolicy.MinTotalSlots, Is.EqualTo(0));
        Assert.That(managedPolicy.MaxTotalSlots, Is.EqualTo(10));
        Assert.That(managedPolicy.MinIdleSlots, Is.EqualTo(1));
        Assert.That(managedPolicy.MinIdleTimeSeconds, Is.EqualTo(90));
        Assert.That(managedPolicy.ScalingFactor, Is.EqualTo(0.5).Within(0.01));

        var fixedPolicy = model.Scaling.Policies[1] as QarnotSDK.FixedScalingPolicy;
        Assert.That(fixedPolicy, Is.Not.Null);
        Assert.That(fixedPolicy!.Name, Is.EqualTo("fixed-policy"));
        Assert.That(fixedPolicy.SlotsCount, Is.EqualTo(4));

        Assert.That(fixedPolicy.EnabledPeriods, Has.Count.EqualTo(1));
        var alwaysPeriod = fixedPolicy.EnabledPeriods[0] as QarnotSDK.TimePeriodAlways;
        Assert.That(alwaysPeriod, Is.Not.Null);
        Assert.That(alwaysPeriod!.Name, Is.EqualTo("really-always"));

        if (useJsonFile)
        {
            File.Delete(fileName);
        }
    }

    [Test]
    public async Task UpdatePoolWithScalingPoliciesShouldBeCorrectlyDeserialized([Values(true, false)] bool useJsonFile)
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();

        var fileName = "";
        if (useJsonFile)
        {
            fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, ScalingPolicy2);
        }

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", "set-scaling", "--id", uuid, "--scaling", useJsonFile ? $"@{fileName}" : ScalingPolicy2
            }
        );

        Assert.That(res, Is.EqualTo(0));

        var invocation = mock.PoolUseCases.Invocations.First(invocation => invocation.Method.Name == nameof(IPoolUseCases.UpdateScaling));
        var model = invocation.Arguments.First(arg => arg.GetType() == typeof(UpdatePoolScalingModel)) as UpdatePoolScalingModel;
        Assert.That(model, Is.Not.Null, "Did not find a call to `PoolUseCases.UpdateScaling`");

        Assert.That(model!.Scaling, Is.Not.Null);
        Assert.That(model.Scaling!.Policies, Has.Count.EqualTo(2));

        var fixedPolicy = model.Scaling.Policies[0] as QarnotSDK.FixedScalingPolicy;
        Assert.That(fixedPolicy, Is.Not.Null);
        Assert.That(fixedPolicy!.Name, Is.EqualTo("fixed-policy"));
        Assert.That(fixedPolicy.SlotsCount, Is.EqualTo(4));

        Assert.That(fixedPolicy.EnabledPeriods, Has.Count.EqualTo(1));
        var alwaysPeriod = fixedPolicy.EnabledPeriods[0] as QarnotSDK.TimePeriodAlways;
        Assert.That(alwaysPeriod, Is.Not.Null);
        Assert.That(alwaysPeriod!.Name, Is.EqualTo("really-always"));


        var managedPolicy = model.Scaling!.Policies[1] as QarnotSDK.ManagedTasksQueueScalingPolicy;
        Assert.That(managedPolicy, Is.Not.Null);
        Assert.That(managedPolicy!.Name, Is.EqualTo("managed-policy"));
        Assert.That(managedPolicy.EnabledPeriods, Has.Count.EqualTo(1));

        var firstWeekly = managedPolicy.EnabledPeriods[0] as QarnotSDK.TimePeriodWeeklyRecurring;
        Assert.That(firstWeekly, Is.Not.Null);
        Assert.That(firstWeekly!.Name, Is.EqualTo("monday-mornings"));
        Assert.That(firstWeekly!.Days, Contains.Item(DayOfWeek.Monday));
        Assert.That(firstWeekly.StartTimeUtc, Is.EqualTo("00:00:00"));
        Assert.That(firstWeekly.EndTimeUtc, Is.EqualTo("12:00:00"));
        Assert.That(managedPolicy.MinTotalSlots, Is.EqualTo(16));
        Assert.That(managedPolicy.MaxTotalSlots, Is.EqualTo(32));
        Assert.That(managedPolicy.MinIdleSlots, Is.EqualTo(8));
        Assert.That(managedPolicy.MinIdleTimeSeconds, Is.EqualTo(90));
        Assert.That(managedPolicy.ScalingFactor, Is.EqualTo(0.6).Within(0.01));

        if (useJsonFile)
        {
            File.Delete(fileName);
        }
    }

    [Test]
    public async Task ListPools()
    {
        var mock = new MockParser();

        await mock.Parser.InvokeAsync(new[] { "pool", "list" });
        mock.PoolUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == null &&
            model.Id == null &&
            !model.Tags.Any() &&
            !model.ExclusiveTags.Any()
        )), Times.Once);

        var uuid = Guid.NewGuid().ToString();
        var name = "NAME";
        var shortname = "shortname";
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(new[] { "pool", "list", "--name", name, "--shortname", shortname, "--id", uuid, "--tags", tags[0], tags[1] });
        mock.PoolUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Shortname == shortname &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            !model.ExclusiveTags.Any()
        )), Times.Once);
    }

    [Test]
    public async Task ListPoolsPage()
    {
        var mock = new MockParser();

        await mock.Parser.InvokeAsync(new[] { "pool", "list", "--no-paginate" });
        mock.PoolUseCases.Verify(useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == null &&
            model.Id == null &&
            !model.Tags.Any() &&
            !model.ExclusiveTags.Any() &&
            model.NoPaginate
        )), Times.Once);

        var token = "zefabuloustoken";
        await mock.Parser.InvokeAsync(new[] { "pool", "list", "--next-page-token", token });
        mock
            .PoolUseCases
            .Verify(
                useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model => model.NextPageToken == token)),
                Times.Once);

        var maxPageSize = 42;
        var namePrefix = "zeprefix";
        var createdBefore = "01-01-2022";
        var createdAfter = "02-01-2022";
        await mock.Parser.InvokeAsync(new[] { "pool", "list", "--max-page-size", $"{maxPageSize}", "--next-page", "--name-prefix", namePrefix, "--created-before", createdBefore, "--created-after", createdAfter });
        mock
            .PoolUseCases
            .Verify(
                useCases => useCases.List(It.Is<GetPoolsOrTasksModel>(model =>
                    model.MaxPageSize == maxPageSize
                    && model.NextPage
                    && model.NamePrefix == namePrefix
                    && model.CreatedAfter == createdAfter
                    && model.CreatedBefore == createdBefore)),
                Times.Once);
    }

    [Test]
    public async Task InfoPools()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var name = "NAME";
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(new[] { "pool", "info", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] });

        mock.PoolUseCases.Verify(useCases => useCases.Info(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            !model.ExclusiveTags.Any()
        )), Times.Once);
    }

    [TestCase("list")]
    [TestCase("info")]
    [TestCase("update-resources")]
    [TestCase("set-elastic-settings")]
    [TestCase("delete")]
    public async Task CantHaveTagsAndExclusiveTags(string subcommand)
    {
        var mock = new MockParser();

        var name = "NAME";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };

        var res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", subcommand, "-i", uuid, "-n", name, "-t", tags[0], tags[1],
                "--exclusive-tags", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.Not.EqualTo(0), "can't have both --tags and --exclusive-tags");

        res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", subcommand, "-i", uuid, "-n", name, "-t", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.EqualTo(0), "should be able to have only --tags");

        res = await mock.Parser.InvokeAsync(
            new[] {
                "pool", subcommand, "-i", uuid, "-n", name, "--exclusive-tags", tags[0], tags[1]
            }
        );

        Assert.That(res, Is.EqualTo(0), "shold be able to have only --exclusive-tags");
    }

    [Test]
    public async Task UpdatePoolResources()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var name = "NAME";
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(new[] { "pool", "update-resources", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] });

        mock.PoolUseCases.Verify(useCases => useCases.UpdateResources(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second)
        )), Times.Once);
    }

    [Test]
    public async Task SetPoolElasticSettingsCheckTestParsArg()
    {
        var mock = new MockParser();

        var name = "NAME";
        var uuid = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] {
                "pool", "set-elastic-settings", "--name", name, "--id", uuid, "--min-slot", "1",
                "--max-slot", "2", "--min-idling-slot", "3", "--resize-period", "4",
                "--resize-factor", "5", "--min-idling-time", "6"
            }
        );

        mock.PoolUseCases.Verify(useCases => useCases.UpdateElasticSettings(It.Is<UpdatePoolElasticSettingsModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.MinSlots == 1 &&
            model.MaxSlots == 2 &&
            model.MinIdlingSlots == 3 &&
            model.ResizePeriod == 4 &&
            model.ResizeFactor == 5 &&
            model.MinIdlingTime == 6
        )), Times.Once);
    }

    [Test]
    public async Task DeletePools()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var name = "NAME";
        var tags = new List<string> { "TAG1", "TAG2" };

        await mock.Parser.InvokeAsync(new[] { "pool", "delete", "--name", name, "--id", uuid, "--tags", tags[0], tags[1] });

        mock.PoolUseCases.Verify(useCases => useCases.Delete(It.Is<GetPoolsOrTasksModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            !model.ExclusiveTags.Any()
        )), Times.Once);
    }

    [Test]
    public async Task UpdatePoolConstant()
    {
        var mock = new MockParser();

        var uuid = Guid.NewGuid().ToString();
        var name = "NAME";
        var tags = new List<string> { "TAG1", "TAG2" };
        var constantName = "CONSTANT_NAME";
        var constantValue = "new-constant-value";

        await mock.Parser.InvokeAsync(new[] {
            "pool", "update-constant", "--name", name, "--id", uuid, "--tags", tags[0], tags[1],
            "--constant-name", constantName, "--constant-value", constantValue
        });

        mock.PoolUseCases.Verify(useCases => useCases.UpdateConstant(It.Is<UpdatePoolsOrTasksConstantModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            !model.ExclusiveTags.Any() &&
            model.ConstantName == constantName &&
            model.ConstantValue == constantValue
        )), Times.Once);
    }

    [Test]
    public async Task CreatePoolRequiresNameInstanceCountAndProfile()
    {
        var name = "NAME";
        var instance = "42";
        var profile = "PROFILE";

        var mock = new MockParser();

        var res = await mock.Parser.InvokeAsync(new[] { "pool", "create", "--name", name, "--instanceNodes", instance });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing profile");

        res = await mock.Parser.InvokeAsync(new[] { "pool", "create", "--name", name, "--profile", profile });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing instance count");

        res = await mock.Parser.InvokeAsync(new[] { "pool", "create", "--profile", profile, "--instanceNodes", instance });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed because of missing name");
    }

    [TestCase("-d")]
    [TestCase("--datacenter")]
    public async Task GetPoolCarbonFacts(string datacenterOption)
    {
        var mock = new MockParser();

        var name = "NAME1";
        var uuid = Guid.NewGuid().ToString();
        var tags = new List<string> { "TAG1", "TAG2" };
        var datacenterName = "SOME_DATACENTER";

        await mock.Parser.InvokeAsync(
            new[] {
                "pool", "carbon-facts", "--name", name, "--id", uuid, "--tags", tags[0], tags[1],
                datacenterOption,  datacenterName
            }
        );

        mock.PoolUseCases.Verify(useCases => useCases.CarbonFacts(It.Is<GetCarbonFactsModel>(model =>
            model.Name == name &&
            model.Id == uuid &&
            model.Tags.Zip(tags).All(pair => pair.First == pair.Second) &&
            model.EquivalentDataCenterName == datacenterName
        )), Times.Once);
    }

    private const string ScalingPolicy1 = @"{
        ""policies"": [
            {
                ""type"": ""ManagedTasksQueue"",
                ""name"": ""managed-policy"",
                ""enabledPeriods"": [
                    {
                        ""type"": ""Weekly"",
                        ""name"": ""thursday-evening"",
                        ""days"": [ ""Thursday"" ],
                        ""startTimeUtc"": ""19:30:00"",
                        ""endTimeUtc"": ""22:00:00""
                    },
                    {
                        ""type"": ""Weekly"",
                        ""name"": ""wednesdays"",
                        ""days"": [""Wednesday""],
                        ""startTimeUtc"": ""00:00:00"",
                        ""endTimeUtc"": ""23:59:59.9999999""
                    }
                ],
                ""minTotalSlots"": 0,
                ""maxTotalSlots"": 10,
                ""minIdleSlots"": 1,
                ""minIdleTimeSeconds"": 90,
                ""scalingFactor"": 0.5
            },
            {
                ""type"": ""Fixed"",
                ""name"": ""fixed-policy"",
                ""enabledPeriods"": [
                    {
                        ""type"": ""Always"",
                        ""name"": ""really-always""
                    }
                ],
                ""slotsCount"": 4
            }
        ]
    }";

    private const string ScalingPolicy2 = @"{
        ""policies"": [
            {
                ""type"": ""Fixed"",
                ""name"": ""fixed-policy"",
                ""enabledPeriods"": [
                    {
                        ""type"": ""Always"",
                        ""name"": ""really-always""
                    }
                ],
                ""slotsCount"": 4
            },
            {
                ""type"": ""ManagedTasksQueue"",
                ""name"": ""managed-policy"",
                ""enabledPeriods"": [
                    {
                        ""type"": ""Weekly"",
                        ""name"": ""monday-mornings"",
                        ""days"": [""Monday""],
                        ""startTimeUtc"": ""00:00:00"",
                        ""endTimeUtc"": ""12:00:00""
                    }
                ],
                ""minTotalSlots"": 16,
                ""maxTotalSlots"": 32,
                ""minIdleSlots"": 8,
                ""minIdleTimeSeconds"": 90,
                ""scalingFactor"": 0.6
            }
        ]
    }";
}
