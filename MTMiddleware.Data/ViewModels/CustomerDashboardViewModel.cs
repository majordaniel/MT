using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public decimal SwiftTotal { get; set; }
        public decimal SwiftPending { get; set; }
        public decimal SwiftSuccessful { get; set; }
        public decimal SwiftDailyLimit { get; set; }

        public decimal RTGSTotal { get; set; }
        public decimal RTGSPending { get; set; }
        public decimal RTGSSuccessful { get; set; }
        public decimal RTGSDailyLimit { get; set; }
    }
}
