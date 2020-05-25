namespace QarnotCLI
{
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// retrive the Assembly attribute information
    /// </summary>
    public interface IPackageDetails
    {
        /// <summary>
        /// The hash value of the last commit
        /// </summary>
        /// <value>hash value commit</value>
        string CommitHash { get; }

        /// <summary>
        /// The assembly name
        /// </summary>
        /// <value>assembly name</value>
        string AssemblyName { get; }

        /// <summary>
        /// The assembly version
        /// </summary>
        /// <value>assembly version</value>
        string AssemblyVersion { get; }

        /// <summary>
        /// The builder used
        /// </summary>
        /// <value>Builder name</value>
        string FrameworkName { get; }

        /// <summary>
        /// The build date
        /// </summary>
        /// <value>build date</value>
        string BuildDate { get; }
    }

    public class PackageDetails : IPackageDetails
    {
        /// <summary>
        /// The assembly last git commit short hash
        /// </summary>
        /// <value>git commit hash</value>
        public string CommitHash
        {
            get => ThisAssembly.Git.Commit;
        }

        /// <summary>
        /// The Project name
        /// </summary>
        /// <value>Project name</value>
        public string AssemblyName
        {
            get => Assembly.GetEntryAssembly().GetName().Name;
        }

        /// <summary>
        /// The project version
        /// </summary>
        /// <value>Project version</value>
        public string AssemblyVersion
        {
            get => Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// The builder name
        /// </summary>
        /// <value>Framework name</value>
        public string FrameworkName
        {
            get => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        }

        /// <summary>
        /// The date of build
        /// </summary>
        /// <value>Build date</value>
        public string BuildDate
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttributesData().First(x => x.AttributeType.Name == "TimestampAttribute");
                return (string)attribute.ConstructorArguments.First().Value;
            }
        }
    }
}