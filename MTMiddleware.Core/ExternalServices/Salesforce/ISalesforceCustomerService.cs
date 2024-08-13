using MTMiddleware.Core.ExternalServices.Salesforce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce
{
    public interface ISalesforceCustomerService
    {
        Task<SalesforceResponseModel?> RetailCustomerAddAsync(SalesforceRetailCustomerAddViewModel model);
        Task<SalesforceResponseModel?> RetailCustomerUpdateAsync(SalesforceRetailCustomerUpdateViewModel model);

        Task<SalesforceResponseModel?> CorporateCustomerAddAsync(SalesforceCorporateCustomerAddViewModel model);
        Task<SalesforceResponseModel?> CorporateCustomerUpdateAsync(SalesforceCorporateCustomerUpdateViewModel model);

        Task<SalesforceResponseModel> CorporateCustomerDirectorUpdateAsync(SalesforceCorporateCustomerDirectorUpdateViewModel model);
    }
}
