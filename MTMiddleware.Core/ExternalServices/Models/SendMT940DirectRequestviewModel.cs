
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Models
{
 

    public class SendMT940DirectRequestviewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$|^[0-9]+$", ErrorMessage = "Enter Account Numbers with valid characters")]
        public string accountNo { get; set; }
        [Required]

        public string startDate { get; set; }
        [Required]
        public string endDate { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]

        public string recipientEmail { get; set; }
        //[RegularExpression(@"^[a-zA-Z]+$|^[0-9]+$", ErrorMessage = "Enter Account Name with valid characters")]
        [Required]
        public string accountName { get; set; }




    }
}
