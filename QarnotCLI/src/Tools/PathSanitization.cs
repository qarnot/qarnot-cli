using System;

namespace QarnotCLI
{
    public static class PathSanitization
    {
        public static bool IsThePathInvalid(string path)
        {
            var directorySeparators = new[]{'/', '\\'};
            var willBeSanitized = false;
            if (path != default)
            {
                foreach (var separator in directorySeparators)
                {
                    if (path.Contains(String.Format("{0}{0}",separator)))
                    {
                        CLILogs.Warn($"Bucket path should not contain duplicated slashes ('{String.Format("{0}{0}",separator)}')");
                        willBeSanitized = true;
                    }
                    if (path.StartsWith(separator.ToString()))
                    {
                        CLILogs.Warn($"Bucket path should not start with a slash ('{separator}')");
                        willBeSanitized = true;
                    }
                }
                if (willBeSanitized)
                {
                    CLILogs.Warn($"The path {path} is invalid. Fix the path by removing the extra separators or use the argument --no-sanitize-bucket-paths if you are sure about the path");
                }
            }

            return willBeSanitized;
        }
    }
}