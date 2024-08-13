using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTMiddleware.Data.Entities
{
    [Table("Users")]
    public  class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string DisplayName { get; set; } = String.Empty;
        public bool IsRootAdmin { get; set; }
        //[Key, ForeignKey("ApplicationRole")]
        //public Guid RoleId { get; set; }
        public string RoleId { get; set; } = String.Empty;
        public DateTime? LastSignInDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailSent { get; set; }
        public string? EmailVerificationToken { get; set; } = null;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdated { get; set; }

        //public virtual ICollection<CustomerDetails> CustomerDetails { get; set; }
        public virtual ICollection<CustomerDetails> CustomerDetails { get; set; }

        //public virtual ApplicationRole ApplicationRole { get; set; }

        //public virtual ICollection<CustomersTransactionTags> CustomersTransactionTags { get; set; }



    }
}
