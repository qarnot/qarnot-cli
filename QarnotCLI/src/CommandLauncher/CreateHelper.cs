namespace QarnotCLI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using QarnotSDK;

    public interface ICreateHelper
    {
        Dictionary<string, string> CreateConstants(List<string> line);

        Dictionary<string, string> CreateConstraints(List<string> line);

        Dictionary<string, string> CreateLabels(List<string> line);

        List<QAbstractStorage> CreateResources(List<string> names, QarnotSDK.Connection connect, CancellationToken ct);

        QAbstractStorage CreateResult(string names, QarnotSDK.Connection connect, CancellationToken ct);
    }

    public class CreateHelper : ICreateHelper
    {
        private void ParseKeyValuePair(Dictionary<string, string> pairs, string line)
        {
            int position = line.IndexOf("=");
            if (position > 0)
            {
                var key = line.Substring(0, position);
                var value = line.Substring(position + 1);
                pairs[key] = value;
                CLILogs.Debug("  [" + key + "] = [" + value + "]");
            }
        }

        private Dictionary<string, string> ParseKeyValuePairs(List<string> lines, string name)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();

            if (lines == null)
            {
                return pairs;
            }

            CLILogs.Debug(name + " find :");

            lines.ForEach(line => ParseKeyValuePair(pairs, line));

            return pairs;
        }

        public Dictionary<string, string> CreateLabels(List<string> lines)
        {
            return ParseKeyValuePairs(lines, "labels");
        }

        public Dictionary<string, string> CreateConstants(List<string> lines)
        {
            return ParseKeyValuePairs(lines, "constant");
        }

        public Dictionary<string, string> CreateConstraints(List<string> lines)
        {
            return ParseKeyValuePairs(lines, "constraint");
        }

        public List<QAbstractStorage> CreateResources(List<string> names, QarnotSDK.Connection connection, CancellationToken ct)
        {
            return names.Select(name => connection.CreateBucketAsync(name, ct).Result as QAbstractStorage).ToList();
        }

        public QAbstractStorage CreateResult(string name, QarnotSDK.Connection connection, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return connection.CreateBucketAsync(name, ct).Result as QAbstractStorage;
            }

            return null;
        }
    }
}
