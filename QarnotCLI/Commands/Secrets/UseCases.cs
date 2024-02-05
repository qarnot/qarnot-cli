using QarnotSDK;

namespace QarnotCLI;

public interface ISecretsUseCases
{
    Task Get(GetSecretModel model);
    Task Create(WriteSecretModel model);
    Task Update(WriteSecretModel model);
    Task Delete(GetSecretModel model);
    Task List(ListSecretsModel model);
}

public class SecretsUseCases : ISecretsUseCases
{
    private readonly Connection QarnotAPI;
    private readonly ILogger Logger;
    private readonly IFormatter Formatter;

    public SecretsUseCases(
        Connection qarnotAPI,
        IFormatter formatter,
        IStateManager _,
        ILogger logger)
    {
        QarnotAPI = qarnotAPI;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task Get(GetSecretModel model)
    {
        Logger.Debug($"Retrieving secret with key {model.Key}");

        var value = await QarnotAPI.Secrets.GetSecretRawAsync(model.Key);
        Logger.Result(Formatter.Format(new {
            Key = model.Key,
            Value = value,
        }));
    }

    public async Task Create(WriteSecretModel model)
    {
        Logger.Debug($"Creating secret with key {model.Key}");

        await QarnotAPI.Secrets.CreateSecretRawAsync(model.Key, model.Value);
        Logger.Result(Formatter.Format(new {
            Message = $"Created secret with key {model.Key}"
        }));
    }

    public async Task Update(WriteSecretModel model)
    {
        Logger.Debug($"Updating secret with key {model.Key}");

        await QarnotAPI.Secrets.UpdateSecretRawAsync(model.Key, model.Value);
        Logger.Result(Formatter.Format(new {
            Message = $"Updated secret with key {model.Key}"
        }));
    }

    public async Task Delete(GetSecretModel model)
    {
        Logger.Debug($"Deleting secret with key {model.Key}");

        await QarnotAPI.Secrets.DeleteSecretAsync(model.Key);
        Logger.Result(Formatter.Format(new {
            Message = $"Deleted secret with key {model.Key}"
        }));
    }

    public async Task List(ListSecretsModel model)
    {
        Logger.Debug($"Listing secrets{(model.Recursive ? " recursively" : "")}{(string.IsNullOrWhiteSpace(model.Prefix) ? "" : $" starting with {model.Prefix}")}");
        var list = await QarnotAPI.Secrets.ListSecretsAsync(model.Prefix, model.Recursive);
        Logger.Result(Formatter.FormatCollection(list.ToList()));
    }
}
