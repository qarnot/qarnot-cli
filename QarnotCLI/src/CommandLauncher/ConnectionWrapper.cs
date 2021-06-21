using System;
namespace QarnotCLI
{
    using QarnotSDK;
    using System.Net.Http;

    public interface IConnectionWrapper
    {
        Connection CreateConnection(IConfiguration config);
    }

    public class ConnectionWrapper : IConnectionWrapper
    {
        private HttpClientHandler getClientHandler(IConfiguration config)
        {
            if (config.UnsafeSslCertificate)
            {
               return new QarnotSDK.UnsafeClientHandler();
            }
            else if (!string.IsNullOrWhiteSpace(config.CustomSslCertificate))
            {
                return new QarnotSDK.CustomCAClientHandler(config.CustomSslCertificate);
            }

            return null;
        }

        public Connection CreateConnection(IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException();
            string apiUri = config?.ApiConnection?.ApiUri == null ? "https://api.qarnot.com" : config?.ApiConnection?.ApiUri;
            HttpClientHandler client = getClientHandler(config);
            return new Connection(apiUri, config?.ApiConnection?.StorageUri, config?.ApiConnection?.Token, client, forceStoragePathStyle: config?.ApiConnection?.ForcePathStyle ?? false)
            {
                StorageAccessKey = config?.ApiConnection?.AccountEmail
            };
        }
    }
}