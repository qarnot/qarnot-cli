using System.Text.RegularExpressions;
using QarnotSDK;

namespace QarnotCLI;

public interface ITaskUseCases
{
    Task Create(CreateTaskModel model);
    Task List(GetPoolsOrTasksModel model);
    Task Info(GetPoolsOrTasksModel model);
    Task Wait(WaitTasksModel model);
    Task Abort(GetPoolsOrTasksModel model);
    Task Delete(GetPoolsOrTasksModel model);
    Task UpdateResources(GetPoolsOrTasksModel model);
    Task UpdateConstant(UpdatePoolsOrTasksConstantModel model);
    Task Snapshot(SnapshotTasksModel model);
    Task Stdout(GetTasksOutputModel model);
    Task Stderr(GetTasksOutputModel model);
}

public class TaskUseCases : ITaskUseCases
{
    private readonly ILogger Logger;
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly IStateManager StateManager;

    public TaskUseCases(
        Connection qarnotAPI,
        IFormatter formatter,
        IStateManager stateManager,
        ILogger logger)
    {
        QarnotAPI = qarnotAPI;
        Formatter = formatter;
        StateManager = stateManager;
        Logger = logger;
    }

    public async Task Create(CreateTaskModel model)
    {
        Logger.Debug("Creating task");
        QTask task = await CreateTask(model);
        await InitializeTask(model, task);
        Logger.Debug("Submitting task");
        await task.SubmitAsync();

        if (model.Periodic is uint period && period > 0)
        {
            Logger.Debug("Triggering task's periodic snapshot");
            task.SnapshotWhitelist = model.Whitelist;
            task.SnapshotBlacklist = model.Blacklist;
            await task.TriggerPeriodicSnapshotAsync(period, task.SnapshotWhitelist, task.SnapshotBlacklist);
        }

        Logger.Result(Formatter.Format(new {
            Task = task.Uuid,
            State = "Created",
        }));
    }

    private async Task InitializeTask(CreateTaskModel model, QTask task)
    {
        task.SetTags(model.Tags.ToArray());
        task.Resources = (await Task.WhenAll(
            model.Resources.Select(async r => await QarnotAPI.CreateBucketAsync(r) as QAbstractStorage)
        )).ToList();
        task.Results = string.IsNullOrWhiteSpace(model.Result)
            ? null
            : await QarnotAPI.CreateBucketAsync(model.Result);

        foreach (var constant in model.Constants)
        {
            if (Helpers.ParseKeyValuePair(constant, out var kvp))
            {
                task.SetConstant(kvp.Key, kvp.Value);
            }
        }

         foreach (var constraint in model.Constraints)
         {
             if (Helpers.ParseKeyValuePair(constraint, out var kvp))
             {
                 task.SetConstraint(kvp.Key, kvp.Value);
             }
         }

         foreach (var label in model.Labels)
         {
             if (Helpers.ParseKeyValuePair(label, out var kvp))
             {
                 task.SetLabel(kvp.Key, kvp.Value);
             }
         }

         task.SetTaskDependencies(model.Dependents.Select(id => new Guid(id)).ToArray());

         task.ResultsWhitelist = model.Whitelist;
         task.ResultsBlacklist = model.Blacklist;

         if (model.MaxTotalRetries.HasValue || model.MaxRetriesPerInstance.HasValue)
         {
             task.RetrySettings = new();
             if (model.MaxTotalRetries.HasValue)
             {
                 task.RetrySettings.MaxTotalRetries = model.MaxTotalRetries.Value;
             }
             if (model.MaxRetriesPerInstance.HasValue)
             {
                 task.RetrySettings.MaxPerInstanceRetries = model.MaxRetriesPerInstance.Value;
             }
         }

         task.Privileges.ExportApiAndStorageCredentialsInEnvironment = model.ExportCredentialsToEnv;
         task.DefaultResourcesCacheTTLSec = model.Ttl;
         task.HardwareConstraints = model.HardwareConstraints;

         task.SecretsAccessRights = new();
         task.SecretsAccessRights.BySecret = model
             .SecretsAccessRightsByKey
             .Select(k => new QSecretAccessRightBySecret { Key = k })
             .ToList();
        task.SecretsAccessRights.ByPrefix = model
            .SecretsAccessRightsByPrefix
            .Select(p => new QSecretAccessRightByPrefix { Prefix = p })
            .ToList();

        if (model.SchedulingType is not null && Enum.TryParse<SchedulingType>(model.SchedulingType, true, out var schedType))
        {
            task.SchedulingType = schedType;
        }

        task.TargetedReservedMachineKey = model.MachineTarget;
    }

    private async Task<QTask> CreateTask(CreateTaskModel model)
    {
        AdvancedRanges? ranges = model.Instance > 0 || model.Range is null
            ? null
            : new AdvancedRanges(model.Range);

        if (!string.IsNullOrEmpty(model.Job))
        {
            QJob job;
            if (Guid.TryParse(model.Job, out var _))
            {
                job = await QarnotAPI.RetrieveJobByUuidAsync(model.Job);
            }
            else
            {
                job = await QarnotAPI.RetrieveJobByShortnameAsync(model.Job);
            }

            return ranges is null
                ? new QTask(QarnotAPI, model.Name, job, model.Instance, model.ShortName, model.Profile!)
                : new QTask(QarnotAPI, model.Name, job, ranges, model.ShortName, model.Profile!);
        }
        else if (!string.IsNullOrEmpty(model.Pool))
        {
            QPool pool;
            if (Guid.TryParse(model.Pool, out var poolUuid))
            {
                pool = new QPool(QarnotAPI, poolUuid);
            }
            else
            {
                pool = await QarnotAPI.RetrievePoolByShortnameAsync(model.Pool);
            }

            return ranges is null
                ? new QTask(QarnotAPI, model.Name, pool, model.Instance, model.ShortName, model.WaitForResourcesSynchronization)
                : new QTask(QarnotAPI, model.Name, pool, ranges, model.ShortName, model.WaitForResourcesSynchronization);
        }

        return ranges is null
            ? new QTask(QarnotAPI, model.Name, model.Profile!, model.Instance, model.ShortName)
            : new QTask(QarnotAPI, model.Name, model.Profile!, ranges, model.ShortName);
    }


    public async Task List(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Listing tasks summaries");
        string listTasksOutput;

        if(model.IsTargetingSingleResource())
        {
            QTask task = null;
            if (!string.IsNullOrEmpty(model.Shortname))
            {
                Logger.Debug($"Retrieving task by shortname: {model.Shortname}");
                task = await QarnotAPI.RetrieveTaskByShortnameAsync(model.Shortname);
            }
            else if (!string.IsNullOrEmpty(model.Id))
            {
                Logger.Debug($"Retrieving task by UUID: {model.Id}");
                task = await QarnotAPI.RetrieveTaskByUuidAsync(model.Id);
            }

            listTasksOutput = Formatter
                .FormatCollection(task == null ?
                    new TaskSummary[] {} :
                    new [] { new TaskSummary(
                        Name: task.Name,
                        State: task.State,
                        Uuid: task.Uuid.ToString(),
                        Shortname: task.Shortname,
                        Profile: task.Profile,
                        InstanceCount: task.InstanceCount)});
        }
        else if (model.NoPaginate)
        {
            var tasks = await GetTasks(model);
            listTasksOutput = Formatter
                .FormatCollection(tasks
                    .Select(t =>
                        new TaskSummary(
                            Name: t.Name,
                            State: t.State,
                            Uuid: t.Uuid.ToString(),
                            Shortname: t.Shortname,
                            Profile: t.Profile,
                            InstanceCount: t.InstanceCount))
                    .ToList());
        }
        else
        {
            var tasksPage = await GetTasksPage(model);
            var pageResponse = new ResourcesPageModel<TaskSummary>(
                Items: tasksPage
                    .Data
                    .Select(t =>
                        new TaskSummary(
                            Name: t.Name,
                            State: t.State,
                            Uuid: t.Uuid.ToString(),
                            Shortname: t.Shortname,
                            Profile: t.Profile,
                            InstanceCount: t.InstanceCount))
                    .ToList(),
                MaxPageSize: model.MaxPageSize,
                NextPageToken: tasksPage.IsTruncated ? tasksPage.NextToken : string.Empty);

            listTasksOutput = Formatter
                .FormatCollectionPage(pageResponse);
        }

        Logger.Result(listTasksOutput);
    }

    public async Task Info(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Listing tasks");
        Logger.Result(Formatter.FormatCollection(await GetTasks(model)));
    }

    public async Task Wait(WaitTasksModel model)
    {
        Logger.Debug("Waiting tasks");
        var tasks = await GetTasks(model);
        var statuses = await Task.WhenAll(tasks.Select(async task =>
        {
            bool end = false;
            while (!end)
            {
                end = await task.WaitAsync(2);

                if (model.Stdout)
                {
                    var stdmessage = await task.FreshStdoutAsync();
                    if (!string.IsNullOrEmpty(stdmessage))
                    {
                        Logger.Info($"Stdout:{Environment.NewLine}{stdmessage}");
                    }
                }

                if (model.Stderr)
                {
                    var stdmessage = await task.FreshStderrAsync();
                    if (!string.IsNullOrEmpty(stdmessage))
                    {
                        Logger.Info($"Stderr:{Environment.NewLine}{stdmessage}");
                    }
                }
            }

            return new {
                Task = task.Uuid,
                State = task.State
            };
        }));

        Logger.Result(Formatter.FormatCollection(statuses.ToList()));
    }

    public async Task Abort(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Aborting tasks");
        var tasks = await GetTasks(model);
        var abortions = await Task.WhenAll(tasks.Select(async task => {
            await task.AbortAsync();
            return new {
                Task = task.Uuid,
                State = "Aborted"
            };
        }));

        Logger.Result(Formatter.FormatCollection(abortions.ToList()));
    }

    public async Task Delete(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Deleting tasks");
        var tasks = await GetTasks(model);
        var deletions = await Task.WhenAll(tasks.Select(async task => {
            await task.DeleteAsync();
            return new {
                Task = task.Uuid,
                State = "Deleted"
            };
        }));

        Logger.Result(Formatter.FormatCollection(deletions.ToList()));
    }

    public async Task UpdateResources(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Updating tasks resources");
        var tasks = await GetTasks(model);
        var updates = await Task.WhenAll(tasks.Select(async task => {
            await task.UpdateResourcesAsync();
            return new {
                Task = task.Uuid,
                State = "Updated resources"
            };
        }));

        Logger.Result(Formatter.FormatCollection(updates.ToList()));
    }

    public async Task UpdateConstant(UpdatePoolsOrTasksConstantModel model)
    {
        Logger.Debug("Updating tasks constant");
        var tasks = await GetTasks(model);
        var updates = await Task.WhenAll(tasks.Select(async task => {
            task.SetConstant(model.ConstantName, model.ConstantValue);
            Console.WriteLine(task.PoolUuid);
            await task.CommitAsync();
            return new {
                Task = task.Uuid,
                State = "Updated constant",
                Constant = model.ConstantName
            };
        }));

        Logger.Result(Formatter.FormatCollection(updates.ToList()));
    }

    public async Task Snapshot(SnapshotTasksModel model)
    {
        Logger.Debug("Snapshoting tasks");
        var tasks = await GetTasks(model);
        var snapshots = await Task.WhenAll(tasks.Select(async task => {
            task.SnapshotWhitelist = model.Whitelist;
            task.SnapshotBlacklist = model.Blacklist;
            var bucket = model.Bucket != null ? new QBucket(task.Connection, model.Bucket): null;

            if (model.Periodic is uint period && period > 0)
            {
                await task.TriggerPeriodicSnapshotAsync(period, task.SnapshotWhitelist, task.SnapshotBlacklist, bucket);
            }
            else
            {
                await task.TriggerSnapshotAsync(task.SnapshotWhitelist, task.SnapshotBlacklist, bucket);
            }

            return new {
                Task = task.Uuid,
                State = "Snapshot",
            };
        }));

        Logger.Result(Formatter.FormatCollection(snapshots.ToList()));
    }

    public async Task Stdout(GetTasksOutputModel model)
    {
        Logger.Debug("Printing tasks' STDOUT");
        var tasks = await GetTasks(model);
        var outputs = await Task.WhenAll(tasks.Select(async task => {
            if (model.InstanceId is uint instanceId)
            {
                return model.Fresh
                    ? await task.FreshStdoutAsync(instanceId)
                    : await task.StdoutAsync(instanceId);
            }
            else
            {
                return model.Fresh
                    ? await task.FreshStdoutAsync()
                    : await task.StdoutAsync();
            }
        }));

        Logger.Result(Formatter.FormatCollection(outputs.ToList()));
    }

    public async Task Stderr(GetTasksOutputModel model)
    {
        Logger.Debug("Printing tasks' STDERR");
        var tasks = await GetTasks(model);
        var outputs = await Task.WhenAll(tasks.Select(async task => {
            if (model.InstanceId is uint instanceId)
            {
                return model.Fresh
                    ? await task.FreshStderrAsync(instanceId)
                    : await task.StderrAsync(instanceId);
            }
            else
            {
                return model.Fresh
                    ? await task.FreshStderrAsync()
                    : await task.StderrAsync();
            }
        }));

        Logger.Result(Formatter.FormatCollection(outputs.ToList()));
    }

    private async Task<List<QTask>> GetTasks(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Retrieving all the tasks");
        var allTasks = new List<QTask>();
        var pageDetails = GeneratePageRequest(
            model.Name,
            model.CreatedBefore,
            model.CreatedAfter,
            model.NamePrefix,
            exclusiveTags: model?.ExclusiveTags,
            tags: model?.Tags);
        PaginatedResponse<QTask> page;
        do
        {
            page = await QarnotAPI.RetrievePaginatedTaskAsync(pageDetails);
            allTasks.AddRange(page.Data);
        } while (pageDetails.PrepareNextPage(page));

        return allTasks;
    }


    private PaginatedRequest<QTask> GeneratePageRequest(
        string? name,
        string? createdBefore,
        string? createdAfter,
        string? namePrefix,
        int? maxPageSize = null,
        string? nextPageToken = null,
        List<string> exclusiveTags = default,
        List<string> tags = default)
    {
        var filters = new List<QFilter<QTask>>();
        if (!string.IsNullOrWhiteSpace(name))
        {
            filters.Add(QFilter<QTask>.Eq(
                x => x.Name,
                name));
        }

        if (!string.IsNullOrWhiteSpace(createdBefore))
        {
            filters.Add(QFilter<QTask>.Lt(
                x => x.CreationDate,
                DateTime.Parse(createdBefore)));
        }

        if (!string.IsNullOrWhiteSpace(createdAfter))
        {
            filters.Add(QFilter<QTask>.Gt(
                x => x.CreationDate,
                DateTime.Parse(createdAfter)));
        }

        if (!string.IsNullOrWhiteSpace(namePrefix))
        {
            var prefixRegex = $"/^{Regex.Escape(namePrefix)}/";

            filters.Add(QFilter<QTask>.Like(
                x => x.Name,
                prefixRegex));
        }

        if (exclusiveTags?.Any() ?? false)
        {
            var filtersList = exclusiveTags.Select(tag => QFilter<QTask>.Contains(t => t.Tags, tag));
            filters.Add(QFilter<QTask>.And(filtersList.ToArray()));
        }

        if (tags?.Any() ?? false)
        {
            var filtersList = tags.Select(tag => QFilter<QTask>.Contains(t => t.Tags, tag));
            filters.Add(QFilter<QTask>.Or(filtersList.ToArray()));
        }

        var pageRequest = new PaginatedRequest<QTask>();
        if (maxPageSize != default)
        {
            pageRequest.MaximumResults = maxPageSize;
        }

        if (nextPageToken != default)
        {
            pageRequest.Token = nextPageToken;
        }

        if (filters.Any())
        {
            pageRequest.Filter = QFilter<QTask>.And(filters.ToArray());
        }

        return pageRequest;
    }

    private async Task<PaginatedResponse<QTask>> GetTasksPage(GetPoolsOrTasksModel model)
    {
        Logger.Debug("Retrieving a tasks page");
        var nextPageToken = model.NextPageToken;
        if (model.NextPage && string.IsNullOrWhiteSpace(nextPageToken))
        {
            nextPageToken = StateManager.GetNextPageToken()?.Token;
        }

        var pageDetails = GeneratePageRequest(
            model.Name,
            model.CreatedBefore,
            model.CreatedAfter,
            model.NamePrefix,
            model.MaxPageSize,
            nextPageToken,
            exclusiveTags: model?.ExclusiveTags,
            tags: model?.Tags);

        var page = await QarnotAPI.RetrievePaginatedTaskAsync(pageDetails);
        StateManager.SaveNextPageToken(
            new PageToken(page.IsTruncated ? page.NextToken : string.Empty));
        return page;
    }
}
