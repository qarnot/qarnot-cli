namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using QarnotSDK;
    using static CommandValues;

    public interface ILauncherFactory
    {
        ICommandLauncher CreateLauncher(ConfigType type, CommandApi command);
    }

    public class LauncherFactory : ILauncherFactory
    {
        private readonly IResultFormatter Formatter;

        private readonly IConnectionWrapper ConnectionWrapper;

        public LauncherFactory(IResultFormatter format, IConnectionWrapper connectWrapper)
        {
            Formatter = format;
            ConnectionWrapper = connectWrapper;
        }

        public ICommandLauncher CreateLauncher(ConfigType type, CommandApi command)
        {
            switch (type)
            {
                case ConfigType.Pool:
                    return this.CreatePoolCommandLauncher(command);
                case ConfigType.Task:
                    return this.CreateTaskCommandLauncher(command);
                case ConfigType.Job:
                    return this.CreateJobCommandLauncher(command);
                case ConfigType.Bucket:
                    return this.CreateBucketCommandLauncher(command);
                case ConfigType.All:
                    return this.CreateAllCommandLauncher(command);
                case ConfigType.Config:
                    return this.CreateConfigurationCommandLauncher(command);
                case ConfigType.Account:
                    return this.CreateAccountCommandLauncher(command);
                default:
                    throw new NotImplementedException("Not implemented command : " + Enum.GetName(typeof(ConfigType), type));
            }
        }

        private ICommandLauncher CreatePoolCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Create:
                    return new CreateCommandLauncher(new ApiObjectCreator.CreateNewPool(new CreateHelper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Info:
                    return new CommandGeneric<QPool, QPool>(new QPoolsRetriever(), new GenericCommand<QPool, QPool>(new ConnectionTransformer()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.List:
                    return new CommandGeneric<QPool, PoolCommandValue>(new QPoolsRetriever(), new GenericCollectionCommand<QPool, PoolCommandValue>(new PoolModelMapper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Delete:
                    return new CommandGeneric<QPool, GenericInfoCommandValue>(new QPoolsRetriever(), new DeletePoolCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.UpdateResources:
                    return new CommandGeneric<QPool, GenericInfoCommandValue>(new QPoolsRetriever(), new UpdatePoolResourcesCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Set:
                    return new CommandGeneric<QPool, GenericInfoCommandValue>(new QPoolsRetriever(), new SetPoolCommand(), this.Formatter, this.ConnectionWrapper);
                default:
                    throw new NotImplementedException("Not implemented command for Pool");
            }
        }

        private ICommandLauncher CreateTaskCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Create:
                    return new CreateCommandLauncher(new ApiObjectCreator.CreateNewTask(new CreateHelper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Info:
                    return new CommandGeneric<QTask, QTask>(new QTasksRetriever(), new GenericCommand<QTask, QTask>(new ConnectionTransformer()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Wait:
                    return new CommandGeneric<QTask, GenericInfoCommandValue>(new QTasksRetriever(), new WaitTaskCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.List:
                    return new CommandGeneric<QTask, TaskCommandValue>(new QTasksRetriever(), new GenericCollectionCommand<QTask, TaskCommandValue>(new TaskModelMapper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Abort:
                    return new CommandGeneric<QTask, GenericInfoCommandValue>(new QTasksRetriever(), new AbortTaskCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Delete:
                    return new CommandGeneric<QTask, GenericInfoCommandValue>(new QTasksRetriever(), new DeleteTaskCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.UpdateResources:
                    return new CommandGeneric<QTask, GenericInfoCommandValue>(new QTasksRetriever(), new UpdateTaskResourcesCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.GetStdout:
                    return new CommandGeneric<QTask, string>(new QTasksRetriever(), new StdoutTaskCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.GetStderr:
                    return new CommandGeneric<QTask, string>(new QTasksRetriever(), new StderrTaskCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Snapshot:
                    return new CommandGeneric<QTask, GenericInfoCommandValue>(new QTasksRetriever(), new SnapshotTaskCommand(), this.Formatter, this.ConnectionWrapper);
                default:
                    throw new NotImplementedException("Not implemented command for Task");
            }
        }

        private ICommandLauncher CreateJobCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Create:
                    return new CreateCommandLauncher(new ApiObjectCreator.CreateNewJob(new CreateHelper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Info:
                    return new CommandGeneric<QJob, QJob>(new QJobsRetriever(), new GenericCommand<QJob, QJob>(new ConnectionTransformer()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.List:
                    return new CommandGeneric<QJob, JobCommandValue>(new QJobsRetriever(), new GenericCollectionCommand<QJob, JobCommandValue>(new JobModelMapper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Abort:
                    return new CommandGeneric<QJob, GenericInfoCommandValue>(new QJobsRetriever(), new AbortJobCommand(new JobModelMapper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Delete:
                    return new CommandGeneric<QJob, GenericInfoCommandValue>(new QJobsRetriever(), new DeleteJobCommand(), this.Formatter, this.ConnectionWrapper);
                default:
                    throw new NotImplementedException("Not implemented command for Job");
            }
        }

        private ICommandLauncher CreateBucketCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Create:
                    return new CreateCommandLauncher(new ApiObjectCreator.BucketCreator(new CreateHelper()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.List:
                    return new CommandGeneric<QBucket, BucketCommandValue>(new QBucketsRetriever(), new BucketListResumeCommand(new BytesFormatter()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.InfoBucket:
                    return new CommandGenericOneElem<QBucket, BucketInfoCommandValue>(new QBucketRetriever(), new ListBucketEntriesCommand(new BytesFormatter()), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Delete:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new DeleteBucketCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.SyncFrom:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new SynchronizeLocalFolderFromBucketCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.SyncTo:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new SynchronizeLocalFolderToBucketCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Upload:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new UploadBucketCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Download:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new DownloadBucketCommand(), this.Formatter, this.ConnectionWrapper);
                case CommandApi.Remove:
                    return new CommandGeneric<QBucket, GenericInfoCommandValue>(new QBucketsRetriever(), new RemoveEntityBucketCommand(), this.Formatter, this.ConnectionWrapper);
                default:
                    throw new NotImplementedException("Not implemented command for Bucket");
            }
        }

        private ICommandLauncher CreateAllCommandLauncher(CommandApi command)
        {
            List<ConfigType> listType = new List<ConfigType>()
            {
                ConfigType.Pool,
                ConfigType.Task,
                ConfigType.Job,
                ConfigType.Bucket,
            };

            Dictionary<string, ICommandLauncher> dictLauncher = new Dictionary<string, ICommandLauncher>();
            switch (command)
            {
                case CommandApi.List:
                    break;
                case CommandApi.Abort:
                    break;
                case CommandApi.Delete:
                    break;
                default:
                    throw new NotImplementedException("Not implemented command for All");
            }

            foreach (var type in listType)
            {
                try
                {
                    dictLauncher[Enum.GetName(typeof(ConfigType), type)] = this.CreateLauncher(type, command);
                }
                catch (NotImplementedException ex)
                {
                    CLILogs.Debug(ex.Message);
                }
            }

            return new CommandAll(dictLauncher);
        }

        private ICommandLauncher CreateConfigurationCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Set:
                    throw new NotImplementedException("Set Configuration don't call the SDK");
                default:
                    throw new NotImplementedException("Not implemented command for Config ");
            }
        }

        private ICommandLauncher CreateAccountCommandLauncher(CommandApi command)
        {
            switch (command)
            {
                case CommandApi.Info:
                    return new CommandGenericOneElem<UserInformation, UserInformationCommandValue>(new UserInformationRetriever(), new GetAccountInformationCommand(new UserInformationModelMapper(), new BytesFormatter()), this.Formatter, this.ConnectionWrapper);
                default:
                    throw new NotImplementedException("Not implemented command for Account");
            }
        }
    }
}