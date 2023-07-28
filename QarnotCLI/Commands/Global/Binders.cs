using System.CommandLine.Binding;

namespace QarnotCLI;

public abstract class GlobalBinder<T> : BinderBase<T>
    where T: GlobalModel
{
    private readonly GlobalOptions GlobalOptions;

    public GlobalBinder(GlobalOptions options)
    {
        GlobalOptions = options;
    }

    protected abstract T GetBoundValueImpl(BindingContext bindingContext);

    protected override T GetBoundValue(BindingContext bindingContext)
    {
        var value = GetBoundValueImpl(bindingContext);
        value.Initialize(
            token: bindingContext.ParseResult.GetValueForOption(GlobalOptions.TokenOpt) ?? GlobalOptions.ConnectionConfiguration.Token,
            apiUri: GlobalOptions.ConnectionConfiguration.ApiUri,
            storageUri: GlobalOptions.ConnectionConfiguration.StorageUri,
            accountEmail: GlobalOptions.ConnectionConfiguration.AccountEmail,
            unsafeSsl: bindingContext.ParseResult.GetValueForOption(GlobalOptions.UnsafeSslOpt) ?? GlobalOptions.ConnectionConfiguration.UnsafeSsl,
            storageUnsafeSsl: GlobalOptions.ConnectionConfiguration.StorageUnsafeSsl,
            customSslCertificat: bindingContext.ParseResult.GetValueForOption(GlobalOptions.CustomSslCertificateOpt) ?? "",
            verbose: bindingContext.ParseResult.GetValueForOption(GlobalOptions.VerboseOpt),
            quiet: bindingContext.ParseResult.GetValueForOption(GlobalOptions.QuietOpt),
            noColor: bindingContext.ParseResult.GetValueForOption(GlobalOptions.NoColorOpt),
            format: bindingContext.ParseResult.GetValueForOption(GlobalOptions.FormatOpt),
            humanReadable: bindingContext.ParseResult.GetValueForOption(GlobalOptions.HumanReadableOpt),
            forcePathStyle: GlobalOptions.ConnectionConfiguration.ForcePathStyle,
            disableBucketPathsSanitization: GlobalOptions.ConnectionConfiguration.DisableBucketPathsSanitization,
            configurationFile: GlobalOptions.ConnectionConfiguration.ConfigurationFile
        );
        return value;
    }
}

public class GlobalBinder : GlobalBinder<GlobalModel>
{
    public GlobalBinder(GlobalOptions options)
        : base(options)
    {
    }

    protected override GlobalModel GetBoundValueImpl(BindingContext bindingContext) =>
        new GlobalModel();
}
