namespace QarnotCLI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class DeleteBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : Delete");
            await bucket.DeleteAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = "Bucket deleted",
            };
        }
    }

    public class BucketListResumeCommand : ICommand<QBucket, CommandValues.BucketCommandValue>
    {
        public BucketListResumeCommand(IFormatter bytesFormater)
        {
            BytesFormat = bytesFormater;
        }

        private IFormatter BytesFormat { get; }

        public virtual Task<CommandValues.BucketCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            var command = new CommandValues.BucketCommandValue()
            {
                Shortname = bucket.Shortname,
                FileCount = bucket.FileCount.ToString(),
                UsedSpaceBytes = iconfig.HumanReadable ? BytesFormat.ConvertToHumanReadable(bucket.UsedSpaceBytes) : bucket.UsedSpaceBytes.ToString(),
            };
            return Task.FromResult(command);
        }
    }

    public class ListBucketEntriesCommand : ICommand<QBucket, CommandValues.BucketInfoCommandValue>
    {
        public ListBucketEntriesCommand(IFormatter convert)
        {
            ConvertBytes = convert;
        }

        private IFormatter ConvertBytes { get; }

        private string FormatQAbstractStorageEntryToBucketInfo(QAbstractStorageEntry entity, bool humanReadable)
        {
            string sizeFormat = null;

            if (humanReadable)
            {
                sizeFormat = this.ConvertBytes.ConvertToHumanReadable(entity.Size, 2);
            }
            else
            {
                sizeFormat = entity.Size.ToString();
            }

            return string.Format("{0,7} - {1}", sizeFormat, entity.Name);
        }

        private async Task<List<string>> ExtractBucketFilesInfoFromBucket(QBucket bucket, string folder, CancellationToken ct, bool humanReadable)
        {
            List<string> folderList = new List<string>();

            var listEntries = await bucket.ListEntriesAsync(folder, cancellationToken:ct);
            foreach (QAbstractStorageEntry entity in listEntries)
            {
                string parsedString = FormatQAbstractStorageEntryToBucketInfo(entity, humanReadable);
                folderList.Add(parsedString);
            }

            return folderList;
        }

        public virtual async Task<CommandValues.BucketInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : Info");
            await bucket.UpdateAsync(ct);
            BucketConfiguration config = iconfig as BucketConfiguration;
            List<string> folderList = new List<string>();
            string folder = config.RemoteRelativePath ?? "";

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (PathSanitization.IsThePathInvalid(folder)))
            {
                return null;
            }

            folderList = await ExtractBucketFilesInfoFromBucket(bucket, folder, ct, iconfig.HumanReadable);

            return new CommandValues.BucketInfoCommandValue()
            {
                Shortname = bucket.Shortname,
                TotalFileCount = bucket.FileCount.ToString(),
                TotalUsedSpaceBytes = iconfig.HumanReadable ? this.ConvertBytes.ConvertToHumanReadable(bucket.UsedSpaceBytes, 2) : bucket.UsedSpaceBytes.ToString(),
                BucketFilesDetail = folderList,
            };
        }
    }

    public class SynchronizeLocalFolderFromBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : SyncFrom");
            BucketConfiguration config = iconfig as BucketConfiguration;
            string localFolderPath = config.LocalPathGet;
            bool donTDelete = config.DeleteFiles;
            string remoteFolderRelativePath = config.RemoteRelativePath;

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (PathSanitization.IsThePathInvalid(remoteFolderRelativePath)))
            {
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = bucket.Shortname,
                    Message = "Synchronization failed. Invalid remote path",
                };
            }

            CLILogs.Debug("Local path : " + localFolderPath);

            await bucket.SyncRemoteToLocalAsync(localFolderPath, ct, donTDelete, remoteFolderRelativePath);

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = "Bucket sync to " + localFolderPath,
            };
        }
    }

    public class SynchronizeLocalFolderToBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : SyncTo");
            BucketConfiguration config = iconfig as BucketConfiguration;
            string localFolderPath = config.LocalPathGet;
            bool donTDelete = config.DeleteFiles;
            string remoteFolderRelativePath = config.RemoteRelativePath;

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (PathSanitization.IsThePathInvalid(remoteFolderRelativePath)))
            {
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = bucket.Shortname,
                    Message = "Synchronization failed. Invalid remote path",
                };
            }

            CLILogs.Debug("Local path : " + localFolderPath);

            await bucket.SyncLocalToRemoteAsync(localFolderPath, ct, donTDelete, remoteFolderRelativePath);

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = localFolderPath + "send to Bucket",
            };
        }
    }

    public class UploadBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : Upload");

            BucketConfiguration config = iconfig as BucketConfiguration;
            List<Task> listOfTask = new List<Task>();

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (PathSanitization.IsThePathInvalid(config.RemoteRelativePath)))
            {
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = bucket.Shortname,
                    Message = "Upload failed. Invalid remote path",
                };
            }

            if (!string.IsNullOrEmpty(config.String))
            {
                CLILogs.Debug("Command Bucket : Upload String ");
                CLILogs.Debug(config.StringPath);
                await bucket.UploadStringAsync(config.String, config.StringPath, cancellationToken: ct);
            }

            listOfTask.AddRange(config.LocalPathFiles.Select(localFile => bucket.UploadFileAsync(localFile, config.RemoteRelativePath, cancellationToken: ct)));
            listOfTask.AddRange(config.LocalPathFolders.Select(localFolder => bucket.UploadFolderAsync(localFolder, config.RemoteRelativePath, cancellationToken: ct)));

            await Task.WhenAll(listOfTask);

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = "Bucket Upload",
            };
        }
    }

    public class DownloadBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Bucket : Download");
            BucketConfiguration config = iconfig as BucketConfiguration;
            List<Task> listOfTask = new List<Task>();
            string returnString = "Bucket download";
            string localPath = config.LocalPathGet ?? ".";

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (config.RemoteRelativePathFiles.Any(remoteFile => PathSanitization.IsThePathInvalid(remoteFile))
                 || config.RemoteRelativePathFolders.Any(remoteFile => PathSanitization.IsThePathInvalid(remoteFile))
                 || PathSanitization.IsThePathInvalid(config.StringPath)))
            {
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = bucket.Shortname,
                    Message = "Download failed. Invalid remote path(s)",
                };
            }

            if (!string.IsNullOrEmpty(config.StringPath))
            {
                CLILogs.Debug("Command Bucket : DownLoad String");
                returnString = await bucket.DownloadStringAsync(config.StringPath, cancellationToken: ct);
            }

            listOfTask.AddRange(config.RemoteRelativePathFiles.Select(remoteFile => bucket.DownloadFileAsync(remoteFile, Path.Join(localPath, Path.GetFileName(remoteFile)), cancellationToken:ct)));
            listOfTask.AddRange(config.RemoteRelativePathFolders.Select(remoteFolder => bucket.DownloadFolderAsync(remoteFolder, localPath, cancellationToken: ct)));

            await Task.WhenAll(listOfTask);

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = returnString,
            };
        }
    }

    public class RemoveEntityBucketCommand : ICommand<QBucket, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QBucket bucket, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            BucketConfiguration config = iconfig as BucketConfiguration;

            if (!config.ApiConnection.DisableBucketPathsSanitization
                && (config.RemoteRelativePaths.Any(remoteFile => PathSanitization.IsThePathInvalid(remoteFile))))
            {
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = bucket.Shortname,
                    Message = "Removal failed. Invalid remote path(s)",
                };
            }
            CLILogs.Debug("Command Bucket : Remove entities " + string.Join(", ", config.RemoteRelativePaths));
            string returnString = "Bucket delete paths : " + string.Join(", ", config.RemoteRelativePaths);

            await config.RemoteRelativePaths.ParallelForEachAsync(bucket.DeleteEntryAsync, 10, ct:ct);

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = bucket.Shortname,
                Message = returnString,
            };
        }
    }

}