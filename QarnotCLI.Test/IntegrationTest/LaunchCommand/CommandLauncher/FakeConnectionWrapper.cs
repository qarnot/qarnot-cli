namespace QarnotCLI.Test
{
    using System;
    using QarnotSDK;

    public class FakeConnectionWrapper : IConnectionWrapper
    {
        public FakeHTTPHandler FakeHTTP { get; set; }

        public FakeConnectionWrapper(FakeHTTPHandler fakeHTTP)
        {
            FakeHTTP = fakeHTTP;
        }

        public Connection CreateConnection(IConfiguration config)
        {
            return new Connection("http://localhost", "http://localhost", "Token", httpClientHandler: FakeHTTP);
        }
    }
}