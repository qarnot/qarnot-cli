using QarnotSDK;

namespace QarnotCLI;

public interface IJobUseCases
{
    Task Create(CreateJobModel model);
    Task Delete(GetJobModel model);
    Task List(GetJobModel model);
    Task Abort(GetJobModel model);
    Task Info(GetJobModel model);
}

public class JobUseCases : IJobUseCases
{
    private readonly Connection QarnotAPI;
    private readonly IFormatter Formatter;
    private readonly ILogger Logger;

    public JobUseCases(
        Connection api,
        IFormatter formatter,
        IStateManager _,
        ILogger logger)
    {
        QarnotAPI = api;
        Formatter = formatter;
        Logger = logger;
    }

    public async Task Abort(GetJobModel model)
    {
        Logger.Debug("Aborting jobs");
        var jobs = await GetJobs(model);

        var abortions = await Task.WhenAll(jobs.Select(async j => {
            await j.TerminateAsync();
            return new {
                Job = j.Uuid,
                State = "Aborted"
            };
        }));

        Logger.Result(Formatter.FormatCollection(abortions.ToList()));
    }

    public async Task Create(CreateJobModel model)
    {
        Logger.Debug("Creating job");
        QPool? pool = null;
        if (!string.IsNullOrEmpty(model.Pool))
        {
            pool = await QarnotAPI.RetrievePoolByUuidAsync(model.Pool);
        }

        var job = QarnotAPI.CreateJob(model.Name, pool, model.Shortname, model.IsDependent);

        if (model.MaxWallTime is TimeSpan maxWallTime)
        {
            job.MaximumWallTime = maxWallTime;
        }

        await job.SubmitAsync();

        Logger.Result(Formatter.Format(new {
            Job = job.Uuid,
            State = "Created"
        }));
    }

    public async Task Delete(GetJobModel model)
    {
        Logger.Debug("Deleting jobs");
        var jobs = await GetJobs(model);

        var deletions = await Task.WhenAll(jobs.Select(async j => {
            await j.DeleteAsync();
            return new {
                Job = j.Uuid,
                State = "Deleted",
            };
        }));

        Logger.Result(Formatter.FormatCollection(deletions.ToList()));
    }

    public async Task Info(GetJobModel model)
    {
        Logger.Debug("Listing detailed jobs information");
        Logger.Result(Formatter.FormatCollection(await GetJobs(model)));
    }

    public async Task List(GetJobModel model)
    {
        Logger.Debug("Listing jobs summaries");
        var jobs = await GetJobs(model);
        Logger.Result(Formatter.FormatCollection(jobs.Select(j => new JobSummary(
            Name: j.Name,
            Shortname: j.Shortname,
            Uuid: j.Uuid.ToString(),
            State: j.State
        )).ToList()));
    }

    private async Task<List<QJob>> GetJobs(GetJobModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Name))
        {
            Logger.Debug($"Retrieving job by name: {model.Name}");
            QDataDetail<QJob> filter = new QDataDetail<QJob>()
            {
                Filter = QFilter<QJob>.Eq(t => t.Name, model.Name),
            };
            var job = (await QarnotAPI.RetrieveJobsAsync(filter)).FirstOrDefault();
            if (job is null)
            {
                throw new Exception($"Job with name {model.Name} doesn't exist");
            }

            return new() { job };
        }
        else if (!string.IsNullOrWhiteSpace(model.Id))
        {
            Logger.Debug($"Retrieving job by UUID: {model.Id}");
            var job = await QarnotAPI.RetrieveJobByUuidAsync(model.Id);
            if (job is null)
            {
                throw new Exception($"Job with ID {model.Id} doesn't exist");
            }

            return new() { job };
        }
        else
        {
            return await QarnotAPI.RetrieveJobsAsync();
        }
    }
}
