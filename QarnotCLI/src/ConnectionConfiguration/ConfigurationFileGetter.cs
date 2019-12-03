namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IConfigurationFileGetter
    {
        string ConfigurationFilePath { get; }

        string NewConfigurationFilePath { get; }

        bool CanConfigurationFileBeCreated();

        bool DoesConfigurationFileExist();
    }

    public class ConfigurationFileGetter : IConfigurationFileGetter
    {
        private const string LocalEnvironmentName = "QARNOT_LOCAL_PATH";

        private const string DefaultEnvironmentName = "QARNOT_DEFAULT_PATH";

        private readonly List<ConfigurationPath> ConfigurationPathList = Enum.GetValues(typeof(ConfigurationPath)).Cast<ConfigurationPath>().ToList();

        private static readonly string HomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
            Environment.OSVersion.Platform == PlatformID.MacOSX)
            ? Environment.GetEnvironmentVariable("HOME") + Path.DirectorySeparatorChar
            : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + Path.DirectorySeparatorChar;

        public ConfigurationFileGetter()
        {
        }

        private delegate bool TestPath(string path);

        /// <summary>
        /// Internal configuration Enum To set path.
        /// </summary>
        private enum ConfigurationPath
        {
            /// <summary>
            /// Environnement local Path.
            /// </summary>
            EnvLocal,

            /// <summary>
            /// Environnement global Path.
            /// </summary>
            EnvGlobal,

            /// <summary>
            /// Default local Path.
            /// </summary>
            DefaultLocal,

            /// <summary>
            /// Default global Path.
            /// </summary>
            DefaultGlobal,
        }

        public static string LocalPathName { get; } = Path.Combine(".", ".Qarnot", "cli.config");

        public static string DefaultPathName { get; } = HomePath + Path.Combine(".Qarnot", "cli.config");

        public string ConfigurationFilePath
        {
            get
            {
                return TestConfigurationFileExist(filePath => ConfigurationFileGetter.DoesFileExist(filePath));
            }
        }

        public string NewConfigurationFilePath
        {
            get
            {
                return TestConfigurationFileExist(filePath => ConfigurationFileGetter.DoesFileExist(filePath)) ??
                        TestConfigurationFileExist(filePath => !string.IsNullOrEmpty(filePath));
            }
        }

        public bool CanConfigurationFileBeCreated()
        {
            return (TestConfigurationFileExist(filePath => ConfigurationFileGetter.DoesFileExist(filePath)) != null) ||
                    (TestConfigurationFileExist(filePath => !string.IsNullOrEmpty(filePath)) != null);
        }

        public bool DoesConfigurationFileExist()
        {
            return TestConfigurationFileExist(filePath => ConfigurationFileGetter.DoesFileExist(filePath)) != null;
        }

        private string TestConfigurationFileExist(TestPath testPath)
        {
            foreach (ConfigurationPath suit in ConfigurationPathList)
            {
                string filePath = this.GetOneConfigPath(suit);
                if (testPath(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        public virtual string GetEnvironmentVariableValue(string envVariableName)
        {
            return Environment.GetEnvironmentVariable(envVariableName);
        }

        private string GetOneConfigPath(ConfigurationPath config)
        {
            switch (config)
            {
                case ConfigurationPath.EnvLocal:
                    return this.GetEnvironmentVariableValue(ConfigurationFileGetter.LocalEnvironmentName);
                case ConfigurationPath.EnvGlobal:
                    return this.GetEnvironmentVariableValue(ConfigurationFileGetter.DefaultEnvironmentName);
                case ConfigurationPath.DefaultLocal:
                    return ConfigurationFileGetter.LocalPathName;
                case ConfigurationPath.DefaultGlobal:
                    return ConfigurationFileGetter.DefaultPathName;
                default:
                    throw new IndexOutOfRangeException($"case '{config}' out of range");
            }
        }

        public static bool DoesFileExist(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }
    }
}
