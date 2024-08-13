using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibrary;

public class ApplicationConstants
{
    public const string AppSettingsKey = "AppSettings";

    public const string AllowedCORSOrigins = "AppSettings:AllowedCORSOrigins";

    public const string SUCCESS_CODE = "00";

    public const string STANDARD_DATE_TIME_FORMAT = "dd/MM/yyyy hh:mm tt";

    public const string SERVER_DATE_TIME_FORMAT = "yyyy-MM-ddTHH:mm:sszzz";

    public const string APPLICATION_CONTENT_TYPE_JSON = "application/json";

    public const string SYSTEM_USER = "SYSTEM";
}

public static class TransactionChannels
{
    public static readonly string ERP = "ERP";
    public static readonly string CIB = "CIB";
}


public static class AuditLogActivites
{
    public static readonly string AddNewUser = "Added New user";
    public static readonly string EnabledAUser = "Enabled a user";
    public static readonly string DisableAUser = "Disabled a user";

    public static readonly string EnabledCustomer = "Enabled a customer";
    public static readonly string Disabledcustomer = "Disabled a customer";

    public static readonly string LoggedIn = "Logged in to the application";
    public static readonly string LoggedOut = "Logged out of the application";
}