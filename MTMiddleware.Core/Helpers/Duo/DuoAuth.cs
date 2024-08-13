using System.Security.Cryptography;
using System.Text;

namespace MTMiddleware.Core.Helpers.Duo
{
    public static class DuoAuth
    {
        const string DUO_PREFIX = "TX";
        const string APP_PREFIX = "APP";
        const string AUTH_PREFIX = "AUTH";

        const int DUO_EXPIRE = 300;
        const int APP_EXPIRE = 3600;

        const int IKEY_LEN = 20;
        const int SKEY_LEN = 40;
        const int AKEY_LEN = 40;

        public static string ERR_USER = "ERR|The username passed to 'sign request' is invalid.";
        public static string ERR_IKEY = "ERR|The Duo integration key passed to 'sign request' is invalid.";
        public static string ERR_SKEY = "ERR|The Duo secret key passed to 'sign request' is invalid.";
        public static string ERR_AKEY = "ERR|The application secret key passed to 'sign request' must be at least 40 characters.";
        public static string ERR_UNKNOWN = "ERR|An unknown error has occurred.";

        // throw on invalid bytes
        private static Encoding _encoding = new UTF8Encoding(false, true);

        public static string SignRequest(string integrationkey, string secretkey, string akey, string username, DateTime? currentTime = null)
        {
            string duo_sig;
            string app_sig;

            DateTime currentTimeValue = currentTime ?? DateTime.UtcNow;

            if (username == "")
            {
                return ERR_USER;
            }
            if (username.Contains("|"))
            {
                return ERR_USER;
            }
            if (integrationkey.Length != IKEY_LEN)
            {
                return ERR_IKEY;
            }
            if (secretkey.Length != SKEY_LEN)
            {
                return ERR_SKEY;
            }
            if (akey.Length < AKEY_LEN)
            {
                return ERR_AKEY;
            }

            try
            {
                duo_sig = SignVals(secretkey, username, integrationkey, DUO_PREFIX, DUO_EXPIRE, currentTimeValue);
                app_sig = SignVals(akey, username, integrationkey, APP_PREFIX, APP_EXPIRE, currentTimeValue);
            }
            catch
            {
                return ERR_UNKNOWN;
            }

            return duo_sig + ":" + app_sig;
        }

        public static string? VerifyResponse(string integrationkey, string secretkey, string akey, string sigResponse, DateTime? currentTime = null)
        {
            string? authUser = null;
            string? appUser = null;

            DateTime currentTimeValue = currentTime ?? DateTime.UtcNow;

            try
            {
                string[] sigs = sigResponse.Split(':');
                string auth_sig = sigs[0];
                string app_sig = sigs[1];

                authUser = ParseVals(secretkey, auth_sig, AUTH_PREFIX, integrationkey, currentTimeValue);
                appUser = ParseVals(akey, app_sig, APP_PREFIX, integrationkey, currentTimeValue);
            }
            catch
            {
                return null;
            }

            if (authUser != appUser)
            {
                return null;
            }

            return authUser;
        }

        private static string SignVals(string key, string username, string integrationkey, string prefix, Int64 expire, DateTime currentTime)
        {

            Int64 ts = (Int64)(currentTime - new DateTime(1970, 1, 1)).TotalSeconds;
            expire = ts + expire;

            string val = username + "|" + integrationkey + "|" + expire.ToString();
            string cookie = prefix + "|" + Encode64(val);

            string sig = HmacSign(key, cookie);

            return cookie + "|" + sig;
        }

        private static string? ParseVals(string key, string val, string prefix, string ikey, DateTime currentTime)
        {
            Int64 ts = (int)(currentTime - new DateTime(1970, 1, 1)).TotalSeconds;

            string[] parts = val.Split('|');
            if (parts.Length != 3)
            {
                return null;
            }

            string uPrefix = parts[0];
            string uB64 = parts[1];
            string uSig = parts[2];

            string sig = HmacSign(key, uPrefix + "|" + uB64);
            if (HmacSign(key, sig) != HmacSign(key, uSig))
            {
                return null;
            }

            if (uPrefix != prefix)
            {
                return null;
            }

            string cookie = Decode64(uB64);
            string[] cookieParts = cookie.Split('|');
            if (cookieParts.Length != 3)
            {
                return null;
            }

            string username = cookieParts[0];
            string uIkey = cookieParts[1];
            string expire = cookieParts[2];

            if (uIkey != ikey)
            {
                return null;
            }

            long expireTs = Convert.ToInt64(expire);
            if (ts >= expireTs)
            {
                return null;
            }

            return username;
        }

        private static string HmacSign(string secretkey, string data)
        {
            byte[] keyBytes = _encoding.GetBytes(secretkey);

            using (HMACSHA1 hmac = new HMACSHA1(keyBytes))
            {
                byte[] dataBytes = _encoding.GetBytes(data);
                hmac.ComputeHash(dataBytes);

                if (hmac.Hash == null)
                {
                    return string.Empty;
                }

                string hex = BitConverter.ToString(hmac.Hash);

                return hex.Replace("-", "").ToLower();
            }
        }

        private static string Encode64(string plaintext)
        {
            byte[] plaintextBytes = _encoding.GetBytes(plaintext);
            return System.Convert.ToBase64String(plaintextBytes);
        }

        private static string Decode64(string encoded)
        {
            byte[] plaintext_bytes = System.Convert.FromBase64String(encoded);
            return _encoding.GetString(plaintext_bytes);
        }
    }
}