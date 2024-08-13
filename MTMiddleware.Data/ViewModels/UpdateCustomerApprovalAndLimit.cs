using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class UpdateCustomerApprovalAndLimit
    {
        public string CustomerId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public decimal SwiftDailyTransactionLimit { get; set; }
        public decimal RTGSDailyTransactionLimit { get; set; }

    }
}
