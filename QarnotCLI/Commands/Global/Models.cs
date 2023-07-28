namespace QarnotCLI;

public record GlobalModel
{
    public string Token { get; set; } = "";
    public string ApiUri { get; set; } = "";
    public string StorageUri { get; set; } = "";
    public string AccountEmail { get; set; } = "";
    public bool UnsafeSsl { get; set; }
    public bool StorageUnsafeSsl { get; set; }
    public string CustomSslCertificat { get; private set; } = "";
    public bool Verbose { get; private set; }
    public bool Quiet { get; private set; }
    public bool NoColor { get; private set; }
    public string Format { get; private set; }
    public bool HumanReadable { get; private set; }
    public bool ForcePathStyle { get; set; }
    public bool DisableBucketPathsSanitization { get; set; }
    public string ConfigurationFile { get; set; } = "";

    public GlobalModel()
    {
        Format = "TABLE";
    }

    public GlobalModel Initialize(
        string token,
        string apiUri,
        string storageUri,
        string accountEmail,
        bool unsafeSsl,
        bool storageUnsafeSsl,
        string customSslCertificat,
        bool verbose,
        bool quiet,
        bool noColor,
        string? format,
        bool humanReadable,
        bool forcePathStyle,
        bool disableBucketPathsSanitization,
        string configurationFile
    )
    {
        Token = token;
        ApiUri = apiUri;
        StorageUri = storageUri;
        UnsafeSsl = unsafeSsl;
        StorageUnsafeSsl = storageUnsafeSsl;
        CustomSslCertificat = customSslCertificat;
        Verbose = verbose;
        Quiet = quiet;
        NoColor = noColor;

        if (format is not null)
        {
            Format = format;
        }

        HumanReadable = humanReadable;
        ForcePathStyle = forcePathStyle;
        DisableBucketPathsSanitization = disableBucketPathsSanitization;
        ConfigurationFile = configurationFile;

        return this;
    }
}
