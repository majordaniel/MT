using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        //Transactions
        Task<Response<PagedList<CustomerTransactionResponseViewModel>>>
            //Task<Response<List<CustomerTransactionResponseViewModel>>> 
            GetAllCustomerSwiftTransaction(DateRangeQueryModel queryModel, string CustomerId);
        Task<Response<PagedList<CustomerTransactionResponseViewModel>>> 
            GetAllCustomerRTGSTransaction(DateRangeQueryModel queryModel, string CustomerId);
        Task<Response<CreateTransactionResponse>> CreateTransaction(CreateTransactionRequest model);

    }
}
