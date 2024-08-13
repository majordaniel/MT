using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Salesforce.Models
{
    public class NextOfKin
    {
        public string RelationshipType { get; set; } = string.Empty;
        public string NextOfKinTitle { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string Lga { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class EmploymentInfo
    {
        public string AnnualSalaryRange { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EmploymentName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string OfficePhoneNumber { get; set; } = string.Empty;
    }

    public class IdentificationRequest
    {
        public string IdIssueDate { get; set; } = string.Empty;
        public string IdExpiryDate { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string IdType { get; set; } = string.Empty;
        public string PlaceOfIssue { get; set; } = string.Empty;
    }

    public class Director
    {
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string FinanciallyExposedPersons { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string BankVerificationNumber { get; set; } = string.Empty;
        
        public string PolitcallyExposedPersons { get; set; } = string.Empty;

        public Address? Address { get; set; }
        public IdentificationRequest? identification { get; set; }
        public Contact? Contact { get; set; }
    }

    public class Address
    {
        public string City { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string houseNumber { get; set; } = string.Empty;
        public string streetName { get; set; } = string.Empty;
    }

    public class Contact
    {
        public string EmailAddress { get; set; } = string.Empty;
        public string ContactNumber1 { get; set; } = string.Empty;
        public string ContactNumber2 { get; set; } = string.Empty;
    }
}