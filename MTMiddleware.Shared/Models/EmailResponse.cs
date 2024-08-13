using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.Models
{
    public class EmailResponse
    {
        public string code { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public object data { get; set; } = string.Empty;
    }
}
