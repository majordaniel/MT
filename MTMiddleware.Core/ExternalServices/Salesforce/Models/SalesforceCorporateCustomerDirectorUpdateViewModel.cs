using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce.Models
{
    public class SalesforceCorporateCustomerDirectorUpdateViewModel
    {
        public string CustomerId { get; set; } = string.Empty;
        public Director? Director { get; set; }
    }
}
