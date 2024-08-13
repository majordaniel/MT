using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Models
{


    public class MsgData
    {
        public string accountNo { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string recipientEmail { get; set; }
        public string accountName { get; set; }
    }

    public class ResponseData
    {
        public int code { get; set; }
        public MsgData msgData { get; set; }
        public string msgDescription { get; set; }
    }

    public class SendMT940DirectResponseViewModel
    {
        public ResponseData responseData { get; set; }
    }

}
