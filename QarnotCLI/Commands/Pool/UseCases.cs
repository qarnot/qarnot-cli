using QarnotSDK;
using Newtonsoft.Json;

namespace QarnotCLI;

public interface IPoolUseCases
{
    Task Create(CreatePoolModel model);
    Task List(GetPoolsOrTasksModel model);
    Task Info(GetPoolsOrTasksModel model);
    Task Delete(GetPoolsOrTasksModel model);
    Task UpdateElasticSettings(UpdatePoolElasticSettingsModel model);
    Task UpdateScaling(UpdatePoolScalingModel model);
    Task UpdateResources(GetPoolsOrTasksModel model);
    Task UpdateConstant(UpdatePoolsOrTasksConstantModel model);
}

public class PoolUseCases : IPoolUseCases
{
    private readonly ILogger Logger;
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly JsonConverter[] Converters;

    public PoolUseCases(Connection qarnotAPI, IFormatter formatter, ILogger logger)
    {
        QarnotAPI = qarnotAPI;
        Logger = logger;
        Formatter = formatter;
        Converters = new JsonConverter[] {
            new TimePeriodSpecificationJsonConverter(),
            new ScalingPolicyConverter(),
        };
    }

    public async Task Create(CreatePoolModel model)
    {
        Logger.Debug("Creating pool");
        var pool = await CreatePool(model);
        Logger.Debug("Starting pool");
        await pool.StartAsync();

        Logger.Result(Formatter.Format(new {
            Pool = pool.Uuid,
            State = "Created"
        }));
    }

    private async Task<QPool> CreatePool(CreatePoolModel model)
    {
        var pool = QarnotAPI.CreatePool(model.Name, model.Profile, model.InstanceCount, model.Shortname, model.TasksWaitForSynchronization);
        pool.SetTags(model.Tags.ToArray());

        pool.Resources = (await Task.WhenAll(
            model.Resources.Select(async r => await QarnotAPI.CreateBucketAsync(r) as QAbstractStorage)
        )).ToList();

        foreach (var constant in model.Constants)
        {
            if (Helpers.ParseKeyValuePair(constant, out var kvp))
            {
                pool.SetConstant(kvp.Key, kvp.Value);
            }
        }

        foreach (var constraint in model.Constraints)
        {
            if (Helpers.ParseKeyValuePair(constraint, out var kvp))
            {
                pool.SetConstraint(kvp.Key, kvp.Value);
            }
        }

        foreach (var label in model.Labels)
        {
            if (Helpers.ParseKeyValuePair(label, out var kvp))
            {
                pool.SetLabel(kvp.Key, kvp.Value);
            }
        }

        pool.IsElastic = model.IsElastic;
        pool.ElasticMinimumTotalSlots = model.ElasticMinSlots ?? pool.ElasticMinimumTotalSlots;
        pool.ElasticMaximumTotalSlots = model.ElasticMaxSlots ?? pool.ElasticMaximumTotalSlots;
        pool.ElasticMinimumIdlingSlots = model.ElasticMinIdlingSlots ?? pool.ElasticMinimumIdlingSlots;
        pool.ElasticResizePeriod = model.ElasticResizePeriod ?? pool.ElasticResizePeriod;
        pool.ElasticResizeFactor = model.ElasticResizeFactor ?? pool.ElasticResizeFactor;
        pool.ElasticMinimumIdlingTime = model.ElasticMinIdlingTime ?? pool.ElasticMinimumIdlingTime;
        pool.Scaling = model.Scaling ?? pool.Scaling;

        if (model.ExportCredentialsToEnv.HasValue)
        {
            pool.Privileges.ExportApiAndStorageCredentialsInEnvironment = model.ExportCredentialsToEnv.Value;
        }

        if (model.Ttl.HasValue)
        {
            pool.DefaultResourcesCacheTTLSec = model.Ttl.Value;
        }

        if (model.MaxTotalRetries.HasValue || model.MaxRetriesPerInstance.HasValue) {
            pool.DefaultRetrySettings = new();
            if (model.MaxTotalRetries.HasValue) {
                pool.DefaultRetrySettings.MaxTotalRetries = model.MaxTotalRetries.Value;
            }
            if (model.MaxRetriesPerInstance.HasValue) {
                pool.DefaultRetrySettings.MaxPerInstanceRetries = model.MaxRetriesPerInstance.Value;
            }
        }

        if (model.SecretsAccessRightsByKey is not null)
        {
            pool.SecretsAccessRights ??= new();
            pool.SecretsAccessRights.BySecret = model
                .SecretsAccessRightsByKey
                .Select(k => new QSecretAccessRightBySecret() { Key = k })
                .ToList();
        }

        if (model.SecretsAccessRightsByPrefix is not null)
        {
            pool.SecretsAccessRights ??= new();
            pool.SecretsAccessRights.ByPrefix = model
                .SecretsAccessRightsByPrefix
                .Select(p => new QSecretAccessRightByPrefix() { Prefix = p })
                .ToList();
        }

        if (model.SchedulingType is not null && Enum.TryParse<SchedulingType>(model.SchedulingType, true, out var schedType))
        {
            pool.SchedulingType = schedType;
        }

        pool.TargetedReservedMachineKey = model.MachineTarget;

        return pool;
    }

    public async Task List(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Listing pools' summaries");
        var pools = await GetPools(model);
        Logger.Result(Formatter.FormatCollection(
            pools
                .Select(p =>
                    new PoolSummary(
                        p.Name,
                        p.Shortname,
                        p.Profile,
                        p.State,
                        p.CreationDate,
                        p.QueuedOrRunningTaskInstancesCount,
                        p.TaskDefaultWaitForPoolResourcesSynchronization
                    )
                ).ToList()
        ));
    }

    public async Task Info(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Listing pools");
        Logger.Result(Formatter.FormatCollection(await GetPools(model)));
    }

    public async Task UpdateElasticSettings(UpdatePoolElasticSettingsModel model)
    {
        Logger.Debug("Updating pools elastic settings");
        var pools = await GetPools(model);
        var updates = await Task.WhenAll(pools.Select(async p => {
            p.ElasticMinimumTotalSlots = model.MinSlots ?? p.ElasticMinimumTotalSlots;
            p.ElasticMaximumTotalSlots = model.MaxSlots ?? p.ElasticMaximumTotalSlots;
            p.ElasticMinimumIdlingSlots = model.MinIdlingSlots ?? p.ElasticMinimumIdlingSlots;
            p.ElasticResizePeriod = model.ResizePeriod ?? p.ElasticResizePeriod;
            p.ElasticResizeFactor = model.ResizeFactor ?? p.ElasticResizeFactor;
            p.ElasticMinimumIdlingTime = model.MinIdlingTime ?? p.ElasticMinimumIdlingTime;
            await p.CommitAsync();
            return p.Uuid;
        }));

        Logger.Result(Formatter.FormatCollection(updates.Select(p => new {
            Pool = p,
            State = "Updated elastic settings"
        }).ToList()));
    }

    public async Task UpdateScaling(UpdatePoolScalingModel model)
    {
        Logger.Debug("Updating pools scaling");
        var pools = await GetPools(model);
        var updates = await Task.WhenAll(pools.Select(async pool => {
            await pool.UpdateScalingAsync(model.Scaling);
            return pool.Uuid;
        }));

        Logger.Result(Formatter.FormatCollection(updates.Select(p => new {
            Pool = p,
            State = "Updated scaling"
        }).ToList()));
    }

    public async Task Delete(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Deleting pools");
        var pools = await GetPools(model);
        var deletions = await Task.WhenAll(pools.Select(async pool => {
            await pool.DeleteAsync();
            return pool.Uuid;
        }));

        Logger.Result(Formatter.FormatCollection(deletions.Select(p => new {
            Pool = p,
            State = "Deleted",
        }).ToList()));
    }

    public async Task UpdateResources(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Updating pools resources");
        var pools = await GetPools(model);
        var updates = await Task.WhenAll(pools.Select(async pool => {
            await pool.UpdateResourcesAsync();
            return pool.Uuid;
        }));

        Logger.Result(Formatter.FormatCollection(updates.Select(p => new {
            Pool = p,
            State = "Updated resources"
        }).ToList()));
    }

    public async Task UpdateConstant(UpdatePoolsOrTasksConstantModel model)
    {
        Logger.Debug("Updating pools constant");
        var pools = await GetPools(model);
        var updates = await Task.WhenAll(pools.Select(async pool => {
            pool.SetConstant(model.ConstantName, model.ConstantValue);
            await pool.CommitAsync();
            return pool.Uuid;
        }));

        Logger.Result(Formatter.FormatCollection(updates.Select(p => new {
            Pool = p,
            State = "Updated constant",
            Constant = model.ConstantName,
        }).ToList()));
    }

    private async Task<List<QPool>> GetPools(GetPoolsOrTasksModel model)
    {
        if (!string.IsNullOrEmpty(model.Name))
        {
            Logger.Debug($"Retrieving pool by shortname: {model.Name}");
            return new() { await QarnotAPI.RetrievePoolByShortnameAsync(model.Name) };
        }
        else if (!string.IsNullOrEmpty(model.Id))
        {
            Logger.Debug($"Retrieving pool by UUID: {model.Id}");
            return new() { await QarnotAPI.RetrievePoolByUuidAsync(model.Id) };
        }
        else if (model.ExclusiveTags?.Any() ?? false)
        {
            Logger.Debug($"Retrieving pools by tags intersection: {string.Join(", ", model.ExclusiveTags)}");
            var filters = new QDataDetail<QPool>();
            var filtersList = model.ExclusiveTags.Select(tag => QFilter<QPool>.Contains(t => t.Tags, tag));
            filters.Filter = QFilter<QPool>.And(filtersList.ToArray());
            return await QarnotAPI.RetrievePoolsAsync(filters);
        }
        else if (model.Tags?.Any() ?? false)
        {
            Logger.Debug($"Retrieving pools by tags union: {string.Join(", ", model.Tags)}");
            return await QarnotAPI.RetrievePoolsByTagsAsync(model.Tags);
        }
        else
        {
            Logger.Debug($"Retrieving all the pools");
            return await QarnotAPI.RetrievePoolsAsync();
        }
    }
}
