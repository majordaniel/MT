using MTMiddleware.Shared.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.Entities
{
    public class CustomerAccounts :BaseApprovalEntity
    {
        public string accountName { get; set; }
        public string CifId { get; set; }
        public string accountType { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string accountNo { get; set; }
        public string currency { get; set; }
    }
}
