using MTMiddleware.Shared.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.Entities
{
    //[Table("CustomersChannelTransKey")]
    public class CustomersChannelTransKey : AuditEntity
    {

        public CustomersChannelTransKey()
        {

        }

        public string Channel { get; set; } = string.Empty;
        public string TransKey { get; set; } = string.Empty;
        public string CustomerDetailsId  { get; set; }
        public bool IsActive  { get; set; }
        public virtual CustomerDetails CustomerDetails { get; set; }
        public virtual ICollection<CustomerTransactions> CustomerTransactions { get; set; }
    }
}
