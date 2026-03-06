using System.CommandLine;

namespace QarnotCLI;

public abstract class ModelBinder<T>
{
    public abstract T GetBoundValue(ParseResult parseResult);
}

public abstract class GlobalBinder<T> : ModelBinder<T>
    where T: GlobalModel
{
    private readonly GlobalOptions GlobalOptions;

    public GlobalBinder(GlobalOptions options)
    {
        GlobalOptions = options;
    }

    protected abstract T GetBoundValueImpl(ParseResult parseResult);

    public override T GetBoundValue(ParseResult parseResult)
    {
        var value = GetBoundValueImpl(parseResult);
        value.Initialize(
            token: parseResult.GetValue(GlobalOptions.TokenOpt) ?? GlobalOptions.ConnectionConfiguration.Token,
            apiUri: GlobalOptions.ConnectionConfiguration.ApiUri,
            storageUri: GlobalOptions.ConnectionConfiguration.StorageUri,
            accountEmail: GlobalOptions.ConnectionConfiguration.AccountEmail,
            unsafeSsl: parseResult.GetValue(GlobalOptions.UnsafeSslOpt) ?? GlobalOptions.ConnectionConfiguration.UnsafeSsl,
            storageUnsafeSsl: GlobalOptions.ConnectionConfiguration.StorageUnsafeSsl,
            customSslCertificat: parseResult.GetValue(GlobalOptions.CustomSslCertificateOpt) ?? "",
            verbose: parseResult.GetValue(GlobalOptions.VerboseOpt),
            quiet: parseResult.GetValue(GlobalOptions.QuietOpt),
            noColor: parseResult.GetValue(GlobalOptions.NoColorOpt),
            format: parseResult.GetValue(GlobalOptions.FormatOpt),
            humanReadable: parseResult.GetValue(GlobalOptions.HumanReadableOpt),
            forcePathStyle: GlobalOptions.ConnectionConfiguration.ForcePathStyle,
            disableBucketPathsSanitization: GlobalOptions.ConnectionConfiguration.DisableBucketPathsSanitization,
            noPersistedNextPageToken: GlobalOptions.ConnectionConfiguration.NoPersistedNextPageToken,
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

    protected override GlobalModel GetBoundValueImpl(ParseResult parseResult) =>
        new GlobalModel();
}
