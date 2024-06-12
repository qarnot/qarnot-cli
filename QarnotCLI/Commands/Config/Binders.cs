using System.CommandLine;
using System.CommandLine.Binding;

namespace QarnotCLI;

public class SetConfigBinder : GlobalBinder<SetConfigModel>
{
    private readonly Option<bool> LocalOpt;
    private readonly Option<bool> ShowOpt;
    private readonly Option<string> ApiUriOpt;
    private readonly Option<string> StorageUriOpt;
    private readonly Option<string> AccountEmailOpt;
    private readonly Option<bool?> ForceStoragePathStyleOpt;
    private readonly Option<bool?> NoSanitizeBucketPathOpt;
    private readonly Option<bool?> StorageUnsafeSslOpt;

    public SetConfigBinder(
        Option<bool> localOpt,
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
        LocalOpt = localOpt;
        ShowOpt = showOpt;
        ApiUriOpt = apiUriOpt;
        StorageUriOpt = storageApiUriOpt;
        AccountEmailOpt = accountEmailOpt;
        ForceStoragePathStyleOpt = forceStoragePathStyleOpt;
        NoSanitizeBucketPathOpt = noSanitizeBucketPathOpt;
        StorageUnsafeSslOpt = storageUnsafeSslOpt;
    }

    protected override SetConfigModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(LocalOpt),
            bindingContext.ParseResult.GetValueForOption(ShowOpt)
        );

    protected override SetConfigModel GetBoundValue(BindingContext bindingContext)
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

public class ShowConfigBinder : GlobalBinder<ShowConfigModel>
{
    private readonly Option<bool> ShowGlobalConfigOpt;
    private readonly Option<bool> WithoutEnvOpt;

    public ShowConfigBinder(
        Option<bool> showGlobalConfigOpt,
        Option<bool> withoutEnvOpt,
        GlobalOptions globalOptions
    ): base(globalOptions)
    {
        ShowGlobalConfigOpt = showGlobalConfigOpt;
        WithoutEnvOpt = withoutEnvOpt;
    }

    protected override ShowConfigModel GetBoundValueImpl(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(ShowGlobalConfigOpt),
            bindingContext.ParseResult.GetValueForOption(WithoutEnvOpt)
        );
}