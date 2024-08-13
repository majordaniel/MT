
using Newtonsoft.Json;

namespace MTMiddleware.Data.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("data")]
        public T? Data { get; set; }
    }
}