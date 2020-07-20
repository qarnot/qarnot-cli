namespace QarnotCLI
{
    using System.IO;
    using System.Linq;

    public interface IConfigurationFileReader
    {
        APIConnectionInformation ReadFile(string filePath);
    }

    public class ConfigurationFileReader : IConfigurationFileReader
    {
        public void GetValue(string line, APIConnectionInformation connectionVariables)
        {
            if (!line.Contains("="))
            {
                return;
            }

            var splitline = line.Split('=');
            switch (splitline[0])
            {
                case "token":
                    connectionVariables.Token = splitline[1];
                    break;
                case "uri":
                    connectionVariables.ApiUri = splitline[1];
                    break;
                case "storage":
                    connectionVariables.StorageUri = splitline[1];
                    break;
                case "account-email":
                    connectionVariables.AccountEmail = splitline[1];
                    break;
                case "force-path":
                    connectionVariables.SetForcePathStyleString(splitline[1]);
                    break;
            }
        }

        public APIConnectionInformation ParseLines(string[] lines)
        {
            APIConnectionInformation connectionVariables = new APIConnectionInformation();

            if (lines != null)
            {
                lines.ToList().ForEach(line => GetValue(line, connectionVariables));
            }

            return connectionVariables;
        }

        public virtual APIConnectionInformation ReadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new FileNotFoundException();
            }

            CLILogs.Debug("File info get " + filePath);

            string[] lines = File.ReadAllLines(filePath);
            return this.ParseLines(lines);
        }
    }
}
