namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class ConnectionOverrider: Connection
    {
        public ConnectionOverrider(string url, string storage, string token, FakeHTTPHandler httpClientHandler = null ): base (url, storage, token, httpClientHandler:httpClientHandler)
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

        public FakeConnectionWrapper(FakeHTTPHandler fakeHTTP)
        {
            FakeHTTP = fakeHTTP;
        }

        public Connection CreateConnection(IConfiguration config)
        {
            return new ConnectionOverrider("http://localhost", "http://localhost", "Token", httpClientHandler: FakeHTTP) {
                StorageAccessKey = "test@test.test"
            };
        }
    }
}