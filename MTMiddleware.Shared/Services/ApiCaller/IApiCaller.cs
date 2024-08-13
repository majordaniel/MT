using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.Services
{
    public interface IApiCaller
    {
        Task<ApiResult> GetAsync(string url, IDictionary<string, string> headers);
        Task<ApiResult> PostAsync(string url, object content, IDictionary<string, string> headers);
        Task<ApiResult> PutAsync(string url, object content, IDictionary<string, string> headers);

        Task<TResult> PostAsync<TResult>(string url, List<KeyValuePair<string, string>> postData, IDictionary<string, string> headers, string headerAccept);
    }
}
