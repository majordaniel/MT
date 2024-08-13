using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce.Models
{
    public class SalesforceResponseModel
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SalesforceDataModel? Data { get; set; }
    }

    public class SalesforceDataModel
    {
        public string CustomerId { get; set; } = string.Empty;
    }
}
