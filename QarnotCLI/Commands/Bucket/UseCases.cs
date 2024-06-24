using QarnotSDK;

namespace QarnotCLI;

public interface IBucketUseCases
{
    Task Create(CreateBucketModel model);
    Task List(ListBucketModel model);
    Task Get(GetBucketModel model);
    Task Put(PutBucketModel model);
    Task SyncFrom(SyncBucketModel model);
    Task SyncTo(SyncBucketModel model);
    Task Delete(DeleteBucketModel model);
    Task DeleteAll();
}

public class BucketUseCases : IBucketUseCases
{
    private readonly ILogger Logger;
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;

    public BucketUseCases(
        Connection qarnotAPI,
        IFormatter formatter,
        IStateManager _,
        ILogger logger)
    {
        QarnotAPI = qarnotAPI;
        Logger = logger;
        Formatter = formatter;
    }

    public async Task Create(CreateBucketModel model)
    {
        Logger.Debug($"Creating new bucket {model.Name}");

        var bucket = await QarnotAPI.CreateBucketAsync(model.Name);

        await bucket.CreateAsync();

        Logger.Debug($"Bucket created with short name {bucket.Shortname}");
        Logger.Debug($"Uploading files to bucket {bucket.Shortname}");

        var fileUploads = model.Files.Select(
            async f => await bucket.UploadFileAsync(f, null)
        );

        var folderUploads = model.Folders.Select(
            async f => await bucket.UploadFolderAsync(f, null)
        );

        await Task.WhenAll(fileUploads.Concat(folderUploads));

        Logger.Result(Formatter.Format(new {
            Bucket = bucket.Shortname,
            State = "Created",
            LocalPaths = model.Files.Concat(model.Folders),
        }));
    }

    public async Task Delete(DeleteBucketModel model)
    {
        var bucket = await GetBucket(model.Name);
        if (model.BucketPaths.Any())
        {
            Logger.Debug($"Deleting from bucket {model.Name}: {string.Join(",", model.BucketPaths)}");

            if (!model.DisableBucketPathsSanitization
                    && model.BucketPaths.Any(p => ShouldBeSanitized(p)))
            {
                var invalidPaths = model.BucketPaths.Where(p => ShouldBeSanitized(p));
                throw new Exception($"Invalid remote paths: {string.Join(",", invalidPaths)}");
            }

            await Task.WhenAll(model.BucketPaths.Select(async p =>
                await bucket.DeleteEntryAsync(p)
            ));

            Logger.Result(Formatter.Format(new {
                Bucket = bucket.Shortname,
                State = "Deleted",
                RemotePaths = model.BucketPaths
            }));
        }
        else
        {
            Logger.Debug($"Deleting bucket {model.Name}");

            await bucket.DeleteAsync();
            Logger.Result(Formatter.Format(new {
                Bucket = bucket.Shortname,
                State = "Deleted"
            }));
        }
    }

    public async Task DeleteAll()
    {
        Logger.Debug("Deleting all buckets");
        var bucket = await QarnotAPI.RetrieveBucketsAsync();
        var deletions = await Task.WhenAll(bucket.Select(async b => {
            await b.DeleteAsync();
            return new {
                Bucket = b.Shortname,
                State = "Deleted"
            };
        }));

        Logger.Result(Formatter.FormatCollection(deletions.ToList()));
    }

    public async Task Get(GetBucketModel model)
    {
        var bucket = await GetBucket(model.Name);
        var localPath = model.LocalPath ?? ".";
        if (!model.DisableBucketPathsSanitization
            && (model.BucketFiles.Any(f => ShouldBeSanitized(f))
             || model.BucketFolders.Any(f => ShouldBeSanitized(f))
             || ShouldBeSanitized(model.String)))
        {
            var invalidPaths = model.BucketFiles.Where(f => ShouldBeSanitized(f))
                .Concat(model.BucketFolders.Where(f => ShouldBeSanitized(f)))
                .ToList();

            if (ShouldBeSanitized(model.String))
            {
                invalidPaths.Append(model.String);
            }

            throw new Exception($"Invalid remote paths: {invalidPaths}");
        }

        if (!string.IsNullOrEmpty(model.String))
        {
            Logger.Debug($"Downloading string {model.String} from bucket {model.Name}");
            var result = await bucket.DownloadStringAsync(model.String);
            Logger.Result(Formatter.Format(new {
                Bucket = bucket.Shortname,
                String = model.String,
                Result = result
            }));

            return;
        }

        Logger.Debug($"Downloading files and folders from bucket {model.Name}");
        var downloadFiles = model.BucketFiles.Select(async f => {
            await bucket.DownloadFileAsync(f, Path.Join(localPath, Path.GetFileName(f)));
            return f;
        });

        var downloadFolders = model.BucketFolders.Select(async f => {
            await bucket.DownloadFolderAsync(f, localPath);
            return f;
        });

        var downloads = await Task.WhenAll(downloadFiles.Concat(downloadFolders));

        Logger.Result(Formatter.Format(new {
            Bucket = bucket.Shortname,
            State = "Downloaded",
            RemotePaths = downloads,
        }));
    }

    public async Task List(ListBucketModel model)
    {
        if (model.Name is null)
        {
            Logger.Debug("Listing all buckets");
            await ListBuckets(model);
        }
        else
        {
            Logger.Debug($"Listing bucket {model.Name}");
            await ListBucket(model);
        }
    }

    private async Task ListBucket(ListBucketModel model)
    {
        var bucket = await GetBucket(model.Name!);
        await bucket.UpdateAsync();

        var folder = model.Folder ?? "";
        if (!model.DisableBucketPathsSanitization && ShouldBeSanitized(folder))
        {
            throw new Exception($"Invalid remote path: {folder}");
        }

        var bytesFormatter = new  ByteValueFormatter(model.HumanReadable);
        var entries = await bucket.ListEntriesAsync(folder);
        Logger.Result(Formatter.Format(
            new BucketSummary(
                Shortname: bucket.Shortname,
                TotalFileCount: (uint)bucket.FileCount,
                TotalBytes: bytesFormatter.Format(bucket.UsedSpaceBytes),
                Entries: entries.Select(e =>
                    string.Format("{0,7} - {1}", bytesFormatter.Format(e.Size), e.Name)
                ).ToList()
            )
        ));
    }

    private async Task ListBuckets(ListBucketModel model)
    {
        var buckets = await QarnotAPI.RetrieveBucketsAsync();
        var bytesFormatter = new ByteValueFormatter(model.HumanReadable);
        Logger.Result(Formatter.FormatCollection(
            buckets.Select(b =>new {
                Shortname = b.Shortname,
                TotalFileCount= (uint)b.FileCount,
                TotalBytes= bytesFormatter.Format(b.UsedSpaceBytes),
            }).ToList()
        ));
    }

    public async Task Put(PutBucketModel model)
    {
        var bucket = await GetBucket(model.Name!);
        if (!model.DisableBucketPathsSanitization
            && (ShouldBeSanitized(model.BucketPath)))
        {
            throw new Exception($"Invalid remote path: {model.BucketPath}");
        }

        if (!string.IsNullOrEmpty(model.Send))
        {
            if (model.BucketPath is null)
            {
                throw new Exception($"Can't upload string without a path");
            }

            Logger.Debug($"Uploading string to {model.BucketPath} in bucket {model.Name}");
            await bucket.UploadStringAsync(model.Send, model.BucketPath);
            return;
        }

        Logger.Debug($"Uploading files and folders to {model.BucketPath} in bucket {model.Name}");
        var uploadFiles = model.LocalFiles.Select(async f => {
            await bucket.UploadFileAsync(f, model.BucketPath);
            return f;
        });

        var uploadFolders = model.LocalFolders.Select(async f => {
            await bucket.UploadFolderAsync(f, model.BucketPath);
            return f;
        });

        var uploads = await Task.WhenAll(uploadFiles.Concat(uploadFolders));
        Logger.Result(Formatter.Format(new {
            Bucket = model.Name,
            RemotePath = model.BucketPath,
            LocalPaths = uploads.ToList(),
            State = "Uploaded"
        }));
    }

    public async Task SyncFrom(SyncBucketModel model)
    {
        var bucket = await GetBucket(model.Name);
        var bucketFolder = model.BucketFolder ?? "";
        if (!model.DisableBucketPathsSanitization
            && ShouldBeSanitized(bucketFolder))
        {
            throw new Exception($"Invalid remote path: {bucketFolder}");
        }

        Logger.Debug($"Syncing from remote bucket {model.Name} folder {bucketFolder} to local path {model.LocalPath}");
        await bucket.SyncRemoteToLocalAsync(
            model.LocalPath,
            cancellationToken: default,
            remoteFolderRelativePath: bucketFolder,
            dontDelete: model.NoDelete
        );

        Logger.Result(Formatter.Format(new {
            Bucket = model.Name,
            RemotePath = bucketFolder,
            LocalPath = model.LocalPath,
            State = "Downloaded",
        }));
    }

    public async Task SyncTo(SyncBucketModel model)
    {
        var bucket = await GetBucket(model.Name);
        var bucketFolder = model.BucketFolder ?? "";
        if (!model.DisableBucketPathsSanitization && ShouldBeSanitized(bucketFolder))
        {
            throw new Exception($"Invalid remote path: {bucketFolder}");
        }

        Logger.Debug($"Syncing from local path {model.LocalPath} to remote bucket {model.Name} folder {bucketFolder}");
        await bucket.SyncLocalToRemoteAsync(
            model.LocalPath,
            cancellationToken: default,
            remoteFolderRelativePath: bucketFolder,
            dontDelete: model.NoDelete
        );

        Logger.Result(Formatter.Format(new {
            Bucket = model.Name,
            RemotePath = bucketFolder,
            LocalPath = model.LocalPath,
            State = "Uploaded"
        }));
    }

    private bool ShouldBeSanitized(string? path)
    {
        var directorySeparators = new[]{'/', '\\'};
        var shouldBeSanitized = false;

        if (path != default)
        {
            foreach (var separator in directorySeparators)
            {
                if (path.Contains(String.Format("{0}{0}", separator)))
                {
                    Logger.Warning($"Bucket path should not contain duplicated slashes ('{String.Format("{0}{0}",separator)}')");
                    shouldBeSanitized = true;
                }
                if (path.StartsWith(separator.ToString()))
                {
                    Logger.Warning($"Bucket path should not start with a slash ('{separator}')");
                    shouldBeSanitized = true;
                }
            }

            if (shouldBeSanitized)
            {
                Logger.Warning($"The path {path} is invalid. Fix the path by removing the extra separators or use the argument --no-sanitize-bucket-paths if you are sure about the path");
            }
        }

        return shouldBeSanitized;
    }

    private async Task<QarnotSDK.QBucket> GetBucket(string name)
    {
        var bucket = await QarnotAPI.RetrieveBucketAsync(name);
        if (bucket is null)
        {
            throw new Exception($"Bucket {name} doesn't exist");
        }

        return bucket;
    }
}
