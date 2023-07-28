using QarnotSDK;

namespace QarnotCLI;

public class UseCasesFactory
{
    private readonly IQarnotAPIFactory APIFactory;
    private readonly IFormatterFactory FormatterFactory;
    public ILoggerFactory LoggerFactory { get; }

    public UseCasesFactory(
        IQarnotAPIFactory apiFactory,
        IFormatterFactory formatterFactory,
        ILoggerFactory loggerFactory
    )
    {
        APIFactory = apiFactory;
        FormatterFactory = formatterFactory;
        LoggerFactory = loggerFactory;
    }

    public TUseCases Create<TUseCases>(GlobalModel options)
    {
        var ctor = typeof(TUseCases).GetConstructor(
            new[] { typeof(Connection), typeof(IFormatter), typeof(ILogger) }
        );

        if (ctor is null)
        {
            throw new Exception($"Could not find a suitable constructor for {typeof(TUseCases).Name}");
        }

        return (TUseCases)ctor.Invoke(
            new object?[] {
                APIFactory.Create(options),
                FormatterFactory.Create(options.Format),
                LoggerFactory.Create(options)
            }
        );
    }
}
