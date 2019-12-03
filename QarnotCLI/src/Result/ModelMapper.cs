namespace QarnotCLI
{
    using QarnotSDK;

    /// <summary>
    /// Class used to create an object dedicate to the print the result value
    /// Close to a custom "ICommandValue" design pattern Factory
    /// allowing personal type objects
    /// it create the ICommandValue object and hydrate it with the needed information
    /// this object can be transform in the Format class to have a custom value
    /// See Format "TABLE".
    /// </summary>
    /// <typeparam name="T">The generic type parameter: QObject.</typeparam>
    public interface IModelMapper<T>
    {
        ICommandValue CreateObj(T elem);
    }

    public class GenericInfoModelMapper : IModelMapper<(string uuid, string message)>
    {
        public ICommandValue CreateObj((string uuid, string message) infoMessage)
        {
            var command = new CommandValues.GenericInfoCommandValue()
            {
                Uuid = infoMessage.uuid,
                Message = infoMessage.message,
            };
            return command;
        }
    }

    public class TaskModelMapper : IModelMapper<QTask>
    {
        public ICommandValue CreateObj(QTask task)
        {
            var command = new CommandValues.TaskCommandValue()
            {
                Name = task.Name,
                Shortname = task.Shortname,
                Uuid = task.Uuid.ToString(),
                Profile = task.Profile,
                State = task.State,
                InstanceCount = task.InstanceCount,
            };
            return command;
        }
    }

    public class JobModelMapper : IModelMapper<QJob>
    {
        public ICommandValue CreateObj(QJob job)
        {
            var command = new CommandValues.JobCommandValue()
            {
                Name = job.Name,
                Shortname = job.Shortname,
                Uuid = job.Uuid.ToString(),
                State = job.State,
            };
            return command;
        }
    }

    public class PoolModelMapper : IModelMapper<QPool>
    {
        public ICommandValue CreateObj(QPool pool)
        {
            var command = new CommandValues.PoolCommandValue()
            {
                Name = pool.Name,
                Shortname = pool.Shortname,
                Uuid = pool.Uuid.ToString(),
                Profile = pool.Profile,
                State = pool.State,
                NodeCount = pool.NodeCount,
            };
            return command;
        }
    }

    public class BucketModelMapper : IModelMapper<QBucket>
    {
        public BucketModelMapper(IFormatter bytesFormater)
        {
            BytesFormat = bytesFormater;
        }

        private IFormatter BytesFormat { get; }

        public ICommandValue CreateObj(QBucket bucket)
        {
            var command = new CommandValues.BucketCommandValue()
            {
                Shortname = bucket.Shortname,
                FileCount = bucket.FileCount.ToString(),
                UsedSpaceBytes = BytesFormat.ConvertToHumanReadable(bucket.UsedSpaceBytes),
            };
            return command;
        }
    }

    public class ApiSettingsModelMapper : IModelMapper<ApiSettings>
    {
        public ICommandValue CreateObj(ApiSettings apiSetting)
        {
            var command = new CommandValues.ApiSettingsCommandValue();
            return command;
        }
    }

    public class UserInformationModelMapper : IModelMapper<UserInformation>
    {
        public ICommandValue CreateObj(UserInformation apiSetting)
        {
            var command = new CommandValues.UserInformationCommandValue();
            return command;
        }
    }
}