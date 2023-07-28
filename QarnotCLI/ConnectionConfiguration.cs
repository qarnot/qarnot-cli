namespace QarnotCLI;

public class ConnectionConfiguration
{
    public string Token { get; set; } = "";
    public string ApiUri { get; set; } = "";
    public string StorageUri { get; set; } = "";
    public string AccountEmail { get; set; } = "";
    public bool ForcePathStyle { get; set; }
    public bool DisableBucketPathsSanitization { get; set; }
    public bool UnsafeSsl { get; set; }
    public bool StorageUnsafeSsl { get; set; }
    public string ConfigurationFile { get; set; } = "";
}

public class ConnectionConfigurationParser
{
    private readonly ILogger Logger;
    public string? ConfigurationFile { get; }

    public ConnectionConfigurationParser(ILogger logger)
    {
        Logger = logger;
        ConfigurationFile = Helpers.GetConnectionConfigurationPath(Logger, forceExist: true);
    }

    public ConnectionConfiguration Parse() =>
        ParseFromEnv(ParseFromFile(new ConnectionConfiguration(), ConfigurationFile));

    private ConnectionConfiguration ParseFromFile(ConnectionConfiguration cc, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return cc;
        }

        Logger.Info($"Parsing configuration from {path}");

        foreach (var line in File.ReadLines(path))
        {
            if (line.Length == 0)
            {
                continue;
            }

            var i = line.IndexOf('=');
            if (i == -1)
            {
                Logger.Error($"Invalid line in configuration:{Environment.NewLine}{line}");
                continue;
            }

            var option = line.Substring(0, i);
            var value = line.Substring(i + 1);

            switch (option)
            {
                case "token":
                    cc.Token = value;
                    break;
                case "uri":
                    cc.ApiUri = value;
                    break;
                case "storage":
                    cc.StorageUri = value;
                    break;
                case "account-email":
                    cc.AccountEmail = value;
                    break;
                case "force-path":
                    cc.ForcePathStyle = SafeParseBool(value);
                    break;
                case "disable-path-sanitization":
                    cc.DisableBucketPathsSanitization = SafeParseBool(value);
                    break;
                case "unsafe-ssl":
                    cc.UnsafeSsl = SafeParseBool(value);
                    break;
                case "storage-unsafe-ssl":
                    cc.StorageUnsafeSsl = SafeParseBool(value);
                    break;
                default:
                    Logger.Warning($"Unknown configuration option in configuration: {option}");
                    break;
            }
        }

        cc.ConfigurationFile = path;
        return cc;
    }

    private ConnectionConfiguration ParseFromEnv(ConnectionConfiguration cc)
    {
        if (Environment.GetEnvironmentVariable("QARNOT_CLIENT_TOKEN") is string token)
        {
            cc.Token = token;
        }

        if (Environment.GetEnvironmentVariable("QARNOT_ACCOUNT_EMAIL") is string accountEmail)
        {
            cc.AccountEmail = accountEmail;
        }

        if (Environment.GetEnvironmentVariable("QARNOT_CLUSTER_URL") is string apiUri)
        {
            cc.ApiUri = apiUri;
        }

        if (Environment.GetEnvironmentVariable("QARNOT_STORAGE_URL") is string storageUri)
        {
            cc.StorageUri = storageUri;
        }

        if (Environment.GetEnvironmentVariable("QARNOT_USE_STORAGE_PATH_STYLE") is string forcePathStyle)
        {
            cc.ForcePathStyle = SafeParseBool(forcePathStyle);
        }

        if (Environment.GetEnvironmentVariable("QARNOT_USE_UNSAFE_SSL") is string unsafeSsl)
        {
            cc.UnsafeSsl = SafeParseBool(unsafeSsl);
        }

        if (Environment.GetEnvironmentVariable("QARNOT_USE_STORAGE_UNSAFE_SSL") is string storageUnsafeSsl)
        {
            cc.StorageUnsafeSsl = SafeParseBool(storageUnsafeSsl);
        }

        return cc;
    }

    private bool SafeParseBool(string s) =>
        string.IsNullOrWhiteSpace(s) ? false : bool.Parse(s);
}
