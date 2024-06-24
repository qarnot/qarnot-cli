using System.CommandLine;
using System.CommandLine.Binding;

namespace QarnotCLI;

public class CreateBucketBinder : GlobalBinder<CreateBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<List<string>> FilesOpt;
    private readonly Option<List<string>> FoldersOpt;

    public CreateBucketBinder(
        Option<string> nameOpt,
        Option<List<string>> filesOpt,
        Option<List<string>> foldersOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        FilesOpt = filesOpt;
        FoldersOpt = foldersOpt;
    }

    protected override CreateBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt)!,
            bindingContext.ParseResult.GetValueForOption(FilesOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(FoldersOpt) ?? new()
        );
}

public class ListBucketBinder : GlobalBinder<ListBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<string> FolderOpt;

    public ListBucketBinder(
        Option<string> nameOpt,
        Option<string> folderOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        FolderOpt = folderOpt;
    }

    protected override ListBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt),
            bindingContext.ParseResult.GetValueForOption(FolderOpt)
        );
}

public class GetBucketBinder : GlobalBinder<GetBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<string> LocalPathOpt;
    private readonly Option<List<string>> BucketFoldersOpt;
    private readonly Option<List<string>> BucketFilesOpt;
    private readonly Option<string> StringOpt;

    public GetBucketBinder(
        Option<string> nameOpt,
        Option<string> localPathOpt,
        Option<List<string>> bucketFoldersOpt,
        Option<List<string>> bucketFilesOpt,
        Option<string> stringOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        LocalPathOpt = localPathOpt;
        BucketFoldersOpt = bucketFoldersOpt;
        BucketFilesOpt = bucketFilesOpt;
        StringOpt = stringOpt;
    }

    protected override GetBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt)!,
            bindingContext.ParseResult.GetValueForOption(LocalPathOpt),
            bindingContext.ParseResult.GetValueForOption(BucketFoldersOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(BucketFilesOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(StringOpt)
        );
}

public class PutBucketBinder : GlobalBinder<PutBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<List<string>> LocalFoldersOpt;
    private readonly Option<List<string>> LocalFilesOpt;
    private readonly Option<string> BucketPathOpt;
    private readonly Option<string> SendOpt;

    public PutBucketBinder(
        Option<string> nameOpt,
        Option<List<string>> localFoldersOpt,
        Option<List<string>> localFilesOpt,
        Option<string> bucketPathOpt,
        Option<string> sendOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        LocalFoldersOpt = localFoldersOpt;
        LocalFilesOpt = localFilesOpt;
        BucketPathOpt = bucketPathOpt;
        SendOpt = sendOpt;
    }

    protected override PutBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt)!,
            bindingContext.ParseResult.GetValueForOption(LocalFoldersOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(LocalFilesOpt) ?? new(),
            bindingContext.ParseResult.GetValueForOption(BucketPathOpt),
            bindingContext.ParseResult.GetValueForOption(SendOpt)
        );
}

public class SyncBucketBinder : GlobalBinder<SyncBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Option<string> LocalPathOpt;
    private readonly Option<string> BucketFolderOpt;
    private readonly Option<bool> NoDeleteOpt;

    public SyncBucketBinder(
        Option<string> nameOpt,
        Option<string> localPathOpt,
        Option<string> bucketFolderOpt,
        Option<bool> noDeleteOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        LocalPathOpt = localPathOpt;
        BucketFolderOpt = bucketFolderOpt;
        NoDeleteOpt = noDeleteOpt;
    }

    protected override SyncBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt)!,
            bindingContext.ParseResult.GetValueForOption(LocalPathOpt)!,
            bindingContext.ParseResult.GetValueForOption(BucketFolderOpt),
            bindingContext.ParseResult.GetValueForOption(NoDeleteOpt)
        );
}

public class DeleteBucketBinder : GlobalBinder<DeleteBucketModel>
{
    private readonly Option<string> NameOpt;
    private readonly Argument<List<string>> BucketPathsArg;

    public DeleteBucketBinder(
        Option<string> nameOpt,
        Argument<List<string>> bucketPathsArg,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        NameOpt = nameOpt;
        BucketPathsArg = bucketPathsArg;
    }

    protected override DeleteBucketModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(NameOpt)!,
            bindingContext.ParseResult.GetValueForArgument(BucketPathsArg) ?? new()
        );
}
