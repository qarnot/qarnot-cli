using QarnotSDK;

namespace QarnotCLI;

public class UseCasesFactory
{
    private readonly IQarnotAPIFactory APIFactory;
    private readonly IFormatterFactory FormatterFactory;
    private readonly IStateManagerFactory StateManagerFactory;
    public ILoggerFactory LoggerFactory { get; }

    public UseCasesFactory(
        IQarnotAPIFactory apiFactory,
        IFormatterFactory formatterFactory,
        IStateManagerFactory stateManagerFactory,
        ILoggerFactory loggerFactory
    )
    {
        APIFactory = apiFactory;
        FormatterFactory = formatterFactory;
        StateManagerFactory = stateManagerFactory;
        LoggerFactory = loggerFactory;
    }

    public TUseCases Create<TUseCases>(GlobalModel options)
    {
        var ctor = typeof(TUseCases).GetConstructor(
            new[]
            {
                typeof(Connection),
                typeof(IFormatter),
                typeof(IStateManager),
                typeof(ILogger)
            }
        );

        if (ctor is null)
        {
            throw new Exception($"Could not find a suitable constructor for {typeof(TUseCases).Name}");
        }

        var logger = LoggerFactory.Create(options);

        return (TUseCases)ctor.Invoke(
            new object?[] {
                APIFactory.Create(options),
                FormatterFactory.Create(options.Format),
                StateManagerFactory.Create(logger),
                logger
            }
        );
    }
}
