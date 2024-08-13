using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ViewModels
{
    public class EmailRequestViewModel
    {
        public string To { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
    }

}
