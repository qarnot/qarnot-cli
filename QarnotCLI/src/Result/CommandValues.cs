namespace QarnotCLI
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface regroupping all the command values objects.
    /// </summary>
    public interface ICommandValue
    {
    }

    /// <summary>
    /// The interface for the command values return to be a list.
    /// </summary>
    public interface ICollectionCommandValue : ICommandValue
    {
    }

    /// <summary>
    /// Object create from raw QObject send by the SDK API
    /// this object get only the needed values
    /// And can be transform in the Format class to have a custom implementation
    /// See Format "TABLE".
    /// </summary>
    public class CommandValues
    {
        public interface IInfoCommandValue : ICommandValue
        {
            string Uuid { get; set; }

            string Message { get; set; }
        }

        public class StringCommandValue : ICommandValue
        {
            public string StringValue { get; set; }
        }

        public class GenericInfoCommandValue : IInfoCommandValue
        {
            public string Uuid { get; set; }

            public string Message { get; set; }
        }

        public class BucketInfoCommandValue : ICommandValue
        {
            public string Shortname { get; set; }

            public string TotalFileCount { get; set; }

            public string TotalUsedSpaceBytes { get; set; }

            public BucketInfoCommandValue()
            {
                this.BucketFilesDetail = new List<string>();
            }

            public List<string> BucketFilesDetail { get; set; }
        }

        public class TaskCommandValue : ICollectionCommandValue
        {
            public string Name { get; set; }

            public string State { get; set; }

            public string Uuid { get; set; }

            public string Shortname { get; set; }

            public string Profile { get; set; }

            public uint InstanceCount { get; set; }
        }

        public class PoolCommandValue : ICollectionCommandValue
        {
            public string Name { get; set; }

            public string State { get; set; }

            public string Uuid { get; set; }

            public string Shortname { get; set; }

            public string Profile { get; set; }

            public uint NodeCount { get; set; }
        }

        public class JobCommandValue : ICollectionCommandValue
        {
            public string Name { get; set; }

            public string State { get; set; }

            public string Uuid { get; set; }

            public string Shortname { get; set; }
        }

        public class BucketCommandValue : ICollectionCommandValue
        {
            public string Shortname { get; set; }

            public string FileCount { get; set; }

            public string UsedSpaceBytes { get; set; }
        }

        public class ApiSettingsCommandValue : ICommandValue
        {
            public ApiSettingsCommandValue()
            {
                this.AccountInformation = new Dictionary<string, string>();
            }

            public Dictionary<string, string> AccountInformation { get; set; }
        }

        public class UserInformationCommandValue : ICommandValue
        {
            public UserInformationCommandValue()
            {
                this.AccountInformation = new Dictionary<string, string>();
            }

            public Dictionary<string, string> AccountInformation { get; set; }
        }
    }
}