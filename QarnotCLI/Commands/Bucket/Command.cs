using System.CommandLine;

namespace QarnotCLI;

public class BucketCommand : Command
{
    private readonly GlobalOptions GlobalOptions;
    private readonly Func<GlobalModel, IBucketUseCases> Factory;

    public BucketCommand(GlobalOptions globalOptions, Func<GlobalModel, IBucketUseCases> factory)
        : base("bucket", "Bucket commands")
    {
        Factory = factory;
        GlobalOptions = globalOptions;

        AddCommand(BuildCreateCommand());
        AddCommand(BuildListCommand());
        AddCommand(BuildGetCommand());
        AddCommand(BuildPutCommand());
        AddCommand(BuildSyncFromCommand());
        AddCommand(BuildSyncToCommand());
        AddCommand(BuildDeleteCommand());
    }

    private Command BuildCreateCommand()
    {
        var examples = new[]
        {
            new Example(
                Title: "Only create the bucket",
                CommandLines: new[] {
                  "qarnot bucket create --name Bucket-name",
                }
            ),
            new Example(
                Title: "Create a bucket and send some files and/or folders",
                CommandLines: new[] {
                  "qarnot bucket create --files ./FileToSend1.py ./FileToSend2.py --name \"Bucket name\"",
                  "qarnot bucket create --folder ./FolderToSend1 ./FolderToSend2 --name \"Bucket name\""
                }
            ),
            new Example(
                Title: "Error: missing name",
                CommandLines: new[] {
                    "qarnot bucket create"
                },
                IsError: true
            ),
        };

        var filesOpt = new Option<List<string>>(
            name: "--files",
            description: "Files to send to the bucket."
        ) { AllowMultipleArgumentsPerToken = true };

        var folderOpt = new Option<List<string>>(
            name: "--folder",
            description: "Folders to send to the bucket."
        ) { AllowMultipleArgumentsPerToken = true };

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to create"
        ) { IsRequired = true };

        var cmd = new CommandWithExamples("create", "Create a new bucket")
        {
            nameOpt,
            examples,
            filesOpt,
            folderOpt,
        };

        cmd.SetHandler(
            model => Factory(model).Create(model),
            new CreateBucketBinder(
                nameOpt,
                filesOpt,
                folderOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildListCommand()
    {
        var examples = new[]
        {
            new Example(
                Title: "List the bucket of the api",
                CommandLines: new[] {
                    "qarnot bucket list"
                }
            ),
            new Example(
                Title: " See all the files present in a bucket",
                CommandLines: new[] {
                    "qarnot bucket list --name Bucket-name",
                }
            ),
            new Example(
                Title: "See the bucket files present in remote-folder/ inside the bucket \"Bucket-name\"",
                CommandLines: new[] {
                  "qarnot bucket list --folder remote-folder/ --name Bucket-name"
                }
            )
        };

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to list"
        );

        var folderOpt = new Option<string>(
            aliases: new[] { "-f", "--folder" },
            description: "The folder to list"
        );

        var cmd = new CommandWithExamples("list", "List all your buckets, see your buckets info and files")
        {
            examples,
            nameOpt,
            folderOpt,
        };

        cmd.SetHandler(
            model => Factory(model).List(model),
            new ListBucketBinder(
                nameOpt,
                folderOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildGetCommand()
    {
        var examples = new[]
        {
            new Example(
                Title: "Get 2 folder from a bucket",
                CommandLines: new[] {
                    "qarnot bucket get --bucket-folder ./folder1 ./folder2 --name Bucket-name",
                }
            ),
            new Example(
                Title: "Get 2 file from a bucket",
                CommandLines: new[] {
                    "qarnot bucket get --bucket-file ./folder/file1 ./folder/file2 --name Bucket-name",
                }
            ),
            new Example(
                Title: "Get the content of a bucket file and print it",
                CommandLines: new[] {
                    "qarnot bucket get --name Bucket-name --string /folder/file"
                }
            ),
            new Example(
                Title: "Error: missing name",
                CommandLines: new[] {
                    "qarnot bucket get"
                },
                IsError: true
            ),
        };

        var localPathOpt = new Option<string>(
            aliases: new[] { "--local-path", "-l" },
            description: "Local Path folder to send the uploaded files. (default : current path)"
        );

        var bucketFolderOpt = new Option<List<string>>(
            aliases: new[] { "--bucket-folder", "-b" },
            description: "Bucket list of folders to get"
        ) { AllowMultipleArgumentsPerToken = true };

        var bucketFileOpt = new Option<List<string>>(
            aliases: new[] { "--bucket-file", "-f" },
            description: "Bucket list of files to get"
        ) { AllowMultipleArgumentsPerToken = true };

        var stringOpt = new Option<string>(
            aliases: new[] { "--string", "-s" },
            description: "Path of the bucket file to get and print to the terminal"
        );

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to download from"
        ) { IsRequired = true };

        var cmd = new CommandWithExamples("get", "Download a file, folder or string from a bucket")
        {
            examples,
            nameOpt,
            localPathOpt,
            bucketFolderOpt,
            bucketFileOpt,
            stringOpt,
        };

        cmd.SetHandler(
            model => Factory(model).Get(model),
            new GetBucketBinder(
                nameOpt,
                localPathOpt,
                bucketFolderOpt,
                bucketFileOpt,
                stringOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildPutCommand()
    {
        var examples = new[] {
            new Example(
                Title: "Send 2 folders to a bucket folder",
                CommandLines: new[] {
                    "qarnot bucket put --local-folder ./folder1 ./folder2 --name Bucket-name"
                }
            ),
            new Example(
                Title: "Send 2 files to a bucket folder",
                CommandLines: new[] {
                    "qarnot bucket put --local-file ./folder/file1 ./folder/file2 --name Bucket-name"
                }
            ),
            new Example(
                Title: "Send a string to a bucket file",
                CommandLines: new[] {
                    "qarnot bucket put --bucket-path /folder/path/name.txt --name Bucket-name --string \"string to send\""
                }
            ),
            new Example(
                Title: "Error: missing name",
                CommandLines: new[] {
                    "qarnot bucket put"
                },
                IsError: true
            ),
        };

        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to upload to"
        ) { IsRequired = true };

        var localFolderOpt = new Option<List<string>>(
            aliases: new[] { "--local-folder", "-l" },
            description: "List of folders to send"
        ) { AllowMultipleArgumentsPerToken = true };

        var localFileOpt = new Option<List<string>>(
            aliases: new[] { "--local-file", "-f" },
            description: "List of files to send"
        ) { AllowMultipleArgumentsPerToken = true };

        var bucketPathOpt = new Option<string>(
            aliases: new[] { "--bucket-path", "-b" },
            description: "Bucket path must be set for a string.(default:root path)(need to be a file if string set, else, need to be a folder)"
        );

        var stringOpt = new Option<string>(
            aliases: new[] { "--string", "-s" },
            description: "String to send to a bucket"
        );

        var cmd = new CommandWithExamples("put", "Upload a new file, folder or string to a bucket")
        {
            examples,
            nameOpt,
            localFolderOpt,
            localFileOpt,
            bucketPathOpt,
            stringOpt
        };

        cmd.SetHandler(
            model => Factory(model).Put(model),
            new PutBucketBinder(
                nameOpt,
                localFolderOpt,
                localFileOpt,
                bucketPathOpt,
                stringOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSyncFromCommand()
    {
        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to synchronize from"
        ) { IsRequired = true };

        var localPathOpt = new Option<string>(
            aliases: new[] { "--local-path", "-l" },
            description: "Local path folder to send the uploaded files"
        ) { IsRequired = true };

        var bucketFolderOpt = new Option<string>(
            aliases: new[] { "--bucket-folder", "-b" },
            description: "Bucket folder to get"
        );

        var noDeleteOpt = new Option<bool>(
            name: "--no-delete",
            description: "Don't delete local entries not present in remote folder"
        );

        var cmd = new CommandWithExamples("sync-from", "Synchronize your local bucket path from the API")
        {
            nameOpt,
            localPathOpt,
            bucketFolderOpt,
            noDeleteOpt,
        };

        cmd.SetHandler(
            model => Factory(model).SyncFrom(model),
            new SyncBucketBinder(
                nameOpt,
                localPathOpt,
                bucketFolderOpt,
                noDeleteOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildSyncToCommand()
    {
        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to synchronize to"
        ) { IsRequired = true };

        var localPathOpt = new Option<string>(
            aliases: new[] { "--local-path", "-l" },
            description: "Local path folder of files to upload"
        ) { IsRequired = true };

        var bucketFolderOpt = new Option<string>(
            aliases: new[] { "--bucket-folder", "-b" },
            description: "Bucket folder to upload to"
        );

        var noDeleteOpt = new Option<bool>(
            name: "--no-delete",
            description: "Don't delete remote entries not present in local folder"
        );

        var cmd = new CommandWithExamples("sync-to", "Synchronize the API bucket from your local folder")
        {
            nameOpt,
            localPathOpt,
            bucketFolderOpt,
            noDeleteOpt,
        };

        cmd.SetHandler(
            model => Factory(model).SyncTo(model),
            new SyncBucketBinder(
                nameOpt,
                localPathOpt,
                bucketFolderOpt,
                noDeleteOpt,
                GlobalOptions
            )
        );

        return cmd;
    }

    private Command BuildDeleteCommand()
    {
        var nameOpt = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Name of the bucket to delete in"
        ) { IsRequired = true };

        var bucketPathsArg = new Argument<List<string>>(
            name: "paths",
            description: "Bucket list of files and folders to delete"
        );

        var cmd = new CommandWithExamples("delete", "Delete the selected bucket")
        {
            nameOpt,
            bucketPathsArg,
        };

        cmd.SetHandler(
            model => Factory(model).Delete(model),
            new DeleteBucketBinder(
                nameOpt,
                bucketPathsArg,
                GlobalOptions
            )
        );

        return cmd;
    }
}
