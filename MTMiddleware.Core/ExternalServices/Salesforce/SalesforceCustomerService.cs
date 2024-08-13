using MTMiddleware.Core.ExternalServices.Salesforce.Models;
using MTMiddleware.Shared.Models;
using MTMiddleware.Shared.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary.Models;

namespace MTMiddleware.Core.ExternalServices.Salesforce
{
    public class SalesforceCustomerService : ISalesforceCustomerService
    {
        private AppSettings _appSettings;
        private IApiCaller _apiCaller;

        public SalesforceCustomerService(IOptions<AppSettings> appSettings, IApiCaller apiCaller)
        {
            _appSettings = appSettings.Value;
            _apiCaller = apiCaller;
        }

        public async Task<SalesforceResponseModel?> CorporateCustomerAddAsync(SalesforceCorporateCustomerAddViewModel model)
        {
            string url = string.Empty;
            var headers = new Dictionary<string, string>();

            if (_appSettings.SalesforceSettings != null)
            {
                url = $"{_appSettings.SalesforceSettings.BaseUrl}{_appSettings.SalesforceSettings.AddCorporateCustomerUrl}";
            }
            else
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult result = await _apiCaller.PostAsync(url, model, headers);
            if (result.IsSuccessfull)
            {
                var dResponse = JsonConvert.DeserializeObject<SalesforceResponseModel>(result.Data);
                return dResponse;
            }
            else
            {
                var dResponse = new SalesforceResponseModel()
                {
                    Code = "70",
                    Description = "Salesforce Customer Onboarding Failure"
                };

                return dResponse;
            }
        }

        public async Task<SalesforceResponseModel> CorporateCustomerDirectorUpdateAsync(SalesforceCorporateCustomerDirectorUpdateViewModel model)
        {
            string url = string.Empty;
            var headers = new Dictionary<string, string>();

            if (_appSettings.SalesforceSettings != null)
            {
                url = $"{_appSettings.SalesforceSettings.BaseUrl}{_appSettings.SalesforceSettings.UpdateCorporateCustomerDirectorUrl}";
            }
            else
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult result = await _apiCaller.PutAsync(url, model, headers);
            if (result.IsSuccessfull)
            {
                var dResponse = JsonConvert.DeserializeObject<SalesforceResponseModel>(result.Data);
                return dResponse;
            }
            else
            {
                var dResponse = new SalesforceResponseModel()
                {
                    Code = "70",
                    Description = "Salesforce Customer Onboarding Failure"
                };

                return dResponse;
            }
        }

        public async Task<SalesforceResponseModel?> CorporateCustomerUpdateAsync(SalesforceCorporateCustomerUpdateViewModel model)
        {
            string url = string.Empty;
            var headers = new Dictionary<string, string>();

            if (_appSettings.SalesforceSettings != null)
            {
                url = $"{_appSettings.SalesforceSettings.BaseUrl}{_appSettings.SalesforceSettings.UpdateCorporateCustomerUrl}";
            }
            else
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult result = await _apiCaller.PutAsync(url, model, headers);
            if (result.IsSuccessfull)
            {
                var dResponse = JsonConvert.DeserializeObject<SalesforceResponseModel>(result.Data);
                return dResponse;
            }
            else
            {
                var dResponse = new SalesforceResponseModel()
                {
                    Code = "70",
                    Description = "Salesforce Customer Onboarding Failure"
                };

                return dResponse;
            }
        }

        public async Task<SalesforceResponseModel?> RetailCustomerAddAsync(SalesforceRetailCustomerAddViewModel model)
        {
            string url = string.Empty;
            var headers = new Dictionary<string, string>();

            if (_appSettings.SalesforceSettings != null)
            {
                url = $"{_appSettings.SalesforceSettings.BaseUrl}{_appSettings.SalesforceSettings.AddRetailCustomerUrl}";
            }
            else
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult result = await _apiCaller.PostAsync(url, model, headers);
            if (result.IsSuccessfull)
            {
                var dResponse = JsonConvert.DeserializeObject<SalesforceResponseModel>(result.Data);
                return dResponse;
            }else
            {
                var dResponse = new SalesforceResponseModel()
                {
                    Code = "70",
                    Description = "Salesforce Customer Onboarding Failure"
                };
                
                return dResponse;
            }
        }

        public async Task<SalesforceResponseModel?> RetailCustomerUpdateAsync(SalesforceRetailCustomerUpdateViewModel model)
        {
            string url = string.Empty;
            var headers = new Dictionary<string, string>();

            if (_appSettings.SalesforceSettings != null)
            {
                url = $"{_appSettings.SalesforceSettings.BaseUrl}{_appSettings.SalesforceSettings.UpdateRetailCustomerUrl}";
            }
            else
            {
                throw new Exception("Please, provide a valid url");
            }

            ApiResult result = await _apiCaller.PutAsync(url, model, headers);
            if (result.IsSuccessfull)
            {
                var dResponse = JsonConvert.DeserializeObject<SalesforceResponseModel>(result.Data);
                return dResponse;
            }
            else
            {
                var dResponse = new SalesforceResponseModel()
                {
                    Code = "70",
                    Description = "Salesforce Customer Onboarding Failure"
                };

                return dResponse;
            }
        }
    }
}
