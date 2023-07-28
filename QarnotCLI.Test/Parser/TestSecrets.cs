using Moq;
using NUnit.Framework;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestSecretsCommand
{
    [Test]
    public async Task GetSecret()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "secrets", "get", "path/to/secret" }
        );

        mock.SecretsUseCases.Verify(useCases => useCases.Get(It.Is<GetSecretModel>(model =>
            model.Key == "path/to/secret"
        )), Times.Once);
    }

    [Test]
    public async Task CreateSecret()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "secrets", "create", "path/to/secret", "value" }
        );

        mock.SecretsUseCases.Verify(useCases => useCases.Create(It.Is<WriteSecretModel>(model =>
            model.Key == "path/to/secret" &&
            model.Value == "value"
        )), Times.Once);
    }

    [Test]
    public async Task UpdateSecret()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "secrets", "update", "path/to/secret", "value" }
        );

        mock.SecretsUseCases.Verify(useCases => useCases.Update(It.Is<WriteSecretModel>(model =>
            model.Key == "path/to/secret" &&
            model.Value == "value"
        )), Times.Once);
    }

    [Test]
    public async Task DeleteSecret()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "secrets", "delete", "path/to/secret" }
        );

        mock.SecretsUseCases.Verify(useCases => useCases.Delete(It.Is<GetSecretModel>(model =>
            model.Key == "path/to/secret"
        )), Times.Once);
    }

    [Test]
    public async Task ListSecrets()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "secrets", "list", "-r", "path/to/secret" }
        );

        mock.SecretsUseCases.Verify(useCases => useCases.List(It.Is<ListSecretsModel>(model =>
            model.Prefix == "path/to/secret" &&
            model.Recursive
        )), Times.Once);
    }
}
