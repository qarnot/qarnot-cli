using System.Threading;
using System.Threading.Tasks;
using QarnotSDK;

namespace QarnotCLI;

public class UpdateSecretCommand : ICommand<QarnotSDK.Secrets, CommandValues.GenericInfoCommandValue>
{
    public async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(
        Secrets secretsClient,
        IConfiguration iconfig,
        CancellationToken ct
    )
    {
        var config = iconfig as UpdateSecretConfiguration;
        await secretsClient.UpdateSecretRawAsync(config.Key, config.Value, ct);

        return new CommandValues.GenericInfoCommandValue()
        {
            Message = $"Updated secret with key {config.Key}",
        };
    }
}

public class DeleteSecretCommand : ICommand<QarnotSDK.Secrets, CommandValues.GenericInfoCommandValue>
{
    public async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(
        Secrets secretsClient,
        IConfiguration iconfig,
        CancellationToken ct
    )
    {
        var config = iconfig as DeleteSecretConfiguration;
        await secretsClient.DeleteSecretAsync(config.Key, ct);

        return new CommandValues.GenericInfoCommandValue()
        {
            Message = $"Deleted secret with key {config.Key}",
        };
    }
}
