using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.Helpers
{
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class RestClientService
    {
        private RestClient client;

        public RestClientService(string baseUrl)
        {
            client = new RestClient(baseUrl);
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            IRestResponse<T> response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                throw new ApplicationException("Error occurred while executing the request", response.ErrorException);
            }
            return response.Data;
        }

        public void AddHeader(string name, string value)
        {
            client.AddDefaultHeader(name, value);
        }

        public RestRequest CreateRequest(string resource, Method method)
        {
            return new RestRequest(resource, method);
        }

        public void AddParameter(RestRequest request, string name, object value, ParameterType type = ParameterType.GetOrPost)
        {
            request.AddParameter(name, value, type);
        }

        public void AddFile(RestRequest request, string name, byte[] fileBytes, string fileName, string contentType)
        {
            request.AddFileBytes(name, fileBytes, fileName, contentType);
        }

        public void AddFile(RestRequest request, string name, string filePath, string contentType)
        {
            request.AddFile(name, filePath, contentType);
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Initialize RestClientService with base URL
    //        RestClientService restClient = new RestClientService("http://10.100.33.110:9003/api");

    //        // Add default headers if needed
    //        restClient.AddHeader("accept", "text/plain");
    //        restClient.AddHeader("Content-Type", "multipart/form-data");

    //        // Example usage for sending a POST request with form data
    //        SendEmail(restClient);
    //    }

    //    static void SendEmail(RestClientService restClient)
    //    {
    //        // Create request
    //        RestRequest request = restClient.CreateRequest("Email/Send", Method.POST);

    //        // Add form parameters
    //        restClient.AddParameter(request, "To", "daniel.ogwu@fbnquestmb.com");
    //        restClient.AddParameter(request, "Subject", "test");
    //        restClient.AddParameter(request, "MessageBody", "test body");

    //        // Execute the request
    //        var response = restClient.Execute<object>(request);

    //        // Check response
    //        if (response != null)
    //        {
    //            Console.WriteLine("Email sent successfully.");
    //        }
    //        else
    //        {
    //            Console.WriteLine("Error sending email.");
    //        }
    //    }
    //}

}
