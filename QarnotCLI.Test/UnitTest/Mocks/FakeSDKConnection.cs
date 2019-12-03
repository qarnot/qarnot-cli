namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using QarnotSDK;

    public static class FakeSDKConnection
    {
        public static Connection GetFakeConnection()
        {
            return new Connection(ComputeUrl, StorageUrl, Token, HttpHandler);
        }

        private static string StorageUrl { get; } = "http://storage";

        private static string ComputeUrl { get; } = "http://compute";

        private static string Token { get; } = "token";

        private static InterceptingFakeHttpHandler HttpHandler { get; } = new InterceptingFakeHttpHandler();

        public static void ChangeReturnBody(string body)
        {
            HttpHandler.ResponseBody = body;
        }

        private class ParsedRequest
        {
            public string Method { get; set; }

            public string Uri { get; set; }

            public string Content { get; set; }

            public ParsedRequest()
            {
            }

            public override string ToString() => $"[{Method}-{Uri}] : {Content}";
        }

        private class InterceptingFakeHttpHandler : HttpClientHandler
        {
            public InterceptingFakeHttpHandler()
                : base()
            {
            }

            private const string StorageResponse = @"{""storage"":""https://localhost/""}";

            private const string SuccessResponse = "{\"Your\":\"response\"}";

            public string ResponseBody { get; set; }

            private static List<ParsedRequest> ParsedRequests { get; } = new List<ParsedRequest>();

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var parsedRequest = await ParseRequest(request);
                ParsedRequests.Add(parsedRequest);

                if (parsedRequest.Uri.Contains("setting", StringComparison.InvariantCultureIgnoreCase))
                {
                    return await SendHttpAsync(HttpStatusCode.Accepted, StorageResponse);
                }

                return await SendHttpAsync(HttpStatusCode.Accepted, ResponseBody ?? SuccessResponse);
            }

            private async Task<HttpResponseMessage> SendHttpAsync(HttpStatusCode statusCode, string message) =>
                await Task.FromResult(
                    new HttpResponseMessage(statusCode)
                    {
                        Content = new StringContent(message, Encoding.UTF8, "application/json"),
                    });

            private async Task<ParsedRequest> ParseRequest(HttpRequestMessage request)
            {
                var parsedRequest = new ParsedRequest()
                {
                    Method = request.Method.ToString(),
                    Uri = request.RequestUri.ToString(),
                };

                if (request.Content != null)
                {
                    parsedRequest.Content = await request.Content?.ReadAsStringAsync();
                }

                return parsedRequest;
            }
        }
    }
}