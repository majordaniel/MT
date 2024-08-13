
using Microsoft.Extensions.Configuration;
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Core.Helpers.EmailSender.Interfaces;
using MTMiddleware.Data.Models;
using MTMiddleware.Shared.Models;
using Newtonsoft.Json;
using MTMiddleware.Core.Helpers.InternetClient;
using MTMiddleware.Shared.Models;

namespace MTMiddleware.Core.Helpers.EmailSender
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _config;

        public EmailSenderService( ILogger<EmailSenderService> logger, IHttpClientService httpClientService, IConfiguration config)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _config = config;
        }

        public async Task<Response<bool>> SendEmailAsync(EmailSettings settings)
        {
            try
            {
                string fromEmail = _config.GetValue<string>("NotificationFromEmail");
                string url = _config.GetValue<string>("ServiceEndpoints:EmailNotificationUrl");

                _logger.LogInformation($"Attempting to send email to {settings.To}");

                var content = new MultipartFormDataContent();

                content.Add(new StringContent(fromEmail), "From");
                content.Add(new StringContent(settings.To), "To");
                content.Add(new StringContent(settings.Subject), "Subject");
                content.Add(new StringContent(settings.MessageBody), "MessageBody");
                content.Add(new StringContent(settings.Entity), "Entity");

                var response = await _httpClientService.MakeHttpCall(HttpMethod.Post, url, content);
                var responseData = JsonConvert.DeserializeObject<ApiResponse<EmailResponse>>(await response.Content.ReadAsStringAsync());

                if (responseData != null && responseData.Data != null && response.IsSuccessStatusCode && responseData.Code == "00")
                {
                    _logger.LogInformation($"Email sent successfully to {settings.To}");

                    return new Response<bool>() { Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(), Description = ResponseEnum.OperationCompletedSuccesfully.Description(), Data = true };
                }

                _logger.LogError($"Unable to send email to {settings.To} Response : \n" + response.Content.ReadAsStringAsync());

                return new Response<bool>() { Code = ResponseEnum.UnableToSendEmail.ResponseCode(), Description = ResponseEnum.UnableToSendEmail.Description(), Data = false };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending to {settings.To} FAILED. Error : \n" + ex.ToString());

                return new Response<bool>() { Code = ResponseEnum.FailedToSendEmail.ResponseCode(), Description = ResponseEnum.FailedToSendEmail.Description(), Data = false };
            }
        }
    }
}
