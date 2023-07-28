using Moq;
using NUnit.Framework;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestAccountCommand
{
    [Test]
    public async Task RetrieveAccountInformation([Values(false, true)] bool humanReadable)
    {
        var bytesFormatter = new ByteValueFormatter(humanReadable);

        var mock = new MockParser();

        var args = humanReadable
            ? new[] { "account", "-h" }
            : new[] { "account" };

        await mock.Parser.InvokeAsync(args);

        mock.AccountUseCases.Verify(
            useCases => useCases.Get(It.Is<GlobalModel>(model => model.HumanReadable == humanReadable)),
            Times.Once
        );
    }
}
