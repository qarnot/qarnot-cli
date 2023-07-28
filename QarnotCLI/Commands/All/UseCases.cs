namespace QarnotCLI;

public interface IAllUseCases
{
    Task Abort(GlobalModel options);
    Task List(GlobalModel options);
    Task Delete(GlobalModel options);
}

public class AllUseCases : IAllUseCases
{
    private readonly ITaskUseCases TaskUseCases;
    private readonly IJobUseCases JobUseCases;
    private readonly IPoolUseCases PoolUseCases;
    private readonly IBucketUseCases BucketUseCases;

    private readonly ILogger Logger;

    public AllUseCases(
        ITaskUseCases taskUseCases,
        IPoolUseCases poolUseCases,
        IJobUseCases jobUseCases,
        IBucketUseCases bucketUseCases,
        ILogger logger
    )
    {
        TaskUseCases = taskUseCases;
        PoolUseCases = poolUseCases;
        JobUseCases = jobUseCases;
        BucketUseCases = bucketUseCases;

        Logger = logger;
    }

    public async Task Abort(GlobalModel options)
    {
        Logger.Debug("Aborting all tasks");
        await TaskUseCases.Abort(new());
        Logger.Debug("Aborting all jobs");
        await JobUseCases.Abort(new(All: true, null, null));
    }

    public async Task Delete(GlobalModel options)
    {
        Logger.Debug("Deleting all tasks");
        await TaskUseCases.Delete(new());
        Logger.Debug("Deleting all pools");
        await PoolUseCases.Delete(new());
        Logger.Debug("Deleting all jobs");
        await JobUseCases.Delete(new(All: true, null, null));
        Logger.Debug("Deleting all buckets");
        await BucketUseCases.DeleteAll();
    }

    public async Task List(GlobalModel options)
    {
        Logger.Debug("Listing all tasks");
        await TaskUseCases.List(new());
        Logger.Debug("Listing all pools");
        await PoolUseCases.List(new());
        Logger.Debug("Listing all jobs");
        await JobUseCases.List(new(All: true, null, null));
        Logger.Debug("Listing all buckets");
        await BucketUseCases.List(new(null, null));
    }
}
