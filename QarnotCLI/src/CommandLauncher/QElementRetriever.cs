namespace QarnotCLI
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    /// <summary>
    /// This class Interface manages the recuperation a list of Qarnot elements from the API.
    /// </summary>
    /// <typeparam name="T">The generic type parameter: QObject.</typeparam>
    public interface IQCollectionRetriever<T>
    {
        Task<List<T>> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct);
    }

    /// <summary>
    /// This class Interface manages the recuperation a Qarnot element from the API.
    /// </summary>
    /// <typeparam name="T">The generic type parameter: QObject.</typeparam>
    public interface IQElementRetriever<T>
    {
        Task<T> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct);
    }

    /// <summary>
    /// This class manages the recuperation of QJobs elements from the API.
    /// </summary>
    public class QJobsRetriever : IQCollectionRetriever<QarnotSDK.QJob>
    {
        public async Task<List<QarnotSDK.QJob>> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            DefaultRunConfiguration config = configuration as DefaultRunConfiguration;
            QarnotSDK.QJob job = null;
            List<QJob> listJob = null;

            if (!string.IsNullOrEmpty(config.Name))
            {
                CLILogs.Debug("Retrieve QJobs by name : " + config.Name);
                QDataDetail<QJob> level = new QDataDetail<QJob>()
                {
                    Filter = QFilter<QJob>.Eq(t => t.Name, config.Name),
                };
                listJob = await connection.RetrieveJobsAsync(level, cancellationToken: ct);
            }
            else if (!string.IsNullOrEmpty(config.Id))
            {
                CLILogs.Debug("Retrieve QJob by Uuid : " + config.Id);
                job = await connection.RetrieveJobByUuidAsync(config.Id, cancellationToken: ct);
            }
            else
            {
                CLILogs.Debug("Retrieve all the QJobs");
                listJob = connection.RetrieveJobs(cancellationToken: ct);
            }

            return listJob ?? new List<QJob>() { job };
        }
    }

    /// <summary>
    /// This class manages the recuperation of QPool elements from the API.
    /// </summary>
    public class QPoolsRetriever : IQCollectionRetriever<QarnotSDK.QPool>
    {
        public async Task<List<QarnotSDK.QPool>> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            DefaultRunConfiguration config = configuration as DefaultRunConfiguration;

            QarnotSDK.QPool pool = null;
            List<QPool> listPool = null;

            if (!string.IsNullOrEmpty(config.Name))
            {
                CLILogs.Debug("Retrieve QPools by name : " + config.Name);
                pool = await connection.RetrievePoolByNameAsync(config.Name, cancellationToken: ct);
            }
            else if (!string.IsNullOrEmpty(config.Id))
            {
                CLILogs.Debug("Retrieve QPool by Uuid : " + config.Id);
                pool = await connection.RetrievePoolByUuidAsync(config.Id, cancellationToken: ct);
            }
            else if (config.Tags != null && config.Tags.Count > 0)
            {
                CLILogs.Debug("Retrieve QPools by Tags : " + config.Tags.ToString());
                listPool = await connection.RetrievePoolsByTagsAsync(config.Tags, cancellationToken: ct);
            }
            else
            {
                CLILogs.Debug("Retrieve all the QPools");
                listPool = await connection.RetrievePoolsAsync(cancellationToken: ct);
            }

            return listPool ?? new List<QPool>() { pool };
        }
    }

    /// <summary>
    /// This class manages the recuperation of QTask elements from the API.
    /// </summary>
    public class QTasksRetriever : IQCollectionRetriever<QarnotSDK.QTask>
    {
        public async Task<List<QarnotSDK.QTask>> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            DefaultRunConfiguration config = configuration as DefaultRunConfiguration;
            QarnotSDK.QTask task = null;
            List<QTask> listTask = null;

            if (!string.IsNullOrEmpty(config.Name))
            {
                CLILogs.Debug("Retrieve QTasks by Name : " + config.Name);
                task = await connection.RetrieveTaskByNameAsync(config.Name, cancellationToken: ct);
            }
            else if (!string.IsNullOrEmpty(config.Id))
            {
                CLILogs.Debug("Retrieve QTask by Uuid : " + config.Id);
                task = await connection.RetrieveTaskByUuidAsync(config.Id, cancellationToken: ct);
            }
            else if (config.Tags != null && config.Tags.Count > 0)
            {
                CLILogs.Debug("Retrieve QTasks by Tags : " + config.Tags.ToString());
                listTask = await connection.RetrieveTasksByTagsAsync(config.Tags, cancellationToken: ct);
            }
            else
            {
                CLILogs.Debug("Retrieve all the QTasks");
                listTask = await connection.RetrieveTasksAsync(cancellationToken: ct);
            }

            return listTask ?? new List<QTask>() { task };
        }
    }

    /// <summary>
    /// This class manages the recuperation of QBBucket elements from the API.
    /// </summary>
    public class QBucketsRetriever : IQCollectionRetriever<QarnotSDK.QBucket>
    {
        public async Task<List<QarnotSDK.QBucket>> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            BucketConfiguration config = configuration as BucketConfiguration;

            if (config != null && !string.IsNullOrEmpty(config.Name))
            {
                CLILogs.Debug("Retrieve QBucket by Name : " + config.Name);
                return new List<QBucket>()
                {
                    await connection.RetrieveBucketAsync(config.Name, cancellationToken: ct),
                };
            }
            else
            {
                CLILogs.Debug("Retrieve all the QBuckets");
                return await connection.RetrieveBucketsAsync(cancellationToken: ct);
            }
        }
    }

    /// <summary>
    /// This class manages the recuperation a QBBucket element By it's name.
    /// </summary>
    public class QBucketRetriever : IQElementRetriever<QarnotSDK.QBucket>
    {
        public async Task<QarnotSDK.QBucket> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            BucketConfiguration config = configuration as BucketConfiguration;
            CLILogs.Debug("Retrieve QBucket by Name : " + config.Name);
            return await connection.RetrieveBucketAsync(config.Name, ct);
        }
    }

    /// <summary>
    /// This class manages the recuperation of QBBucket elements from the API.
    /// </summary>
    public class UserInformationRetriever : IQElementRetriever<QarnotSDK.UserInformation>
    {
        public async Task<QarnotSDK.UserInformation> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            AccountConfiguration config = configuration as AccountConfiguration;
            return await connection.RetrieveUserInformationAsync(config.GetBucket, cancellationToken: ct);
        }
    }

    /// <summary>
    /// This class manages the recuperation of QBBucket elements from the API.
    /// </summary>
    public class APISettingsRetriever : IQElementRetriever<QarnotSDK.ApiSettings>
    {
        public async Task<QarnotSDK.ApiSettings> RetrieveAsync(IConfiguration configuration, QarnotSDK.Connection connection, CancellationToken ct)
        {
            return await connection.RetrieveApiSettingsAsync(cancellationToken: ct);
        }
    }
}