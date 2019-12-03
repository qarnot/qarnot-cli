namespace QarnotCLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    /// <content>
    /// Options Objects of the ParseCommandLine.
    /// </content>
    public partial class Options
    {
        [Verb("bucket create", HelpText = "Create a new bucket.")]
        public class CreateBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Only create the bucket", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new CreateBucketOptions { Name = "Bucket-name" });
                    yield return new Example("Create a bucket and send some files", new[] { UnParserSettings.WithGroupSwitchesOnly() }, new CreateBucketOptions { Name = "Bucket name",  FileList = new List<string>() { "./FileToSend1.py", "./FileToSend2.py" } });
                    yield return new Example("Classic a bucket and send some folders", new[] { UnParserSettings.WithGroupSwitchesOnly() }, new CreateBucketOptions { Name = "Bucket name", FolderList = new List<string>() { "./FolderToSend1", "./FolderToSend2" } });
                    yield return new Example("Logging errors (missing name)", new CreateBucketOptions { });
                }
            }

            [Option("files", Required = false, HelpText = "File or folder to send to the bucket.")]
            public IEnumerable<string> FileList { get; set; }

            [Option("folder", Required = false, HelpText = "File or folder to send to the bucket.")]
            public IEnumerable<string> FolderList { get; set; }
        }

        [Verb("bucket set", HelpText = "Upload a new file, folder or string to a bucket.")]
        public class SetBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Send 2 folder to a bucket folder", UnParserSettings.WithGroupSwitchesOnly(), new SetBucketOptions { Name = "Bucket-name", LocalFoldersPath = new string[] { "./folder1", "./folder2" } });
                    yield return new Example("Send 2 file to a bucket folder", UnParserSettings.WithGroupSwitchesOnly(), new SetBucketOptions { Name = "Bucket-name", LocalFilesPath = new string[] { "./folder/file1", "./folder/file2" } });
                    yield return new Example("Send a string to a bucket file", UnParserSettings.WithGroupSwitchesOnly(), new SetBucketOptions { Name = "Bucket-name", String = "string to send", RemoteFolderPath = "/folder/path/name.txt" });
                    yield return new Example("Logging errors (missing name)", new SetBucketOptions { });
                }
            }

            [Option('l', "local-folder", Group="send", Required = false, HelpText = "List of files to send.")]
            public IEnumerable<string> LocalFoldersPath { get; set; }

            [Option('f', "local-file", Group="send", Required = false, HelpText = "List of folders to send.")]
            public IEnumerable<string> LocalFilesPath { get; set; }

            [Option('b', "bucket-path", Required = false, HelpText = "Bucket path must be set for a string.(default:root path)(need to be a file if string set, else, need to be a folder)")]
            public string RemoteFolderPath { get; set; }

            [Option('s', "string", Group="send", Required = false, HelpText = "String to send to a bucket.")]
            public string String { get; set; }
        }

        [Verb("bucket get", HelpText = "Download a new file, folder or string from a bucket.")]
        public class GetBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Get 2 folder from a bucket", UnParserSettings.WithGroupSwitchesOnly(), new GetBucketOptions { Name = "Bucket-name", RemoteFoldersPath = new string[] { "./folder1", "./folder2" } });
                    yield return new Example("Get 2 file from a bucket", UnParserSettings.WithGroupSwitchesOnly(), new GetBucketOptions { Name = "Bucket-name", RemoteFilesPath = new string[] { "./folder/file1", "./folder/file2" } });
                    yield return new Example("Get the content of a bucket file and print it", UnParserSettings.WithGroupSwitchesOnly(), new GetBucketOptions { Name = "Bucket-name", String = "/folder/file" });
                    yield return new Example("Logging errors (missing name)", new GetBucketOptions { });
                }
            }

            [Option('l', "local-path", Required = false, HelpText = "Local Path folder to send the uploaded files. (default : current path)")]
            public string LocalPath { get; set; }

            [Option('b', "bucket-folder", Group = "Download", Required = false, HelpText = "Bucket list of folders to get.")]
            public IEnumerable<string> RemoteFoldersPath { get; set; }

            [Option('f', "bucket-file", Group = "Download", Required = false, HelpText = "Bucket list of files to get.")]
            public IEnumerable<string> RemoteFilesPath { get; set; }

            [Option('s', "string", Group = "Download", Required = false, HelpText = "Path of the bucket file to get and print in the terminal.")]
            public string String { get; set; }
        }

        [Verb("bucket sync-from", HelpText = "Synchronize your local bucket path from the api.")]
        public class SyncFromBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Synchronize the bucket from the api.", UnParserSettings.WithGroupSwitchesOnly(), new SyncFromBucketOptions { Name = "Bucket-name" });
                    yield return new Example("Logging errors (missing name).", new SyncFromBucketOptions { });
                }
            }

            [Option('l', "local-path", Required = true, HelpText = "Local Path folder to send the uploaded files. ")]
            public string LocalPath { get; set; }

            [Option('b', "bucket-folder", Required = false, HelpText = "Bucket list of folders to get.")]
            public string RemoteFolderPath { get; set; }
        }

        [Verb("bucket sync-to", HelpText = "Synchronize the api bucket from your local folder.")]
        public class SyncToBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Synchronize the bucket to the api", UnParserSettings.WithGroupSwitchesOnly(), new SyncToBucketOptions { Name = "Bucket-name" });
                    yield return new Example("Logging errors (missing name)", new SyncToBucketOptions { });
                }
            }

            [Option('l', "local-path", Required = true, HelpText = "Local Path folder to send the uploaded files. ")]
            public string LocalPath { get; set; }

            [Option('b', "bucket-folder", Required = false, HelpText = "Bucket list of folders to get.")]
            public string RemoteFolderPath { get; set; }
        }

        [Verb("bucket list", HelpText = "List all your buckets, see your bucket info and files.")]
        public class ListBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("List the bucket of the api.", UnParserSettings.WithGroupSwitchesOnly(), new ListBucketOptions { });
                    yield return new Example("See all the files present in a bucket.", UnParserSettings.WithGroupSwitchesOnly(), new ListBucketOptions { Name = "Bucket-name" });
                    yield return new Example("See the bucket files present in remote-folder/ inside the bucket \"Bucket-name\".", UnParserSettings.WithGroupSwitchesOnly(), new ListBucketOptions { Name = "Bucket-name", RemoteFolderPath = "remote-folder/" });
                }
            }

            [Option('n', "name", Required = false, HelpText = "Name of the bucket to list.")]
            public override string Name { get; set; }

            [Option('f', "folder", Required = false, HelpText = "The folder to list.")]
            public string RemoteFolderPath { get; set; }
        }

        [Verb("bucket delete", HelpText = "Delete the selected bucket.")]
        public class TerminateBucketOptions : ABucketGetOptions
        {
            [Usage(ApplicationAlias = "qarnot")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Terminate and delete the bucket to the api", UnParserSettings.WithGroupSwitchesOnly(), new TerminateBucketOptions { Name = "Bucket-name" });
                    yield return new Example("Logging errors (missing name)", new TerminateBucketOptions { });
                }
            }
        }
    }
}