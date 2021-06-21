namespace QarnotCLI
{
    /// <summary>
    /// Type argument of the Qarnot element.
    /// </summary>
    public enum ConfigType
    {
        /// <summary>
        /// Type QPool
        /// </summary>
        Pool,

        /// <summary>
        /// Type QTask
        /// </summary>
        Task,

        /// <summary>
        /// Type QJob
        /// </summary>
        Job,

        /// <summary>
        /// Type QBucket
        /// </summary>
        Bucket,

        /// <summary>
        /// Type contain : QPool, QTask, QJob, QBucket
        /// </summary>
        All,

        /// <summary>
        /// Type Config of the CLI, Not a Qarnot Object
        /// </summary>
        Config,

        /// <summary>
        /// Type Accont
        /// </summary>
        Account,
    }

    /// <summary>
    /// Type argument of the command used by the ConfigElem.
    /// </summary>
    public enum CommandApi
    {
        /// <summary>
        /// Type Set a value
        /// </summary>
        Set,

        /// <summary>
        /// Type Create a new element
        /// </summary>
        Create,

        /// <summary>
        /// Type Get info for one or more elements
        /// </summary>
        Info,

        /// <summary>
        /// Type Get info for one buckets
        /// </summary>
        InfoBucket,

        /// <summary>
        /// Type Get summaries for one or more elements
        /// </summary>
        List,

        /// <summary>
        /// Type Wait an element
        /// </summary>
        Wait,

        /// <summary>
        /// Type Cancel an element
        /// </summary>
        Abort,

        /// <summary>
        /// Type Delete an element
        /// </summary>
        Delete,

        /// <summary>
        /// Type Sync from bucket
        /// </summary>
        SyncFrom,

        /// <summary>
        /// Type Sync to bucket
        /// </summary>
        SyncTo,

        /// <summary>
        /// Type Upload an element to the Api
        /// </summary>
        Upload,

        /// <summary>
        /// Type Download an element to the Api
        /// </summary>
        Download,

        /// <summary>
        /// Synchronize the storage of an Api element
        /// </summary>
        UpdateResources,

        /// <summary>
        /// Type Remove a part of element
        /// </summary>
        Remove,

        /// <summary>
        /// Type Show the Stdout
        /// </summary>
        GetStdout,

        /// <summary>
        /// Type Show the Stderr
        /// </summary>
        GetStderr,

        /// <summary>
        /// Type Snapshot the task results
        /// </summary>
        Snapshot,
    }
}