using Moq;
using NUnit.Framework;

namespace QarnotCLI.Test;

[TestFixture]
public class TestQuotaCommand
{
    [Test]
    public async Task GetUserQuota()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "quota", "computing", "user" }
        );

        mock.QuotaUseCases.Verify(useCases => useCases.Get(It.Is<GetQuotaModel>(model =>
            model.Scope == "user"
        )), Times.Once);
    }

    [Test]
    public async Task GetOrganizationQuota()
    {
        var mock = new MockParser();
        await mock.Parser.InvokeAsync(
            new[] { "quota", "computing", "organization" }
        );

        mock.QuotaUseCases.Verify(useCases => useCases.Get(It.Is<GetQuotaModel>(model =>
            model.Scope == "organization"
        )), Times.Once);
    }
}
