using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce.Models
{
    public class SalesforceCorporateCustomerUpdateViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CertificateOfIncorporation { get; set; } = string.Empty;
        public string CountryOfIncorporation { get; set; } = string.Empty;
        public string DateofIncorporation { get; set; } = string.Empty;
        public string CategoryOfBusiness { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string BankVerificationNumber { get; set; } = string.Empty;
        public int BankEntity { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string NatureOfBusiness { get; set; } = string.Empty;
        public string CorporateBusinessAddress { get; set; } = string.Empty;

        public string TaxIdentificationNumber { get; set; } = string.Empty;
        public string ScumlBusiness { get; set; } = string.Empty;
        public string FatcaClassification { get; set; } = string.Empty;

        public Director? Director { get; set; }
        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
    }
}
