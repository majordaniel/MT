using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary;

namespace MTMiddleware.Shared.Services
{
    public class ApiCaller : IApiCaller
    {
        public async Task<ApiResult> GetAsync(string url, IDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Please, provide a valid service url");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(250);

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                using (var response = await httpClient.GetAsync(url))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();

                    ApiResult apiCallerResponse = new ApiResult()
                    {
                        IsSuccessfull = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode,
                        Data = apiResponse
                    };

                    return apiCallerResponse;
                }
            }
        }

        //public async Task<string> GetAsync(string url, IDictionary<string, string> headers)
        //{
        //    if (string.IsNullOrEmpty(url))
        //    {
        //        throw new Exception("Please, provide a valid url");
        //    }

        //    string apiResponse = string.Empty;

        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.Timeout = TimeSpan.FromSeconds(45);

        //        if (headers != null)
        //        {
        //            foreach (var item in headers)
        //            {
        //                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
        //            }
        //        }

        //        using (var response = await httpClient.GetAsync(url))
        //        {
        //            if (response.IsSuccessStatusCode && response.StatusCode.ToString() == "OK")
        //            {
        //                apiResponse = await response.Content.ReadAsStringAsync();
        //            }
        //        }
        //    }

        //    return apiResponse;
        //}

        public async Task<ApiResult> PostAsync(string url, object content, IDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult apiResult = new ApiResult();

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(300);

                StringContent seriallizedContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, ApplicationConstants.APPLICATION_CONTENT_TYPE_JSON);

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                using (var response = await httpClient.PostAsync(url, seriallizedContent))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    apiResult.StatusCode = response.StatusCode;
                    apiResult.Data = result;

                    if (response.IsSuccessStatusCode && response.StatusCode.ToString().ToLower() == "ok")
                    {
                        apiResult.IsSuccessfull = true;
                    }
                    else
                    {
                        apiResult.IsSuccessfull = false;
                    }
                }
            }

            return apiResult;
        }

        public async Task<TResult> PostAsync<TResult>(string url, List<KeyValuePair<string, string>> postData, IDictionary<string, string> headers, string headerAccept)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Please, provide a valid url");
            }

            string apiResponse = string.Empty;

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(45);

                httpClient.DefaultRequestHeaders.Add("Accept", headerAccept);

                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.Clear();

                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            content.Headers.Add(item.Key, item.Value);
                        }
                    }

                    using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
                    {
                        if (response.IsSuccessStatusCode && (response.StatusCode.ToString().ToLower() == "created" || response.StatusCode.ToString().ToLower() == "ok"))
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                }                
            }

            var dResponse = JsonConvert.DeserializeObject<TResult>(apiResponse);

            if (dResponse != null)
            {
                return dResponse;
            }
            else
            {
                return default;
            }
        }

        public async Task<ApiResult> PutAsync(string url, object content, IDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult apiResult = new ApiResult();

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);

                StringContent seriallizedContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, ApplicationConstants.APPLICATION_CONTENT_TYPE_JSON);

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                using (var response = await httpClient.PutAsync(url, seriallizedContent))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    apiResult.StatusCode = response.StatusCode;
                    apiResult.Data = result;

                    if (response.IsSuccessStatusCode && response.StatusCode.ToString().ToLower() == "ok")
                    {
                        apiResult.IsSuccessfull = true;
                    }
                    else
                    {
                        apiResult.IsSuccessfull = false;
                    }
                }
            }

            return apiResult;
        }
    }
}
