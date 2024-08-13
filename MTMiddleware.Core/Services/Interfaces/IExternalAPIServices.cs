using MTMiddleware.Core.ExternalServices.Models;
using MTMiddleware.Core.ExternalServices.Salesforce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices
{
    public interface IExternalAPIServices
    {
        Task<TransactionHistoryResponseViewModel?> GetCustomerTransactions(string accountNo,string startDate, string endDate);
        Task<SendMT940DirectResponseViewModel?> SendMT940Direct(SendMT940DirectRequestviewModel request);
        Task<AccountDetailsResponse> GetListOfAccountByCIF(string customerid);
        Task<AccountDetailResponseViewModel> GetDetailsofAnAccount(string accountNo);

    }
}
