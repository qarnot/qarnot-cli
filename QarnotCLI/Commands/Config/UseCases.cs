namespace QarnotCLI;

public interface IConfigUseCases
{
    Task Run(RunConfigModel model);
}

public class ConfigUseCases : IConfigUseCases
{
    private readonly ILogger Logger;

    public ConfigUseCases(ILogger logger)
    {
        Logger = logger;
    }

    public Task Run(RunConfigModel model)
    {
        var path = model.Global
            // If the use of the global configuration file is used, then we should
            // re-evaluate which file to use.
            ? Helpers.GetConnectionConfigurationPath(Logger, forceGlobal: model.Global)
            // Otherwise we use the one we parsed the configuration from.
            : model.ConfigurationFile;

        Logger.Debug($"Updating configuration at {path}");

        var cc = new ConnectionConfiguration
        {
            Token = model.Token,
            ApiUri = model.ApiUri,
            StorageUri = model.StorageUri,
            AccountEmail = model.AccountEmail,
            ForcePathStyle = model.ForcePathStyle,
            DisableBucketPathsSanitization = model.DisableBucketPathsSanitization,
            UnsafeSsl = model.UnsafeSsl,
            StorageUnsafeSsl = model.StorageUnsafeSsl,
        };

        {
            // Make sure the directory exists.
            Directory.CreateDirectory(Path.GetDirectoryName(path!)!);
            using var fstream = File.Open(path!, FileMode.Create);
            using var writer = new StreamWriter(fstream);
            WriteConnectionConfiguration(cc, writer);
        }

        if (model.Show)
        {
            WriteConnectionConfiguration(cc, Console.Out);
        }

        return Task.CompletedTask;
    }

    private void WriteConnectionConfiguration(ConnectionConfiguration cc, TextWriter w)
    {
        w.WriteLine($"token={cc.Token}");
        w.WriteLine($"uri={cc.ApiUri}");
        w.WriteLine($"storage={cc.StorageUri}");
        w.WriteLine($"account-email={cc.AccountEmail}");
        w.WriteLine($"force-path={cc.ForcePathStyle}");
        w.WriteLine($"disable-path-sanitization={cc.DisableBucketPathsSanitization}");
        w.WriteLine($"unsafe-ssl={cc.UnsafeSsl}");
        w.WriteLine($"storage-unsafe-ssl={cc.StorageUnsafeSsl}");
    }
}
