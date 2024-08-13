using UtilityLibrary.Extensions;

namespace UtilityLibrary.Enumerations
{
    public enum ResponseEnum
    {
        [EnumDisplay(Name = "Operation Completed Successfully", Description = "Operation was completed successfully")]
        OperationCompletedSuccesfully = 00,

        [EnumDisplay(Name = "Status Unknown", Description = "Status of transacion or operation is unknown")]
        StatusUnknown = 01,

        [EnumDisplay(Name = "Invalid Phone Number", Description = "Phone number is invalid")]
        InvalidPhoneNumber = 02,

        [EnumDisplay(Name = "Account Name Mismatch", Description = "Name provided does not match the account name on the core banking or database")]
        AccountNameMismatch = 03,

        [EnumDisplay(Name = "Request Processing In Progress", Description = "The request is being processed")]
        RequestProcessingInProgress = 04,

        [EnumDisplay(Name = "Time Out ", Description = "Time Out ")]
        TimeOut = 05,

        [EnumDisplay(Name = "Invalid Transaction", Description = "Invalid Transaction")]
        InvalidTransaction = 06,

        [EnumDisplay(Name = "Invalid Amount", Description = "Transaction amount is invalid")]
        InvalidAmount = 07,

        [EnumDisplay(Name = "Unknown Bank Code", Description = "Bank code does not exist")]
        UnknownBankCode = 08,

        [EnumDisplay(Name = "Invalid Channel", Description = "Invalid Channel code")]
        InvalidChannel = 09,

        [EnumDisplay(Name = "Wrong Method Call", Description = "Wrong Method Call")]
        WrongMethodCall = 10,

        [EnumDisplay(Name = "No Action Taken", Description = "No Action Taken")]
        NoActionTaken = 11,

        [EnumDisplay(Name = "Format Error", Description = "Format Error")]
        FormatError = 12,

        [EnumDisplay(Name = "Invalid Account Name", Description = "Invalid Account Name")]
        InvalidAccountName = 13,

        [EnumDisplay(Name = "No Sufficent Funds", Description = "No Sufficent Funds")]
        NoSufficentFunds = 14,

        [EnumDisplay(Name = "Transaction Not Permitted On Channel", Description = "Transaction Not Permitted To Sender")]
        TransactionNotPermittedOnChannel = 15,

        [EnumDisplay(Name = "Security Violation ", Description = "Security violation ")]
        SecurityViolation = 16,

        [EnumDisplay(Name = "Response received too late", Description = "Response received too late")]
        ResponseReceivedTooLate = 17,

        [EnumDisplay(Name = "Unauthorized", Description = "Unauthorized Transaction")]
        Unauthorized = 18,

        [EnumDisplay(Name = "Duplicate transaction ", Description = "Duplicate transaction ")]
        DuplicateTransaction = 19,

        [EnumDisplay(Name = "System Malfunction ", Description = "System malfunction ")]
        SystemMalfunction = 20,

        [EnumDisplay(Name = "Timeout waiting for response from destination", Description = "Timeout waiting for response from destination")]
        TimeoutWaitingForResponseFromDestination = 21,

        [EnumDisplay(Name = "Transaction Failed", Description = "Transaction Failed")]
        TransactionFailed = 22,

        [EnumDisplay(Name = "Record Exist", Description = "Record Exist")]
        RecordExist = 23,

        [EnumDisplay(Name = "Record Does not Exist", Description = "Record Does not Exist")]
        RecordDoesNotExist = 24,

        [EnumDisplay(Name = "Decryption Error", Description = "Decryption Security Error Occurred")]
        DecryptionFailure = 25,

        [EnumDisplay(Name = "Declined Or Failed", Description = "Declined or Failed Operation")]
        DeclinedOrFailed = 26,

        [EnumDisplay(Name = "SalesForce Connection Failure", Description = "Could Not Establish Connection To SalesForce")]
        SalesForceConnectionFailure = 27,

        [EnumDisplay(Name = "Required data Not Provided", Description = "Required data not provided")]
        RequiredDataNotProvided = 28,

        [EnumDisplay(Name = "Invalid Data", Description = "Invalid data")]
        InvalidData = 29,

        [EnumDisplay(Name = "Error Occured", Description = "Error occured")]
        ErrorOccured = 30,

        [EnumDisplay(Name = "Missing Configuration Data", Description = "Missing configuration data")]
        MissingConfigurationData = 31,

        [EnumDisplay(Name = "Invalid Security Token", Description = "Invalid security token")]
        InvalidSecurityToken = 32,

        [EnumDisplay(Name = "Account Already Exists", Description = "Account already exists")]
        AccountAlreadyExists = 33,


        [EnumDisplay(Name = "No AD Records Found Email", Description = "No AD Records Found for the Email")]
        NoADRecordsFound = 34,

        [EnumDisplay(Name = "Parameter Input Not Provided", Description = "Required input data not provided")]
        ParameterInputNotProvided = 35,

        [EnumDisplay(Name = "Record Not Deleted", Description = "Record could not be deleted")]
        RecordNotDeleted = 40,




        [EnumDisplay(Name = "Account Number not exist", Description = "Account Number not exist")]
        AccountNoDoesNotExist = 42,

        [EnumDisplay(Name = "Customer Record Does not Exist", Description = "Customer does not Exist")]
        CustomerRecordDoesNotExist = 43,


        [EnumDisplay(Name = "Customer Record Exist", Description = "Customer already exists")]
        CustomerRecordExist = 68,



        [EnumDisplay(Name = "Invalid Access Token", Description = "Invalid access token")]
        InvalidAccessToken = 79,

        [EnumDisplay(Name = "You must provide Username", Description = "You must provide username")]
        ProvideUserName = 80,

        [EnumDisplay(Name = "You must provide security Token", Description = "You must provide a security token")]
        ProvideSecurityToken = 81,

        [EnumDisplay(Name = "You must provide valid email", Description = "You must provide a valid email")]
        ProvideValidEmail = 82,


        [EnumDisplay(Name = "Invalid username and/or password", Description = "Invalid username and/or password")]
        InvalidUsernameOrPassword = 85,

        [EnumDisplay(Name = "Verify Account", Description = "Kindly verify your account by clicking the link sent to your email before attempting to sign in")]
        VerifyAccount = 86,

        [EnumDisplay(Name = "Account Not Found", Description = "Account not found")]
        AccountNotFound = 87,

        [EnumDisplay(Name = "Provide Password", Description = "Provide password")]
        ProvidePassword = 88,

        [EnumDisplay(Name = "Password Mismatch", Description = "Password and confirmation password do not match")]
        PasswordMismatch = 89,

        [EnumDisplay(Name = "User Not Registered", Description = "User could not be registered")]
        UserNotRegistered = 90,

        [EnumDisplay(Name = "Failed To Reset Password", Description = "Failed to reset password")]
        PasswordResetFailed = 91,

        [EnumDisplay(Name = "Account Could Not Be Updated", Description = "Account could not be updated")]
        AccountCouldNotBeUpdated = 92,

        [EnumDisplay(Name = "Account Could Not Be Enabled", Description = "Account could not be enabled")]
        AccountCouldNotBeEnabled = 93,

        [EnumDisplay(Name = "Account Could Not Be Disabled", Description = "Account could not be disabled")]
        AccountCouldNotBeDisabled = 94,

        [EnumDisplay(Name = "Role Assigned To User Not Found", Description = "Role assigned to user not found")]
        RoleAssignedToUserNotFound = 95,

        [EnumDisplay(Name = "User Profile Url Not Configured", Description = "AD User profile url not configured")]
        UserProfileUrlNotConfigured = 96,

        [EnumDisplay(Name = "SignIn Url Not Configured", Description = "AD signin url not configured")]
        SignInUrlNotConfigured = 97,

        [EnumDisplay(Name = "Unable To Retrieve User Profile", Description = "Unable to retrieve user profile")]
        UnableToRetrieveUserProfile = 98,

        [EnumDisplay(Name = "User Invitation Failed", Description = "User invitation failed")]
        UserInvitationFailed = 99,

        [EnumDisplay(Name = "Unable To Sign In", Description = "Unable to sign in. See the system administrator")]
        UnableToSignIn = 100,


        [EnumDisplay(Name = "Unable to Send Email", Description = "Unable to send email")]
        UnableToSendEmail = 107,

        [EnumDisplay(Name = "Failed to Send Email", Description = "Failed to send email")]
        FailedToSendEmail = 108,

        [EnumDisplay(Name = "The Channel Key You provided is not for the User", Description = "The Channel Key You provided is not for the User")]
        ChannelKeyNotForTheUser = 810,

        [EnumDisplay(Name = "The Channel Key does Not Existing", Description = "The Channel Key does Not Existing")]
        ChannelKeyNotExisting = 811,


        [EnumDisplay(Name = "Role Not found", Description = "Role Not Found")]
        RoleNotfound = 812,

    }

    public enum TransactionType
    {


        [EnumDisplay(Name = "SWIFT", Description = "SWIFT Transaction")]
        SWIFT = 1,

        [EnumDisplay(Name = "RTGS", Description = "RTGS Transaction")]
        RTGS = 2,
    }
    public enum CurrencyEnum
    {


        [EnumDisplay(Name = "NGN", Description = "NGN Currency")]
        NGN = 1,

        [EnumDisplay(Name = "USD", Description = "USD Currency")]
        USD = 2,
    }

    public enum TransactionChannelsEnum
    {
        [EnumDisplay(Name = "ERP", Description = "ERP Channels")]
        ERP = 1,

        [EnumDisplay(Name = "CIB", Description = "CIB Channels")]
        CIB = 2,
    }

    public enum CustomerApprovalStatusEnum
    {
        [EnumDisplay(Name = "Approved", Description = "Approved")]
        Approved = 1,

        [EnumDisplay(Name = "Pending", Description = "Pending")]
        Pending = 2,
        [EnumDisplay(Name = "Declined", Description = "Declined")]
        Declined = 3,

        [EnumDisplay(Name = "Disabled", Description = "Disabled")]
        Disabled = 4,
    }

    public enum TransactionStatusEnum
    {
        [EnumDisplay(Name = "Pending", Description = "Pending")]
        Pending = 1,

        [EnumDisplay(Name = "Successful", Description = "Successful")]
        Successful = 2,
    }

    public enum RoleEnum
    {
        [EnumDisplay(Name = "SuperAdmin", Description = "SuperAdmin")]
        SuperAdmin = 1,

        [EnumDisplay(Name = "Approver", Description = "Approver")]
        Approver = 2,

        [EnumDisplay(Name = "Customer", Description = "Customer")]
        Customer = 3,
    }
}