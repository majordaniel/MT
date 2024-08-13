using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.Entities
{
    public  class ApplicationRole : IdentityRole<Guid>
    {
        public bool IsSystemRole { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdated { get; set; }

        //public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
