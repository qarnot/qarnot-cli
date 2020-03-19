namespace QarnotCLI
{
    using QarnotSDK;

    public interface IConnectionWrapper
    {
        Connection CreateConnection(IConfiguration config);
    }

    public class ConnectionWrapper : IConnectionWrapper
    {
        public Connection CreateConnection(IConfiguration config)
        {
            string apiUri = config.ApiConnection.ApiUri == null ? "https://api.qarnot.com" : config.ApiConnection.ApiUri;
            return new Connection(apiUri, config.ApiConnection.StorageUri, config.ApiConnection.Token);
        }
    }
}