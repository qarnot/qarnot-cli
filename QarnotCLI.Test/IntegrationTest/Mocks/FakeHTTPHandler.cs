namespace QarnotCLI.Test
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeHTTPHandler : HttpClientHandler
    {
        public FakeHTTPHandler()
            : base()
        {
        }

        public string ReturnMessage { get; set; } = "{\"Your\":\"response\"}";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)).ConfigureAwait(false);
            response.Content = new StringContent(ReturnMessage, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
