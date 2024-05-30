using QarnotSDK;

namespace QarnotCLI;

public interface IHardwareConstraintsUseCases
{
    Task List(GlobalModel model);
}

public class HardwareConstraintsUseCases : IHardwareConstraintsUseCases
{
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly ILogger Logger;

    public HardwareConstraintsUseCases(Connection api, IFormatter formatter, IStateManager _, ILogger logger)
    {
        QarnotAPI = api;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task List(GlobalModel model)
    {
        Logger.Debug("Retrieving hardware constraints");
        var constraints = await QarnotAPI.RetrieveUserHardwareConstraintsAsync();

        Logger.Result(Formatter.FormatCollection(constraints.ToList()));
    }
}
