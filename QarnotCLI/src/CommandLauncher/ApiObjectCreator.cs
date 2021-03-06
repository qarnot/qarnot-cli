namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class ApiObjectCreator
    {
        public interface IApiObjectCreator
        {
            Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct);
        }

        public abstract class AApiObjectCreator : IApiObjectCreator
        {
            public AApiObjectCreator(ICreateHelper tool)
            {
                Tools = tool;
            }

            protected ICreateHelper Tools { get; }

            public abstract Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct);
        }

        public class CreateNewPool : AApiObjectCreator
        {
            public CreateNewPool(ICreateHelper tool)
                : base(tool)
            {
            }

            private QPool CreatePool(CreateConfiguration config, QarnotSDK.Connection connect, CancellationToken ct)
            {
                QarnotSDK.QPool pool = new QarnotSDK.QPool(connect, config.Name, config.Profile, config.InstanceCount, config.Shortname);

                pool.SetTags(config.Tags.ToArray());

                pool.Resources = this.Tools.CreateResources(config.Resources, connect, ct);

                foreach (var item in this.Tools.CreateConstants(config.Constants))
                {
                    pool.SetConstant(item.Key, item.Value);
                }

                foreach (var item in this.Tools.CreateConstraints(config.Constraints))
                {
                    pool.SetConstraint(item.Key, item.Value);
                }

                pool.IsElastic = config.IsElastic;
                pool.ElasticMinimumTotalNodes = config.ElasticMinimumTotalNodes == default(int) ? pool.ElasticMinimumTotalNodes : config.ElasticMinimumTotalNodes;
                pool.ElasticMaximumTotalNodes = config.ElasticMaximumTotalNodes == default(int) ? pool.ElasticMaximumTotalNodes : config.ElasticMaximumTotalNodes;
                pool.ElasticMinimumIdlingNodes = config.ElasticMinimumIdlingNodes == default(int) ? pool.ElasticMinimumIdlingNodes : config.ElasticMinimumIdlingNodes;
                pool.ElasticResizePeriod = config.ElasticResizePeriod == default(int) ? pool.ElasticResizePeriod : config.ElasticResizePeriod;
                pool.ElasticResizeFactor = config.ElasticResizeFactor == default(int) ? pool.ElasticResizeFactor : config.ElasticResizeFactor;
                pool.ElasticMinimumIdlingTime = config.ElasticMinimumIdlingTime == default(int) ? pool.ElasticMinimumIdlingTime : config.ElasticMinimumIdlingTime;

                CLILogs.Info("create pool");
                return pool;
            }

            private async Task<QPool> LaunchPoolAsync(QPool pool, CancellationToken ct)
            {
                await pool.StartAsync(ct);
                return pool;
            }

            private CommandValues.GenericInfoCommandValue PrintPoolInformation(QPool pool)
            {
                var info = new CommandValues.GenericInfoCommandValue();
                info.Uuid = pool.Uuid.ToString();
                info.Message = "New pool created, state : " + pool.State;
                return info;
            }

            public async override Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct)
            {
                CreateConfiguration config = iconfig as CreateConfiguration;
                QarnotSDK.QPool pool = this.CreatePool(config, connect, ct);

                await this.LaunchPoolAsync(pool, ct);
                return this.PrintPoolInformation(pool);
            }
        }

        public class CreateNewTask : AApiObjectCreator
        {
            public CreateNewTask(ICreateHelper tool)
                : base(tool)
            {
            }

            private async Task<QTask> CreateInstanceTaskAsync(CreateConfiguration config, QarnotSDK.Connection connect, CancellationToken ct = default(CancellationToken))
            {
                if (!string.IsNullOrEmpty(config.JobUuid))
                {
                    QarnotSDK.QJob job = await connect.RetrieveJobByUuidAsync(config.JobUuid, ct);
                    return new QarnotSDK.QTask(connect, config.Name, job, config.InstanceCount, config.Shortname);
                }
                else if (!string.IsNullOrEmpty(config.PoolUuid))
                {
                    QarnotSDK.QPool pool = new QarnotSDK.QPool(connect, new Guid(config.PoolUuid));
                    return new QarnotSDK.QTask(connect, config.Name, pool, config.InstanceCount, config.Shortname);
                }

                return new QarnotSDK.QTask(connect, config.Name, config.Profile, config.InstanceCount, config.Shortname);
            }

            private async Task<QTask> CreateRangeTaskAsync(CreateConfiguration config, QarnotSDK.Connection connect, CancellationToken ct = default(CancellationToken))
            {
                AdvancedRanges range = new AdvancedRanges(config.Range);
                if (!string.IsNullOrEmpty(config.JobUuid))
                {
                    QarnotSDK.QJob job = await connect.RetrieveJobByUuidAsync(config.JobUuid, ct);
                    return new QarnotSDK.QTask(connect, config.Name, job, range, config.Shortname);
                }
                else if (!string.IsNullOrEmpty(config.PoolUuid))
                {
                    QarnotSDK.QPool pool = new QarnotSDK.QPool(connect, new Guid(config.PoolUuid));
                    return new QarnotSDK.QTask(connect, config.Name, pool, range, config.Shortname);
                }

                return new QarnotSDK.QTask(connect, config.Name, config.Profile, range, config.Shortname);
            }

            private async Task<QTask> CreateTask(CreateConfiguration config, QarnotSDK.Connection connect, CancellationToken ct)
            {
                QarnotSDK.QTask task = null;
                if (config.InstanceCount > 0)
                {
                    task = await this.CreateInstanceTaskAsync(config, connect);
                }
                else
                {
                    task = await this.CreateRangeTaskAsync(config, connect);
                }

                task.SetTags(config.Tags.ToArray());

                task.Resources = this.Tools.CreateResources(config.Resources, connect, ct);

                task.Results = this.Tools.CreateResult(config.Result, connect, ct);

                this.Tools.CreateConstants(config.Constants).ToList().ForEach(item => task.SetConstant(item.Key, item.Value));

                foreach (var item in this.Tools.CreateConstraints(config.Constraints))
                {
                    task.SetConstraint(item.Key, item.Value);
                }

                task.SetTaskDependencies(config.Dependents.Select(id => new Guid(id)).ToArray());

                CLILogs.Info("create task");
                return task;
            }

            private async Task<QTask> LaunchTaskAsync(QTask task, CancellationToken ct)
            {
                await task.SubmitAsync(cancellationToken: ct);
                return task;
            }

            private CommandValues.GenericInfoCommandValue PrintTaskInformation(QTask task)
            {
                var info = new CommandValues.GenericInfoCommandValue();
                info.Uuid = task.Uuid.ToString();
                info.Message = "New task created, state : " + task.State;
                return info;
            }

            public async override Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct)
            {
                CreateConfiguration config = iconfig as CreateConfiguration;
                QTask task = await this.CreateTask(config, connect, ct);
                await this.LaunchTaskAsync(task, ct);
                return this.PrintTaskInformation(task);
            }
        }

        public class CreateNewJob : AApiObjectCreator
        {
            public CreateNewJob(ICreateHelper tool)
                : base(tool)
            {
            }

            private async Task<QJob> CreateJobAsync(CreateConfiguration config, QarnotSDK.Connection connect, CancellationToken ct)
            {
                CLILogs.Info("create job");
                QPool pool = null;
                if (!string.IsNullOrEmpty(config.PoolUuid))
                {
                    pool = await connect.RetrievePoolByUuidAsync(config.PoolUuid, ct);
                }

                QJob job = new QJob(connect, config.Name, pool, config.Shortname, config.IsDependents);

                if (config.MaximumWallTime.HasValue && config.MaximumWallTime.Value != default(TimeSpan))
                {
                    job.MaximumWallTime = config.MaximumWallTime.Value;
                }

                return job;
            }

            private QJob LaunchJob(QJob job, CancellationToken ct)
            {
                job.Submit(cancellationToken: ct);
                return job;
            }

            private CommandValues.GenericInfoCommandValue PrintJobInformation(QJob job)
            {
                var info = new CommandValues.GenericInfoCommandValue();
                info.Uuid = job.Uuid.ToString();
                info.Message = "New job created, state : " + job.State;
                return info;
            }

            public async override Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct)
            {
                CreateConfiguration config = iconfig as CreateConfiguration;
                QJob job = await this.CreateJobAsync(config, connect, ct);
                this.LaunchJob(job, ct);
                return this.PrintJobInformation(job);
            }
        }

        public class BucketCreator : AApiObjectCreator
        {
            public BucketCreator(ICreateHelper tool)
                : base(tool)
            {
            }

            private async Task<QBucket> CreateBucket(BucketConfiguration config, QarnotSDK.Connection connect, CancellationToken ct)
            {
                return await connect.CreateBucketAsync(config.Name, ct);
            }

            private async Task<QBucket> LaunchBucket(QBucket bucket, CancellationToken ct)
            {
                await bucket.CreateAsync(ct);
                return bucket;
            }

            private CommandValues.GenericInfoCommandValue PrintBucketInformation(QBucket bucket)
            {
                var info = new CommandValues.GenericInfoCommandValue();
                info.Uuid = bucket.Shortname;
                info.Message = "New bucket created,  ";
                return info;
            }

            public async override Task<CommandValues.GenericInfoCommandValue> Create(IConfiguration iconfig, QarnotSDK.Connection connect, CancellationToken ct)
            {
                BucketConfiguration config = iconfig as BucketConfiguration;
                QBucket bucket = await this.CreateBucket(config, connect, ct);
                await this.LaunchBucket(bucket, ct);

                List<Task> listOfTask = new List<Task>();
                listOfTask.AddRange(config.LocalPathFiles.Select(localFile => bucket.UploadFileAsync(localFile, config.RemoteRelativePath, cancellationToken: ct)));
                listOfTask.AddRange(config.LocalPathFolders.Select(localFolder => bucket.UploadFolderAsync(localFolder, config.RemoteRelativePath, cancellationToken: ct)));
                await Task.WhenAll(listOfTask);

                return this.PrintBucketInformation(bucket);
            }
        }
    }
}
