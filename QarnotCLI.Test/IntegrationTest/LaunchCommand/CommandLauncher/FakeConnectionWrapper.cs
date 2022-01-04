namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class ConnectionOverrider: Connection
    {
        public ConnectionOverrider(string url, string storage, string token, FakeHTTPHandler httpClientHandler = null, bool sanitizeBucketPaths = true): base (url, storage, token, httpClientHandler:httpClientHandler, sanitizeBucketPaths: sanitizeBucketPaths)
        {
            ;
        }

        public override async Task<QBucket> RetrieveBucketAsync(string bucketName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(1);
            return null;
        }
        public override async Task<List<QBucket>> RetrieveBucketsAsync(bool retrieveBucketStats, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(1);
            return new List<QBucket>();
        }
    }

    public class FakeConnectionWrapper : IConnectionWrapper
    {
        public FakeHTTPHandler FakeHTTP { get; set; }

        public IConfiguration FakeConfig { get; set; }

        public FakeConnectionWrapper(FakeHTTPHandler fakeHTTP, IConfiguration fakeConfig = default)
        {
            FakeHTTP = fakeHTTP;
            FakeConfig = fakeConfig;
        }

        public Connection CreateConnection(IConfiguration config)
        {
            return new ConnectionOverrider("http://localhost", "http://localhost", "Token", httpClientHandler: FakeHTTP, !FakeConfig?.ApiConnection.DisableBucketPathsSanitization ?? true) {
                StorageAccessKey = "test@test.test"
            };
        }
    }
}