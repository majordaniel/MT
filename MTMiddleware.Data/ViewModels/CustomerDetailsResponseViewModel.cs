using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels
{
    public class CustomerDetailsResponseViewModel
    {

        public string Id { get; set; } 
        public string UserId { get; set; } 
        public string CifId { get; set; } 
        public string Email { get; set; } 
        public string CustomerName { get; set; } 
        public string PhoneNumber { get; set; }
        public decimal SwiftDailyTransactionLimit { get; set; }
        public decimal RTGSDailyTransactionLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public string? ApprovalStatus { get; set; } 
        public string? ApprovalActionBy { get; set; } 
        public DateTime? ApprovalActionDate { get; set; }
        public string? ApprovalActionReason { get; set; } 
        public List<CustomerChannelTransKeyResponseViewModel> channelTokens { get; set; } 
        public List<AccountNosViewModel> AccountNosViewModel { get; set; } 
    }
    public class AccountNosViewModel
    {
        public string AccountNo { get; set; }
        public string Currency { get; set; }
    }
}
