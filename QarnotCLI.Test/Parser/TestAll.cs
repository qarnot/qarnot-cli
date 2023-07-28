using Moq;
using NUnit.Framework;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestAllCommmand
{
    [Test]
    public async Task DeleteEverything()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "all", "--delete" });

        mock.AllUseCases.Verify(
            useCases => useCases.Delete(It.IsAny<GlobalModel>()),
            Times.Once
        );
    }

    [Test]
    public async Task AbortEverything()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "all", "--abort" });

        mock.AllUseCases.Verify(
            useCases => useCases.Abort(It.IsAny<GlobalModel>()),
            Times.Once
        );
    }

    [Test]
    public async Task ListEverything()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "all", "--list" });

        mock.AllUseCases.Verify(
            useCases => useCases.List(It.IsAny<GlobalModel>()),
            Times.Once
        );
    }

    [Test]
    public async Task ListEverythingByDefault()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "all" });

        mock.AllUseCases.Verify(
            useCases => useCases.List(It.IsAny<GlobalModel>()),
            Times.Once
        );
    }

    [Test]
    public async Task CantGiveAllOptionsAtOnce()
    {
        var mock = new MockParser();
        var res = await mock.Parser.InvokeAsync(new[] { "all", "--list", "--delete", "--abort" });

        Assert.That(res, Is.Not.EqualTo(0), "parsing should have failed");
    }
}
