namespace QarnotCLI;

public record CreateBucketModel(
    string Name,
    List<string> Files,
    List<string> Folders
) : GlobalModel;

public record ListBucketModel(
    string? Name,
    string? Folder
) : GlobalModel;

public record GetBucketModel(
    string Name,
    string? LocalPath,
    List<string> BucketFolders,
    List<string> BucketFiles,
    string? String
) : GlobalModel;

public record PutBucketModel(
    string Name,
    List<string> LocalFolders,
    List<string> LocalFiles,
    string? BucketPath,
    string? Send
) : GlobalModel;

public record SyncBucketModel(
    string Name,
    string LocalPath,
    string? BucketFolder,
    bool NoDelete
) : GlobalModel;

public record DeleteBucketModel(
    string Name,
    List<string> BucketPaths
) : GlobalModel;

public record BucketSummary(
    string Shortname,
    uint TotalFileCount,
    string TotalBytes,
    List<string> Entries
);
