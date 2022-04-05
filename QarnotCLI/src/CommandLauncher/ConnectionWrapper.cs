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
            if (config.UnsafeSslCertificate || (config?.ApiConnection?.UnsafeSsl ?? false))
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

            var enableSanitization = !(config?.ApiConnection?.DisableBucketPathsSanitization ?? false);
            if (enableSanitization)
            {
                CLILogs.Info("Bucket path sanitization is enabled (default). Use --no-sanitize-bucket-paths argument to disable.");
            }

            return new Connection(
                apiUri,
                config?.ApiConnection?.StorageUri,
                config?.ApiConnection?.Token,
                client,
                forceStoragePathStyle: config?.ApiConnection?.ForcePathStyle ?? false,
                sanitizeBucketPaths: enableSanitization,
                showBucketWarnings: false) // the cli will display custom warnings
            {
                StorageAccessKey = config?.ApiConnection?.AccountEmail,
                S3HttpClientFactory = config?.ApiConnection?.StorageUnsafeSsl ?? false ? new UnsafeS3HttpClientFactory() : null
            };
        }
    }
}