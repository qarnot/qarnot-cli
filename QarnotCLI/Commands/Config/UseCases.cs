using System.Text;

namespace QarnotCLI;

public interface IConfigUseCases
{
    Task SetConfig(SetConfigModel model);
    Task ShowConfig(ShowConfigModel model);
}

public class ConfigUseCases : IConfigUseCases
{
    private readonly ILogger Logger;

    public ConfigUseCases(ILogger logger)
    {
        Logger = logger;
    }

    public Task SetConfig(SetConfigModel model)
    {
        var path = Helpers.GetConnectionConfigurationPath(Logger, forceGlobal: !model.Local, forceExist: false);

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

    public Task ShowConfig(ShowConfigModel model)
    {
        var configParser = new ConnectionConfigurationParser(Logger, forceExist: true, forceGlobal: model.Global);
        var cc = model.WithoutEnv ? configParser.ParseRawConfigFile() : configParser.Parse();

        var sb = new StringBuilder();
        sb.Append($"Showing {(model.Global ? "global " : "" )}configuration");
        if (!string.IsNullOrWhiteSpace(cc.ConfigurationFile))
            sb.Append($" located at {cc.ConfigurationFile}. ");
        else
            sb.Append(". ");
        if (model.WithoutEnv)
            sb.Append("Does not include parameters passed by environment variables.");
        else
            sb.Append("Includes parameters passed by environment variables.");

        Logger.Info(sb.ToString());


        WriteConnectionConfiguration(cc, Console.Out);

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
