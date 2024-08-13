using MTMiddleware.Shared.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.Entities
{
    //[Table("CustomerDetails")]
    public class CustomerDetails: BaseApprovalEntity
    {
        public CustomerDetails() { }

        //public string UserId { get; set; } = String.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        [StringLength(80)]
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal SwiftDailyTransactionLimit { get; set; } 
        public decimal RTGSDailyTransactionLimit { get; set; } 
        public string CifId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<CustomersChannelTransKey> CustomersTransactionTags { get; set; }




    }
}
