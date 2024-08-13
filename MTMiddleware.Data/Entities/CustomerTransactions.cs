using MTMiddleware.Shared.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.Entities
{
    public class CustomerTransactions : BaseApprovalEntity
    {
        public CustomerTransactions()
        {
                
        }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string SourceBank { get; set; }
        public string DestinationBank { get; set; }
        public string DestinationAccount { get; set; }
        public string SourceAccount { get; set; }
        public string Comment { get; set; }

        public string Reference { get; set; }
        public string Channel { get; set; } //CIB, ERP
        public string TransactionType { get; set; } //SWIFT, RTGS
        public string Currency { get; set; } //USD, NGN
        public string TransactionStatus { get; set; } //Pending, Approved
        public string CustomerDetailId { get; set; }
        public string TransKey { get; set; }
        //public string CustomersChannelTokenId { get; set; }
        //[ForeignKey("CustomersChannelTokenId")]
        //public virtual CustomersChannelToken CustomersTransactionTags { get; set; }
        [ForeignKey("CustomerDetailId")]
        public virtual CustomerDetails CustomerDetails { get; set; }


    }
}
