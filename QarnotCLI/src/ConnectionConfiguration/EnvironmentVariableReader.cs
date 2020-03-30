namespace QarnotCLI
{
    using System;

    public interface IEnvironmentVariableReader
    {
        void RetrieveEnvironmentInformation(APIConnectionInformation api);
        bool GetEnvironmentVariableBoolOrElse(string envName, bool elseValue = false);
    }

    public class EnvironmentVariableReader : IEnvironmentVariableReader
    {
        private string GentEnvVariable(string envName)
        {
            return Environment.GetEnvironmentVariable(envName);
        }

        public bool GetEnvironmentVariableBoolOrElse(string envName, bool elseValue = false)
        {
            try
            {
                var value = GentEnvVariable(envName);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return elseValue;
                }

                return Convert.ToBoolean(value);
            }
            catch (Exception)
            {
                return elseValue;
            }
        }

        public void RetrieveEnvironmentInformation(APIConnectionInformation api)
        {
            api.SetToken = GentEnvVariable("QARNOT_CLIENT_TOKEN");
            api.SetApiUri = GentEnvVariable("QARNOT_CLUSTER_URL");
            api.SetStorageUri = GentEnvVariable("QARNOT_STORAGE_URL");
            api.SetForcePathStyleString(GentEnvVariable("QARNOT_USE_STORAGE_PATH_STYLE"));
        }
    }
}