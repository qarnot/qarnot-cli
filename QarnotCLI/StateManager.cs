namespace QarnotCLI;


public record PageToken(string Token);

public interface IStateManager
{
    PageToken GetNextPageToken();
    bool SaveNextPageToken(PageToken pageToken);
}

public interface IStateManagerFactory
{
    IStateManager Create(ILogger logger);
}

public class StateManagerFactory : IStateManagerFactory
{
    public IStateManager Create(ILogger logger) => new StateManager(logger);
}

public class StateManager : IStateManager
{
    private const string NextPageTokenKey = "next-page-token";
    private readonly bool DoNotPersistPageToken;
    private readonly string ConfigurationFile;
    private readonly ILogger Logger;

    public StateManager()
    {
    }

    public StateManager(ILogger logger) : this()
    {
        Logger = logger;
        ConfigurationFile = Helpers
            .GetConnectionConfigurationPath(logger, forceExist: true)
            ?? string.Empty;
    }

    public PageToken GetNextPageToken()
    {
        if(DoNotPersistPageToken)
        {
            return default;
        }

        return new PageToken(ReadNextPageTokenFromFile());
    }

    public bool SaveNextPageToken(PageToken pageToken)
    {
        if (DoNotPersistPageToken)
        {
            return false;
        }

        WriteNextPageTokenToFile(pageToken?.Token ?? string.Empty);

        return true;
    }

    private void WriteNextPageTokenToFile(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ConfigurationFile))
            {
                return;
            }

            bool nextTokenUpdated = false;
            var newConfigurationLines = new List<string>();
            foreach (var line in File.ReadLines(ConfigurationFile))
            {
                if (line.Length == 0)
                {
                    newConfigurationLines.Add(line);
                    continue;
                }

                var i = line.IndexOf('=');
                if (i == -1)
                {
                    newConfigurationLines.Add(line);
                    continue;
                }

                var option = line.Substring(0, i);
                var value = line.Substring(i + 1);

                if (option == NextPageTokenKey)
                {
                    nextTokenUpdated = true;
                    newConfigurationLines.Add($"{NextPageTokenKey}={token ?? string.Empty}");
                    continue;
                }

                newConfigurationLines.Add(line);
            }

            if (!nextTokenUpdated)
            {
                newConfigurationLines.Add($"{NextPageTokenKey}={token}");
            }

            File.WriteAllLines(ConfigurationFile, newConfigurationLines);
        }
        catch (Exception exception)
        {
            Logger.Warning($"Can't save pagination token: '--next-page' won't work (Details: {exception.Message})");
        }
    }

    private string ReadNextPageTokenFromFile()
    {
        if (string.IsNullOrWhiteSpace(ConfigurationFile))
        {
            return string.Empty;
        }

        return File
            .ReadLines(ConfigurationFile)
            .Where(line => line.StartsWith(NextPageTokenKey))
            .Select(line =>
            {
                var i = line.IndexOf('=');
                var option = line.Substring(0, i);
                return line.Substring(i + 1);
            })
            .FirstOrDefault()
            ?? string.Empty;
    }
}
