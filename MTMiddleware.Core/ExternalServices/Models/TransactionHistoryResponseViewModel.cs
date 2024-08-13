using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
 
    public class TransactionHistoryResponseData
    {
        //public string Name { get; set; }
        public string TRA_DATE { get; set; }
        public string VAL_DATE { get; set; }
        public string Narration { get; set; }
        //public string TRA_AMT { get; set; }
        //public string TRA_TYPE { get; set; }
        public string TRA_BALANCE { get; set; }

        public string TRA_ID { get; set; }
        public string TRA_CURRENCY { get; set; }
        public decimal DR_AMT { get; set; }
        public decimal CR_AMT { get; set; }
    }

    public class TransactionHistoryResponseViewModel
    {
        public List<TransactionHistoryResponseData> data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }
}
