namespace QarnotCLI
{
    using System;

    /// <summary>
    /// Class contain the debbug print object functions.
    /// </summary>
    public static class PrintInfoDebug
    {
        public static void IConfiguration(IConfiguration config)
        {
            var jd = new FormatterFactory.JsonFormatter();
            CLILogs.Debug("Config format");
            CLILogs.Debug("Type " + Enum.GetName(typeof(ConfigType), config.Type));
            CLILogs.Debug("Command " + Enum.GetName(typeof(CommandApi), config.Command));
            CLILogs.Debug(jd.Format(config));
        }

        public static void DebugException(Exception ex)
        {
            CLILogs.Debug("InnerException : " + ex?.InnerException?.ToString());
            CLILogs.Debug("StackTrace : " + ex?.StackTrace);
            CLILogs.Debug("Source : " + ex?.Source);
            CLILogs.Debug("Data : " + ex?.Data.ToString());
            CLILogs.Debug("TargetSite : " + ex?.TargetSite?.ToString());
            CLILogs.Error(ex?.Message);
        }
    }
}
