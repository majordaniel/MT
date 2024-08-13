using System.Collections.Generic;

namespace UtilityLibrary.Models
{
    public class GenericResponse
    {
        public string Code { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public object? Data { get; set; } = null;
    }

    public class Response<T> : GenericResponse
    {
        public new T? Data { get; set; }
    }

    public class PagedModel<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public IEnumerable<T>? Items { get; set; }
    }


    public class Result<T>
    {
        public static Response<T> Success(T t)
        {
            return new()
            {
                Code = "00",
                Data = t,
                Description = "Successful"
            };
        }
        public static Response<T> Failure(string message = "Failed", string responseCode = "99")
        {
            return new()
            {
                Code = responseCode,
                Description = string.IsNullOrWhiteSpace(message) ? "Failed" : message,
                Data = default
            };
        }

    }
}
