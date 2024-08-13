using Newtonsoft.Json;
using System.Net;
using UtilityLibrary.Models;

namespace MTMiddleware.Api.Middleware
{
    public class ExceptionLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionLoggingMiddleware(ILogger<ExceptionLoggingMiddleware> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        // get the request and pass to logger
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // log error to file or serilog
                _logger.LogError(ex.Message);
                await WriteExceptionResponseAsync(context, ex);
            }
        }

        // handle excepts on http requests
        private Task WriteExceptionResponseAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception.Message);

            // set content type to json
            context.Response.ContentType = "application/json";
            // set http status code to error 500
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string msg = _env.IsDevelopment() ? exception.Message : "Request failed to process.";
            var errorResponse = new GenericResponse() { Code = "96", Description = $"{msg} (Kindly try again or contact support)" };
            // serialize object to json
            string jsonMsg = JsonConvert.SerializeObject(errorResponse);

            return context.Response.WriteAsync(jsonMsg);
        }
    }
}
