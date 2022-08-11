namespace QarnotCLI.Test
{
    using System.Net;
    using System.Collections.Generic;
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

        public List<HttpRequestMessage> Request = new List<HttpRequestMessage>();

        public string ReturnMessage { get; set; } = "{\"Your\":\"response\"}";
        public HttpStatusCode ReturnStatusCode { get; set; } = HttpStatusCode.Accepted;

        public List<string> ReturnStatusCodeList { get; set; } = null;
        public List<string> ReturnMessageList { get; set; } = null;

        private int ReturnMessageListIndex = 0;
        private int ReturnStatusCodeListIndex = 0;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request.Add(request);

            var response = await Task.FromResult(new HttpResponseMessage(ReturnStatusCode)).ConfigureAwait(false);
            var message = "";
            if (ReturnMessageList != null)
            {
                message = ReturnMessageList[ReturnMessageListIndex % ReturnMessageList.Count];
                ReturnMessageListIndex += 1;
                if (ReturnStatusCodeList != null)
                {
                    message = ReturnStatusCodeList[ReturnStatusCodeListIndex % ReturnStatusCodeList.Count];
                    ReturnStatusCodeListIndex += 1;
                }
            }
            else
            {
                message = ReturnMessage;
            }

            response.Content = new StringContent(message, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
