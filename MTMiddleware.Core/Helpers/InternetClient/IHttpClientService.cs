using MTMiddleware.Core.Helpers.Autofac;

namespace MTMiddleware.Core.Helpers.InternetClient
{
    public interface IHttpClientService : IAutoDependencyCore
    {
        Task<HttpResponseMessage> MakeHttpCall(HttpMethod httpMethod, string url, object? content = null, Dictionary<string, string>? customHeaders = null, string? session = null);
    }
}