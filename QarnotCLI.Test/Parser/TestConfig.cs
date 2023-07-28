using Moq;
using NUnit.Framework;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestConfigCommand
{
    [Test]
    public async Task WriteConfiguration()
    {
        var mock = new MockParser();

        var token = Guid.NewGuid().ToString();
        var uri = Guid.NewGuid().ToString();
        var storageUri = Guid.NewGuid().ToString();
        var email = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "config", "--global", "--token", token, "--api-uri", uri, "--storage-uri", storageUri, "--account-email", email }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.Run(It.Is<RunConfigModel>(model =>
            model.Token == token &&
            model.ApiUri == uri &&
            model.StorageUri == storageUri &&
            model.AccountEmail == email &&
            model.Global &&
            !model.Show
        )), Times.Once);
    }
}
