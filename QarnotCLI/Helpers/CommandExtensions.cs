using System.CommandLine;

namespace QarnotCLI;

public static class CommandExtensions
{
    public static void SetModelAction<T>(
        this Command cmd,
        Func<T, Task> handler,
        ModelBinder<T> binder
    )
    {
        cmd.SetAction(async parseResult =>
        {
            var model = binder.GetBoundValue(parseResult);
            await handler(model);
        });
    }

    public static void SetModelAction<T>(
        this Command cmd,
        Action<T> handler,
        ModelBinder<T> binder
    )
    {
        cmd.SetAction(parseResult =>
        {
            var model = binder.GetBoundValue(parseResult);
            handler(model);
        });
    }
}
