namespace MTMiddleware.Shared.Models
{
    public class AppSettings
    {
        public string ClientURL { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public int? DisablePasswordSignIn { get; set; }
        public string CryptoKey { get; set; } = string.Empty;
        public int RolloverMaximumRetryCount { get; set; }
        public int RolloverProcessingBatchSize { get; set; }
        public ServiceEndpoints? ServiceEndpoints { get; set; }
        public DuoSettings? DuoSettings { get; set; }

        public JwtSettings? JwtSettings { get; set; }
        public SalesforceSettings? SalesforceSettings { get; set; }
        public ADAuthSettings? ADAuthSettings { get; set; }
        public ExternalServices? ExternalServices { get; set; }
    }

    public class DuoSettings
    {
        public string ApplicationKey { get; set; } = string.Empty;
        public string IntegrationKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
    }
    public class ServiceEndpoints
    {
        public string AccountEnquiryUrl { get; set; } = string.Empty;
        public string EmailNotificationUrl { get; set; } = string.Empty;
        public string ADSignInUrl { get; set; } = string.Empty;
        public string ADUserProfileUrl { get; set; } = string.Empty;
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int accessTokenExpiryInMinutes { get; set; }
        public int refreshTokenExpiryInDays { get; set; }
    }

    public class SalesforceSettings
    {
        public string BaseUrl { get; set; } = String.Empty;

        public string AddRetailCustomerUrl { get; set; } = String.Empty;
        public string UpdateRetailCustomerUrl { get; set; } = String.Empty;

        public string AddCorporateCustomerUrl { get; set; } = String.Empty;
        public string UpdateCorporateCustomerUrl { get; set; } = String.Empty;
        public string UpdateCorporateCustomerDirectorUrl { get; set; } = String.Empty;
    }

    public class ExternalServices
    {
        public string EnquiryAPIBaseUrl { get; set; } = String.Empty;
        public string EnquiryAPIGetTransactionV2 { get; set; } = String.Empty;
        public string StatementNotificationAPIBaseUrl { get; set; } = String.Empty;
        public string StatementNotificationAPISendMT940DirectUrl { get; set; } = String.Empty;
        public string EnquiryAPIGetAccountLists { get; set; } = String.Empty;
        public string EnquiryAPIGetAccountDetails { get; set; } = String.Empty;

      

    }




    public class ADAuthSettings
    {
        public string SignInUrl { get; set; } = string.Empty;
        public string UserProfileUrl { get; set; } = string.Empty;
    }
}