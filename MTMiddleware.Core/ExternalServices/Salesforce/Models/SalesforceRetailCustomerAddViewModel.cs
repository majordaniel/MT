using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce.Models
{
    public class SalesforceRetailCustomerAddViewModel
    {
        public string BVN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber2 { get; set; } = string.Empty;
        public string LevelOfAccount { get; set; } = string.Empty;
        public string LgaOfOrigin { get; set; } = string.Empty;
        public string LgaOfResidence { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string NIN { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string StateOfOrigin { get; set; } = string.Empty;
        public string StateOfResidence { get; set; } = string.Empty;
        public string ReferredBy { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int BankEntity { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty;
        public string CountryOfResidence { get; set; } = string.Empty;
        public string PlaceOfBirth { get; set; } = string.Empty;
        public string MotherMaidenName { get; set; } = string.Empty;
    }
}