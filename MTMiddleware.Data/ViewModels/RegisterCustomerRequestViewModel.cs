using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class RegisterCustomerRequestViewModel
    {
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "CustomerName is required")]
        //public string CustomerName { get; set; }

        //[Required(ErrorMessage = "PhoneNumber is required")]
        //public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "AccountNo is required")]
        public string AccountNo { get; set; }
     [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Channel Ids is required")]
        public List<string> ChannelIds { get; set; }
    }
}
