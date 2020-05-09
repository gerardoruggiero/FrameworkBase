using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Framework.Web.CustomRequests
{
    public class CustomRequestResponse<T> : IHttpActionResult
    {
        private HttpRequestMessage _request;
        readonly T _itemResponse;
        readonly string _token;

        public CustomRequestResponse(T pItem, string token, HttpRequestMessage request, string pError = "")
        {
            _itemResponse = pItem;
            _token = token;
            _request = request;
            ErrorMsg = pError;
        }

        public T GetItem()
        {
            return _itemResponse;
        }

        public string GetToken()
        {
            return _token;
        }

        public string ErrorMsg { get; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var retorno = new { Item = _itemResponse, Token = _token, ErrorMsg };

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(retorno)),
                RequestMessage = _request,
            };

            return await Task.FromResult(response);
        }
    }
}