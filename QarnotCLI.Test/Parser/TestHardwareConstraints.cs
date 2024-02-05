using Moq;
using NUnit.Framework;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestHardwareConstraintsCommand
{
    [Test]
    public async Task RetrieveAccountInformation()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(new[] { "hw-constraints", "list" });

        mock.HardwareConstraintsUseCase.Verify(
            useCases => useCases.List(It.IsAny<GlobalModel>()),
            Times.Once
        );

    }
}
