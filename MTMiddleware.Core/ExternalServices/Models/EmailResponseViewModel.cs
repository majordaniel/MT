using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ViewModels
{
    public class EmailResponseViewModel
    {
        public string code { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public object data { get; set; } = string.Empty;
    }
}
