using Moq;
using NUnit.Framework;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestBucketCommand
{
    [Test]
    public async Task CreateBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "create", "--name", name}
        );

        mock.BucketUseCases.Verify(useCases => useCases.Create(It.Is<CreateBucketModel>(model =>
            model.Name == name
        )), Times.Once);
    }

    [Test]
    public async Task CreateBucketMissingName()
    {
        var mock = new MockParser();

        var res = await mock.Parser.InvokeAsync(new[] { "bucket", "create" });
        Assert.That(res, Is.Not.EqualTo(0), "parsing should fail because of missing name");
    }

    [Test]
    public async Task UploadToBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var folder = $"/{Guid.NewGuid()}/";

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "put", "--name", name, "--local-folder", folder }
        );

        mock.BucketUseCases.Verify(useCases => useCases.Put(It.Is<PutBucketModel>(model =>
            model.Name == name &&
            model.LocalFolders.Contains(folder)
        )), Times.Once);
    }

    [Test]
    public async Task GetFromBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var folder = $"/{Guid.NewGuid()}/";

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "get", "--name", name, "--bucket-folder", folder }
        );

        mock.BucketUseCases.Verify(useCases => useCases.Get(It.Is<GetBucketModel>(model =>
            model.Name == name &&
            model.BucketFolders.Contains(folder)
        )), Times.Once);
    }

    [Test]
    public async Task SyncFromBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var localPath = $"/{Guid.NewGuid()}";

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "sync-from", "--name", name, "--local-path", localPath }
        );

        mock.BucketUseCases.Verify(useCases => useCases.SyncFrom(It.Is<SyncBucketModel>(model =>
            model.Name == name &&
            model.LocalPath == localPath
        )), Times.Once);
    }

    [Test]
    public async Task SyncToBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var localPath = $"/{Guid.NewGuid()}";

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "sync-to", "--name", name, "--local-path", localPath }
        );

        mock.BucketUseCases.Verify(useCases => useCases.SyncTo(It.Is<SyncBucketModel>(model =>
            model.Name == name &&
            model.LocalPath == localPath
        )), Times.Once);
    }

    [Test]
    public async Task ListBuckets()
    {
        var mock = new MockParser();

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "list" }
        );

        mock.BucketUseCases.Verify(useCases => useCases.List(It.Is<ListBucketModel>(model =>
            model.Name == null &&
            model.Folder == null
        )), Times.Once);
    }

    [Test]
    public async Task ListBucketItems()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var folder = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(
            new[] { "bucket", "list", "--name", name, "-f", folder }
        );

        mock.BucketUseCases.Verify(useCases => useCases.List(It.Is<ListBucketModel>(model =>
            model.Name == name &&
            model.Folder == folder
        )), Times.Once);
    }

    [Test]
    public async Task DeleteBucket()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(new[] { "bucket", "delete", "--name", name });

        mock.BucketUseCases.Verify(useCases => useCases.Delete(It.Is<DeleteBucketModel>(model =>
            model.Name == name &&
            !model.BucketPaths.Any()
        )), Times.Once);
    }

    [Test]
    public async Task DeleteBucketItems()
    {
        var mock = new MockParser();

        var name = Guid.NewGuid().ToString();
        var path1 = Guid.NewGuid().ToString();
        var path2 = Guid.NewGuid().ToString();

        await mock.Parser.InvokeAsync(new[] { "bucket", "delete", "--name", name, path1, path2 });

        mock.BucketUseCases.Verify(useCases => useCases.Delete(It.Is<DeleteBucketModel>(model =>
            model.Name == name &&
            model.BucketPaths.Contains(path1) &&
            model.BucketPaths.Contains(path2)
        )), Times.Once);
    }
}
