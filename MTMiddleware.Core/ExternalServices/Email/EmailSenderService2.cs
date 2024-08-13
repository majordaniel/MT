
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Core.ViewModels;
using MTMiddleware.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Elastic.Apm.Api;
using MediatR;

namespace MTMiddleware.Core.ExternalServices
{
    public class EmailSenderService2 : IEmailSenderService2
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public EmailSenderService2(ILogger<EmailSenderService2> logger, IOptions<AppSettings> appSettings, HttpClient client, IConfiguration config)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpClient = client;
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailRequestViewModel request)
        {
            //try
            //{
            //    _logger.LogInformation($"Trying to send email to {request.To}");

            //    var content = new MultipartFormDataContent();

            //    content.Add(new StringContent(_appSettings.FromEmail), "From");
            //    content.Add(new StringContent(request.To), "To");
            //    content.Add(new StringContent(request.Subject), "Subject");
            //    content.Add(new StringContent(request.MessageBody), "MessageBody");
            //    //content.Add(new StringContent(request.Entity), "Entity");

            //    var response = await _httpClient.PostAsync($"{_appSettings.NotificationUrl}", content);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        _logger.LogInformation($"Email sent Successfully to {request.To}");
            //        return true;
            //    }

            //    _logger.LogError($"Email sending FAILED to {request.To} Response : \n" + response.Content.ReadAsStringAsync());

            //    return false;

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Email sending FAILED to {request.To} Error : \n" + ex.ToString());
            //    return false;
            //}



            try
            {

                _logger.LogInformation($"Trying to send email to {request.To}");



                using (var httpClient = new HttpClient())
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        // Add form fields to the FormData content
                        formData.Add(new StringContent(request.To), "To");
                        formData.Add(new StringContent(request.Subject), "Subject");
                        formData.Add(new StringContent(request.MessageBody), "MessageBody");

                        // Add file to the FormData content
                        //byte[] fileBytes = System.IO.File.ReadAllBytes("example.txt"); // Read file bytes
                        //var fileContent = new ByteArrayContent(fileBytes);


                        //You can take this as parameter in the method to accept input for attachment
                        //   fileContent.Headers.Add("Content-Type", "application/octet-stream"); // Set content type if needed
                        //  formData.Add(fileContent, "file", "example.txt"); // Assuming "example.txt" is the file name

                        // Make the HTTP POST request
                        var response = await httpClient.PostAsync(_appSettings.NotificationUrl, formData);

                        // Check if the request was successful


                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation($"Email sent Successfully to {request.To}");
                            return true;
                        }

                        _logger.LogError($"Email sending FAILED to {request.To} Response : \n" + response.Content.ReadAsStringAsync());


                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending FAILED to {request.To} Error : \n" + ex.ToString());
                return false;
            }


        }
     





    }
}
