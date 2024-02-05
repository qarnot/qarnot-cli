using System.Text.RegularExpressions;
using QarnotSDK;

namespace QarnotCLI;

public static class Helpers
{
    public static string HomePath =
        Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
            ? Environment.GetEnvironmentVariable("HOME")!
            : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")!;

    public static string LocalConnectionConfigurationPath = Path.Combine(".Qarnot", "cli.config");
    public static string GlobalConnectionConfigurationPath = Path.Combine(HomePath, ".Qarnot", "cli.config");

    public static bool ParseKeyValuePair(string line, out KeyValuePair<string, string> kvp)
    {
        int position = line.IndexOf("=");
        if (position <= 0)
        {
            kvp = default;
            return false;
        }

        var key = line.Substring(0, position);
        var value = line.Substring(position + 1);
        kvp = new(key, value);

        return true;
    }

    public static string? GetConnectionConfigurationPath(ILogger logger, bool forceGlobal = false, bool forceExist = false)
    {
        if (
            !forceGlobal
            && Environment.GetEnvironmentVariable("QARNOT_LOCAL_PATH") is string envLocalPath
            && (!forceExist || File.Exists(envLocalPath))
        )
        {
            return envLocalPath;
        }
        else if (
            Environment.GetEnvironmentVariable("QARNOT_GLOBAL_PATH") is string envGlobalFile
            && (!forceExist || File.Exists(envGlobalFile))
        )
        {
            return envGlobalFile;
        }
        else if (
            !forceGlobal
            && (!forceExist || File.Exists(LocalConnectionConfigurationPath))
        )
        {
            return LocalConnectionConfigurationPath;
        }
        else if (!forceExist || File.Exists(GlobalConnectionConfigurationPath))
        {
            return GlobalConnectionConfigurationPath;
        }
        else
        {
            logger.Warning("No configuration file found");
            return null;
        }
    }

    public static TimeSpan? ConvertDateStringToTimeSpan(string stringToParse)
    {
        DateTime dateValue;

        if (DateTime.TryParseExact(stringToParse, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dateValue))
        {
            return TimeSpan.FromTicks(dateValue.Ticks - DateTime.Now.Ticks);
        }
        else if (DateTime.TryParseExact(stringToParse, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dateValue))
        {
            return TimeSpan.FromTicks(dateValue.Ticks - DateTime.Now.Ticks);
        }

        return null;
    }

    public static TimeSpan? ConvertStringToTimeSpan(string stringToParse)
    {
        Regex regex_day = new Regex(@"^([0-9]+)((\.)([0-9]*)((\:)([0-9]*))?((\:)([0-9]*))?)?$", RegexOptions.Compiled);
        Regex regex_hour = new Regex(@"([0-9]*)(\:([0-9]*))(\:([0-9]*))?$", RegexOptions.Compiled);

        Match match_day = regex_day.Match(stringToParse);
        if (match_day.Success)
        {
            GroupCollection groups = match_day.Groups;
            string day = string.IsNullOrEmpty(groups[1].Value) ? "0" : groups[1].Value;
            string hour = string.IsNullOrEmpty(groups[4].Value) ? "0" : groups[4].Value;
            string minute = string.IsNullOrEmpty(groups[7].Value) ? "0" : groups[7].Value;
            string second = string.IsNullOrEmpty(groups[10].Value) ? "0" : groups[10].Value;
            return new TimeSpan(int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second));
        }

        Match match_hour = regex_hour.Match(stringToParse);
        if (match_hour.Success)
        {
            GroupCollection groups = match_hour.Groups;
            string hour = string.IsNullOrEmpty(groups[1].Value) ? "0" : groups[1].Value;
            string minute = string.IsNullOrEmpty(groups[3].Value) ? "0" : groups[3].Value;
            string second = string.IsNullOrEmpty(groups[5].Value) ? "0" : groups[5].Value;
            return new TimeSpan(0, int.Parse(hour), int.Parse(minute), int.Parse(second));
        }

        return null;
    }

    public static TimeSpan ParseTimeSpanString(string stringToParse)
    {
        TimeSpan? maxWallTime = ConvertDateStringToTimeSpan(stringToParse);
        if (!maxWallTime.HasValue)
        {
            maxWallTime = ConvertStringToTimeSpan(stringToParse);
        }

        if (maxWallTime.HasValue)
        {
            return maxWallTime.Value;
        }

        throw new Exception();
    }

    public static QarnotSDK.HardwareConstraints? BuildHardwareConstraints(
        uint? minimumCoreCount,
        uint? maximumCoreCount,
        decimal? minimumRamCoreRatio,
        decimal? maximumRamCoreRatio,
        List<string>? specificHardware,
        bool? gpuHardware,
        bool? ssdHardware,
        bool? noSsdHardware,
        decimal? minimumRamHardware,
        decimal? maximumRamHardware,
        string? cpuModelHardware
    )
    {
        var hardwareConstraints = new QarnotSDK.HardwareConstraints();

        if (minimumCoreCount is not null) 
            hardwareConstraints.Add(new MinimumCoreHardware((int)minimumCoreCount.Value));
        if (maximumCoreCount is not null)
            hardwareConstraints.Add(new MaximumCoreHardware((int)maximumCoreCount.Value));

        if (minimumRamCoreRatio is not null)
            hardwareConstraints.Add(new MinimumRamCoreRatioHardware(minimumRamCoreRatio.Value));
        if (maximumRamCoreRatio is not null)
            hardwareConstraints.Add(new MaximumRamCoreRatioHardware(maximumRamCoreRatio.Value));

        if (specificHardware is not null)
        {
            foreach (var s in specificHardware)
            {
                hardwareConstraints.Add(new SpecificHardware(s));
            }
        }
        if (ssdHardware ?? false)
            hardwareConstraints.Add(new SSDHardware());
        if (noSsdHardware ?? false)
            hardwareConstraints.Add(new NoSSDHardware());
        if (gpuHardware ?? false)
            hardwareConstraints.Add(new GpuHardware());
        if (minimumRamHardware is not null)
            hardwareConstraints.Add(new MinimumRamHardware(minimumRamHardware.Value));
        if (maximumRamHardware is not null)
            hardwareConstraints.Add(new MaximumRamHardware(maximumRamHardware.Value));
        if (cpuModelHardware is not null)
            hardwareConstraints.Add(new CpuModelHardware(cpuModelHardware));

        return hardwareConstraints;
    }

    public static List<T> CoalesceEmpty<T>(List<T>? lhs, List<T> rhs) =>
        lhs is null || !lhs.Any() ? rhs : lhs;
}
