using MTMiddleware.Shared.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class ActivitiesLogResponseViewModel
    {
            public string activity { get; set; }
            public string email { get; set; }
            public string phoneNumber { get; set; }
            public string Role { get; set; }
            public DateTime CreatedAt { get; set; }

    }
}
