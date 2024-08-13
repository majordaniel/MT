using MTMiddleware.Shared.Models;
using MTMiddleware.Shared.Services;
using Newtonsoft.Json;
using RestSharp;
using MTMiddleware.Core.Services;
using MTMiddleware.Core.ExternalServices.Models;
using Microsoft.Extensions.Configuration;

namespace MTMiddleware.Core.ExternalServices
{
    public class ExternalAPIServices : IExternalAPIServices
    {
        private AppSettings? _appSettings;
        private IApiCaller _apiCaller;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;

        public ExternalAPIServices(IConfiguration config,ILogger<UserService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<TransactionHistoryResponseViewModel?> GetCustomerTransactions(string accountNo, string startDate, string endDate)
        {
            //dateformatNew = 05-10-2017
            var result = new TransactionHistoryResponseViewModel();

            var GetAccountDetailsByCIFUrl = _appSettings.ExternalServices?.EnquiryAPIBaseUrl;
            //var Path = _appSettings.ExternalServices?.EnquiryAPIGetTransactionV2;

            var Path = $"{_config.GetValue<string>("ExternalServices:EnquiryAPIBaseUrl")}{_config.GetValue<string>("ExternalServices:EnquiryAPIGetTransactionV2")}";
            Path = Path?.Replace("#accountNo", accountNo);
            Path = Path?.Replace("#startDate", startDate);
            Path = Path?.Replace("#endDate", endDate);
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;



                var client = new RestClient(GetAccountDetailsByCIFUrl);

                var programRequest = new RestRequest(Path);


                var response = await client.ExecuteAsync(programRequest);

                var data = response.Content;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content != null)
                    {

                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionHistoryResponseViewModel>(response.Content);

                        _logger.LogInformation($"MB- GetTransactionHistory: {Path}--Response from Getting MB Transaction from the Enquiry API -- {result.code.ToString()}--{result.description}");
                        return result;
                    }
                }
                _logger.LogError($"MB Error Getting Transaction for MB Customer-api/Transaction/StatementV2/{accountNo}?StartDate={startDate}&EndDate={endDate}--Response from Getting MB Transaction from the Enquiry API ---{data}-- {response.StatusCode.ToString()}--{response.StatusDescription}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"MB-GetTransactionsV2---{ex.Message}");
                _logger.LogError($"MB-GetTransactionsV2---{ex.StackTrace}");

                result.description = $"MB Error Getting Transaction- GetTransactionHistory: {Path}, Error : {ex.Message}";

                return result;
            }
        }

        public async Task<SendMT940DirectResponseViewModel?> SendMT940Direct(SendMT940DirectRequestviewModel request)
        { //the frontend should send in a format dd-mm-yyyy, then format it to yyyy-mm-dd



            /*
             
             {
              "accountNo": "1100004069",
              "startDate": "2023-01-01",
              "endDate": "2023-05-01",
              "recipientEmail": "ogwu.daniel.ifeanyichukwu@gmail.com",
              "accountName": " Daniel Ogwu"
             }
             
             
             */

            var SDList = request.startDate.Split("-");
            var SD = SDList[2] + "-" + SDList[1] + "-" + SDList[0];
            request.startDate = SD;


            var EDList = request.endDate.Split("-");
            var ED = EDList[2] + "-" + EDList[1] + "-" + EDList[0];
            request.endDate = ED;

            var result = new SendMT940DirectResponseViewModel();
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;


                var endpointUrl = $"{_appSettings?.ExternalServices?.StatementNotificationAPIBaseUrl}{_appSettings?.ExternalServices?.StatementNotificationAPISendMT940DirectUrl}";
                var client = new RestClient(endpointUrl);

                _logger.LogInformation($"Send MT940 direct to Email: Request Payload -- {JsonConvert.SerializeObject(request)}");
                var Req = new RestRequest(endpointUrl).AddJsonBody(request);

                var responseData = await client.PostAsync(Req);
                var resContent = responseData.Content;

                _logger.LogInformation($"Send MT940 direct to Email: Response Content -- {resContent}|| Code {responseData.StatusCode}");
                var responsee = JsonConvert.DeserializeObject<SendMT940DirectResponseViewModel>(resContent);

                _logger.LogInformation($"Response from the Statement Notification send MT940 Direct API -- {responsee.responseData.code}--{responsee.responseData.msgDescription}");
                if (responsee.responseData is not null)
                {

                    if (responsee.responseData.code == 0)
                    {
                        result = responsee;
                        return result;
                    }
                }


                result.responseData.code = 99;
                result.responseData = null;
                result.responseData.msgDescription = "No Data";

                return result;

            }
            catch (Exception ex)
            {

                _logger.LogError($"SendMT940Direct -- {ex.Message}");
                _logger.LogError($"SendMT940Direct -- {ex.InnerException}");
                _logger.LogError($"SendMT940Direct-FAILED TO SEND MT940 STATEMENT DIRECT TO EMAIL---{request.recipientEmail}. startdate {request.startDate}, " +
                    $"endDate {request.endDate}-Error: {ex.Message}----{DateTime.Now}");
                result.responseData.code = 99;
                result.responseData = null;
                result.responseData.msgDescription = ex.Message;

                return result;
            }
        }

        public async Task<AccountDetailsResponse> GetListOfAccountByCIF(string customerid)
        {
            var result = new AccountDetailsResponse();
            try
            {

                var saveds = 0;
                //get the list of Account details attached to a Customer id

                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;


                //var GetAccountDetailsByCIFUrl = _appSettings.ExternalServices?.EnquiryAPIBaseUrl;
                //var Path = _appSettings.ExternalServices?.EnquiryAPIGetAccountLists;

                var GetAccountDetailsByCIFUrl = $"{_config.GetValue<string>("AppSettings:ExternalServices:EnquiryAPIBaseUrl")}";
                var Path = $"{_config.GetValue<string>("AppSettings:ExternalServices:EnquiryAPIGetAccountLists")}";


                var client = new RestClient(GetAccountDetailsByCIFUrl);


                var programRequest = new RestRequest($"{Path}/{customerid}");
                programRequest.AddHeader("Content-Type", "application/json");
                programRequest.AddHeader("Accept", "application/json");


                var response = await client.ExecuteAsync(programRequest);
                var desc = $"THE Account Enquiry API Returned {response.StatusDescription} ---For CustomerID {customerid}----------------------------- {DateTime.Now}";
                _logger.LogInformation(desc);

                var data = response.Content;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content != null)
                    {
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountDetailsResponse>(response.Content);

                    }
                }
                var noAccountsGotten = result.data != null ? result.data.Count : 0;
                _logger.LogInformation($"Service Utility-----Processed {noAccountsGotten} Accounts---For CustomerID {customerid}----------------------------- {DateTime.Now}");

                return result;


            }
            catch (Exception error)
            {
                _logger.LogError($"Error Gettting Account numbers from Account Enquiry API {error.Message}");
                _logger.LogError($"Error Gettting Account numbers from Account Enquiry API -- Stack Trace {error.StackTrace}");

                return result;
            }


        }

        public async Task<AccountDetailResponseViewModel> GetDetailsofAnAccount(string accountNo)
        {
            var result = new AccountDetailResponseViewModel();
            try
            {

                var saveds = 0;
                //get the list of Account details attached to a Customer id

                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;


                //var GetAccountDetailsByCIFUrl = _appSettings.ExternalServices?.EnquiryAPIBaseUrl;
                //var Path = _appSettings.ExternalServices?.EnquiryAPIGetAccountDetails;
                var GetAccountDetailsByCIFUrl = $"{_config.GetValue<string>("AppSettings:ExternalServices:EnquiryAPIBaseUrl")}";
                var Path = $"{_config.GetValue<string>("AppSettings:ExternalServices:EnquiryAPIGetAccountDetails")}";

                var client = new RestClient(GetAccountDetailsByCIFUrl);



                var programRequest = new RestRequest($"{Path}/{accountNo}");
                programRequest.AddHeader("Content-Type", "application/json");
                programRequest.AddHeader("Accept", "application/json");


                var response = await client.ExecuteAsync(programRequest);
                var desc = $"THE Account Enquiry API Returned {response.StatusDescription} ---For AccountNo {accountNo}----------------------------- {DateTime.Now}";
                _logger.LogInformation(desc);

                var data = response.Content;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content != null)
                    {
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountDetailResponseViewModel>(response.Content);

                        _logger.LogInformation($"Service Utility-----Processed for account no: {accountNo} details:  {response.Content}----------------------------- {DateTime.Now}");

                    }
                }

                return result;


            }
            catch (Exception error)
            {
                _logger.LogError($"MB-Error Getting the details of Account numbers from Account Enquiry API {error.Message}");
                _logger.LogError($"MB-Error Getting the details of Account numbers from Account Enquiry API -- Stack Trace {error.StackTrace}");

                return result;
            }


        }



    }
}
