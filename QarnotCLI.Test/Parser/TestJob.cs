using Moq;
using NUnit.Framework;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestJobCommand
{
    [Test]
    public async Task CreateMinimalJob()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "job", "create", "-n", "name" });
        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.Name == "name"
        )), Times.Once);
    }

    [TestCase("12", 12, 0, 0, 0)]
    [TestCase("12.18", 12, 18, 0, 0)]
    [TestCase("12.00:46", 12, 0, 46, 0)]
    [TestCase("12.02:03:10", 12, 2, 3, 10)]
    [TestCase("12.00:00:10", 12, 0, 0, 10)]
    [TestCase("1:2:3", 0, 1, 2, 3)]
    [TestCase("01:00:10", 0, 1, 0, 10)]
    [TestCase("0:10", 0, 0, 10, 0)]
    [TestCase(".::10", 0, 0, 0, 10)]
    [TestCase(":10", 0, 0, 10, 0)]
    public async Task CreateJobParseWallTime(string walltime, int day, int hour, int minute, int second)
    {
        var waitValue = new TimeSpan(day, hour, minute, second);

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime == waitValue
        )), Times.Once);
    }

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(45620)]
    [TestCase(24968746)]
    public async Task CreateJobParseWallTimeFullDate(int secondToAdd)
    {
        var waitValue = new TimeSpan(0, 0, 0, secondToAdd);
        var walltime = DateTime.Now.AddSeconds(secondToAdd).ToString("yyyy/MM/dd HH:mm:ss");

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime >= waitValue - TimeSpan.FromSeconds(10) &&
            model.MaxWallTime <= waitValue + TimeSpan.FromSeconds(10)
        )), Times.Once);
    }

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task CreateJobParseWallTimeWithPartialDate(int dayToAdd)
    {
        var walltime = DateTime.Now.AddDays(dayToAdd).ToString("yyyy/MM/dd");
        var waitValue = new TimeSpan(dayToAdd, 0, 0, 0) - DateTime.Now.TimeOfDay;

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime >= waitValue - TimeSpan.FromSeconds(10) &&
            model.MaxWallTime <= waitValue + TimeSpan.FromSeconds(10)
        )), Times.Once);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task CreateJobParseWallTimeWithOnlyDay(int dayToAdd)
    {
        var walltime = dayToAdd.ToString();
        var waitValue = new TimeSpan(dayToAdd, 0, 0, 0);

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime  }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime!.Value.TotalHours == waitValue.TotalHours
        )), Times.Once);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task CreateJobParseWallTimeWithOnlyHours(int hourToAdd)
    {
        var walltime = "0." + hourToAdd.ToString();
        var waitValue = new TimeSpan(0, hourToAdd, 0, 0);

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime  }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime!.Value.TotalHours == waitValue.TotalHours
        )), Times.Once);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task CreateJobParseWallTimeWithOnlyMinutes(int minuteToAdd)
    {
        var walltime = "0.0:" + minuteToAdd.ToString();
        var waitValue = new TimeSpan(0, 0, minuteToAdd, 0);

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", "name", "--pool", "poolUuid", "--shortname", "shortname", "--max-wall-time", walltime  }
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.MaxWallTime!.Value.TotalHours == waitValue.TotalHours
        )), Times.Once);
    }

    [Test]
    public async Task CreateJob()
    {
        var poolUuid = "PoolUUID";
        var name = "NAME";
        var shortname = "SHORT";

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "create", "--name", name, "--pool", poolUuid, "--shortname", shortname}
        );

        mock.JobUseCases.Verify(useCases => useCases.Create(It.Is<CreateJobModel>(model =>
            model.Name == name &&
            model.Shortname == shortname &&
            model.Pool == poolUuid &&
            !model.IsDependent &&
            model.MaxWallTime == null
        )), Times.Once);
    }

    [Test]
    public async Task ListJob()
    {
        var jobUuid = "JobUUID";
        var name = "NAME";

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "list", "--name", name, "--id", jobUuid }
        );

        mock.JobUseCases.Verify(useCases => useCases.List(It.Is<GetJobModel>(model =>
            model.Name == name &&
            model.Id == jobUuid
        )), Times.Once);
    }

    [Test]
    public async Task JobInfo()
    {
        var name = "NAME";

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "info", "--name", name }
        );

        mock.JobUseCases.Verify(useCases => useCases.Info(It.Is<GetJobModel>(model =>
            model.Name == name
        )), Times.Once);
    }

    [Test]
    public async Task AbortJob()
    {
        var name = "NAME";

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "abort", "--name", name }
        );

        mock.JobUseCases.Verify(useCases => useCases.Abort(It.Is<GetJobModel>(model =>
            model.Name == name
        )), Times.Once);
    }

    [Test]
    public async Task DeleteJob()
    {
        var name = "NAME";

        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "job", "delete", "--name", name }
        );

        mock.JobUseCases.Verify(useCases => useCases.Delete(It.Is<GetJobModel>(model =>
            model.Name == name
        )), Times.Once);
    }

    [TestCase("delete")]
    [TestCase("info")]
    [TestCase("abort")]
    public async Task OnlyOneOfAllNameOrId(string verb)
    {
        var mock = new MockParser();
        var res = await mock.Parser.InvokeAsync(
            new[] { "job", verb, "--id", "id", "--name", "name", "--all" }
        );

        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed");
    }

    [Test]
    public async Task CreateJobNeedsAName()
    {
        var mock = new MockParser();
        var res = await mock.Parser.InvokeAsync(new[] { "job", "create" });

        Assert.That(res, Is.Not.EqualTo(0));
    }
}
