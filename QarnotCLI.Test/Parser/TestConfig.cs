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
            new[] { "config","set", "--local", "-t", token, "--api-uri", uri, "--storage-uri", storageUri, "--account-email", email }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.SetConfig(It.Is<SetConfigModel>(model =>
            model.Token == token &&
            model.ApiUri == uri &&
            model.StorageUri == storageUri &&
            model.AccountEmail == email &&
            model.Local &&
            !model.Show
        )), Times.Once);
    }

    [Test]
    public async Task WriteGlobalConfigurationByDefault()
    {
        var mock = new MockParser();

        var token = Guid.NewGuid().ToString();
        var uri = Guid.NewGuid().ToString();
        var storageUri = Guid.NewGuid().ToString();
        var email = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "config","set", "-t", token, "--api-uri", uri, "--storage-uri", storageUri, "--account-email", email }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.SetConfig(It.Is<SetConfigModel>(model =>
            model.Token == token &&
            model.ApiUri == uri &&
            model.StorageUri == storageUri &&
            model.AccountEmail == email &&
            !model.Local &&
            !model.Show
        )), Times.Once);
    }

    [Test]
    public async Task ReadLocalConfiguration()
    {
        var mock = new MockParser();

        var token = Guid.NewGuid().ToString();
        var uri = Guid.NewGuid().ToString();
        var storageUri = Guid.NewGuid().ToString();
        var email = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "config","show" }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.ShowConfig(It.Is<ShowConfigModel>(model =>
            !model.Global &&
            !model.WithoutEnv
        )), Times.Once);
    }

    [Test]
    public async Task ReadGlobalConfigurationWithoutEnv()
    {
        var mock = new MockParser();

        var token = Guid.NewGuid().ToString();
        var uri = Guid.NewGuid().ToString();
        var storageUri = Guid.NewGuid().ToString();
        var email = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "config","show", "--without-env" }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.ShowConfig(It.Is<ShowConfigModel>(model =>
            !model.Global &&
            model.WithoutEnv
        )), Times.Once);
    }

    [Test]
    public async Task ReadGlobalConfiguration()
    {
        var mock = new MockParser();

        var token = Guid.NewGuid().ToString();
        var uri = Guid.NewGuid().ToString();
        var storageUri = Guid.NewGuid().ToString();
        var email = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "config","show", "--global" }
        );

        mock.ConfigUseCases.Verify(useCases => useCases.ShowConfig(It.Is<ShowConfigModel>(model =>
            model.Global &&
            !model.WithoutEnv
        )), Times.Once);
    }

}
