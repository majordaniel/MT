
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MTMiddleware.Core.Helpers.InternetClient
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> MakeHttpCall(HttpMethod httpMethod, string url, object? content = null, Dictionary<string, string>? customHeaders = null, string? session = null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            return await _httpClient.SendAsync(BuildRequest(httpMethod, url, content, customHeaders));
        }

        #region PrivateMethods
        private static HttpRequestMessage BuildRequest(HttpMethod httpMethod, string url, object? content = null, Dictionary<string, string>? customHeaders = null)
        {
            var request = new HttpRequestMessage(httpMethod, new Uri($"{url}"));
            if (content is not null)
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            if (customHeaders is not null)
            {
                foreach (var header in customHeaders)
                    request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        #endregion
    }
}
