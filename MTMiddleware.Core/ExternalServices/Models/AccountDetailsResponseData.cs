using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Models
{
    public class AccountDetailsResponseData
    {
        public string schemeType { get; set; }
        public string schemeCode { get; set; }
        public string accountName { get; set; }
        public string status { get; set; }
        public bool isFrozen { get; set; }
        public bool isDormant { get; set; }
        public string soL_ID { get; set; }
        public string traceId { get; set; }
        public string bvn { get; set; }
        public string kycLevel { get; set; }
        public string customerId { get; set; }
        public string accountOpeningDate { get; set; }
        public DateTime? lastTransactionDate { get; set; }
        public string accountType { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string accountNo { get; set; }
        public string currency { get; set; }

        public string closeStatus { get; set; }
        public bool IsClosed { get; set; }

        //public string Status { get; set; }
        //public bool IsFrozen { get; set; }
        //public bool IsDormant { get; set; }
    }

    public class AccountDetailsResponse
    {
        public List<AccountDetailsResponseData> data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }


    public class AccountDetailResponseViewModel
    {
        public AccountDetailsResponseData data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }



}
