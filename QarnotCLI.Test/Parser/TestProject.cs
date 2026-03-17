using System.CommandLine.Parsing;
using Moq;
using NUnit.Framework;

namespace QarnotCLI.Test;

[TestFixture]
public class TestProjectCommand
{
    [Test]
    public async Task ListProjects()
    {
        var mock = new MockParser();

        var res = await mock.Parser.InvokeAsync(new[] { "project", "list" });

        Assert.That(res, Is.EqualTo(0), "parsing should succeed");

        mock.ProjectUseCases.Verify(
            useCases => useCases.List(It.IsAny<GlobalModel>()),
            Times.Once
        );
    }
}
