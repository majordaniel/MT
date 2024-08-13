using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class CreateTransactionRequest
    {



        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "SourceBank is required")]
        public string SourceBank { get; set; }


        [Required(ErrorMessage = "DestinationBank is required")]
        public string DestinationBank { get; set; }


        [Required(ErrorMessage = "DestinationAccount is required")]
        public string DestinationAccount { get; set; }


        [Required(ErrorMessage = "SourceAccount is required")]
        public string SourceAccount { get; set; }


        [Required(ErrorMessage = "Currency is required")]
        public string Currency { get; set; }

        //public DateTime TransactionDate { get; set; }
        //public string Comment { get; set; }
        //public DateTime ValueDate { get; set; }
        //public string TransactionStatus { get; set; }
        //public string CustomersChannelTokenId { get; set; }


    }


    public class CreateTransactionResponse
    {

        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
       
        public string DestinationAccount { get; set; }

        public string SourceAccount { get; set; }
        public string Channel { get; set; }

        public string TransactionType { get; set; } //SWIFT, RTGS
        public string Currency { get; set; } //USD, NGN
        public string TransactionStatus { get; set; } //Pending, Approved
        //public string CustomerDetailId { get; set; }
        //public string TransKey { get; set; }










    }
}
