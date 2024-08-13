using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class AdminDashboardViewModel
    {
        public long TotalCustomers { get; set; }
        public long TotalPendingCustomers { get; set; }
        public long TotalRTGSTransactions { get; set; }
        public long TotatSWIFTTransactions { get; set; }
    }
}
