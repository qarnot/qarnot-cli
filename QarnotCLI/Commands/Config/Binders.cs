using System.CommandLine;
using System.CommandLine.Binding;

namespace QarnotCLI;

public class RunConfigBinder : GlobalBinder<RunConfigModel>
{
    private readonly Option<bool> GlobalOpt;
    private readonly Option<bool> ShowOpt;
    private readonly Option<string> ApiUriOpt;
    private readonly Option<string> StorageUriOpt;
    private readonly Option<string> AccountEmailOpt;
    private readonly Option<bool?> ForceStoragePathStyleOpt;
    private readonly Option<bool?> NoSanitizeBucketPathOpt;
    private readonly Option<bool?> StorageUnsafeSslOpt;

    public RunConfigBinder(
        Option<bool> globalOpt,
        Option<bool> showOpt,
        Option<string> apiUriOpt,
        Option<string> storageApiUriOpt,
        Option<string> accountEmailOpt,
        Option<bool?> forceStoragePathStyleOpt,
        Option<bool?> noSanitizeBucketPathOpt,
        Option<bool?> storageUnsafeSslOpt,
        GlobalOptions globalOptions
    ) : base(globalOptions)
    {
        GlobalOpt = globalOpt;
        ShowOpt = showOpt;
        ApiUriOpt = apiUriOpt;
        StorageUriOpt = storageApiUriOpt;
        AccountEmailOpt = accountEmailOpt;
        ForceStoragePathStyleOpt = forceStoragePathStyleOpt;
        NoSanitizeBucketPathOpt = noSanitizeBucketPathOpt;
        StorageUnsafeSslOpt = storageUnsafeSslOpt;
    }

    protected override RunConfigModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(GlobalOpt),
            bindingContext.ParseResult.GetValueForOption(ShowOpt)
        );

    protected override RunConfigModel GetBoundValue(BindingContext bindingContext)
    {
        var model = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.GetValueForOption(ApiUriOpt) is string apiUri)
        {
            model.ApiUri = apiUri;
        }

        if (bindingContext.ParseResult.GetValueForOption(StorageUriOpt) is string storageUri)
        {
            model.StorageUri = storageUri;
        }

        if (bindingContext.ParseResult.GetValueForOption(AccountEmailOpt) is string accountEmail)
        {
            model.AccountEmail = accountEmail;
        }

        if (bindingContext.ParseResult.GetValueForOption(ForceStoragePathStyleOpt) is bool forcePathStyle)
        {
            model.ForcePathStyle = forcePathStyle;
        }

        if (bindingContext.ParseResult.GetValueForOption(NoSanitizeBucketPathOpt) is bool noSanitizeBucketPath)
        {
            model.DisableBucketPathsSanitization = noSanitizeBucketPath;
        }

        if (bindingContext.ParseResult.GetValueForOption(StorageUnsafeSslOpt) is bool storageUnsafeSsl)
        {
            model.StorageUnsafeSsl = storageUnsafeSsl;
        }

        return model;
    }
}
