using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace UtilityLibrary.Common
{
    public static class Utility
    {
        public static bool IsValidDate(this object d)
        {
            try
            {
                string s = Convert.ToString((DateTime)d);
                DateTime p = DateTime.MinValue;
                if (DateTime.TryParse(s, out p)) return true;
                return false;
            }
            catch (Exception) { return default; }
        }

        public static string Mask(this string value, int startIndex = 0, int maskLength = 5, char maskCharacter = '*')
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            if ((value.Length - startIndex) > maskLength)
                return string.Concat(value.Substring(0, startIndex).PadRight(startIndex + maskLength, maskCharacter), value[(startIndex + maskLength)..]);

            return string.Concat(value.Substring(0, startIndex).PadRight(startIndex + maskLength, maskCharacter), value[value.Length..]);
        }

        public static string? ToStringItems<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items != null ? string.Join(separator, items) : null;
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items.ToDelimitedString(p => p, separator);
        }

        public static string ToDelimitedString<S, T>(this IEnumerable<S> items, Func<S, T> selector, string separator = ",")
        {
            return string.Join(separator, items.Select(selector));
        }

        public static int ToInt(this object val)
        {
            try
            {
                var b = int.TryParse(val.ToString(), out int x);
                return x;
            }
            catch (Exception) { return default; }
        }

        public static long ToLong(this object val)
        {
            try
            {
                var b = long.TryParse(val.ToString(), out long x);
                return x;
            }
            catch (Exception) { return default; }
        }

        public static decimal ToDecimal(this object val)
        {
            try
            {
                var b = decimal.TryParse(val.ToString(), out decimal x);
                return x;
            }
            catch (Exception) { return default; }
        }

        public static float ToFloat(this object val)
        {
            try
            {
                var b = float.TryParse(val.ToString(), out float x);
                return x;
            }
            catch (Exception) { return default; }
        }

        public static bool ToBool(this object val)
        {
            try
            {
                var b = bool.TryParse(val.ToString(), out bool x);
                return x;
            }
            catch (Exception) { return default; }
        }

        public static DateTime? ToDate(this string d, string dateFormat = null)
        {
            if (string.IsNullOrEmpty(d)) return null;
            try
            {
                DateTime p;
                if (dateFormat == null)
                {
                    var formats = new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "yyyy-MM-dd", "yyyy-MM-ddT00:00:00.000" };
                    if (DateTime.TryParseExact(d.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
                        return p;
                }
                else
                {
                    if (DateTime.TryParseExact(d.Trim(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
                        return p;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public static DateTime? ToDate(this string d, string dateFormat, CultureInfo culture_info)
        {
            if (string.IsNullOrEmpty(d)) return null;
            try
            {
                if (DateTime.TryParseExact(d.Trim(), dateFormat, culture_info, DateTimeStyles.None, out DateTime p))
                    return p;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string AllowedXters(this string value, int allowed_length)
        {
            string trim_val = String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > allowed_length)
                {
                    trim_val = value.Substring(0, allowed_length - 1);
                }
                else
                {
                    trim_val = value;
                }
            }

            return trim_val;
        }

        public static string Timestamp()
        {
            //create Timespan by subtracting the value provided from the Unix Epoch
            TimeSpan span = (System.DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return Math.Round(Convert.ToDouble(span.TotalSeconds), 0).ToString();
        }
        public static string ToDashFiFormat(this DateTime date)
        {
            var newDate = date.ToString("dd-MM-yyyy");

            return newDate;
        }

        public static Guid UniqueId()
        {
            return Guid.NewGuid();
        }

        public static string MapMonthNumberToShortName(int monthId)
        {
            string[] monthlist = { "Jan", "Feb", "March", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            return monthlist[monthId - 1];
        }

        public static string CreateRandomPasswordWithRandomLength()
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            int size = random.Next(16, validChars.Length);

            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(chars);
        }

        public static int GetWeekNumber(DateTime value)
        {
            decimal d = value.Day / 7;

            return (int)Math.Floor(d) + 1;
        }

        public static int GetDaysInYear(int year)
        {
            return DateTime.IsLeapYear(year) ? 366 : 365;
        }

        public static decimal GetAccruedInterest(decimal amount, int tenor, float interestRate, int year)
        {
            var rate = interestRate / 100;
            return (amount * (decimal)rate * tenor) / GetDaysInYear(year);
        }

        public static string Encrypt(string value, string cryptoKey)
        {

            try
            {


                if (string.IsNullOrEmpty(value)) return string.Empty;

                byte[] privateKeyBytes = Convert.FromBase64String(cryptoKey);
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(value);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = privateKeyBytes;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.GenerateIV();

                    byte[] iv = aesAlg.IV;

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv))
                    {
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                                csEncrypt.FlushFinalBlock();
                                byte[] encryptedBytes = msEncrypt.ToArray();
                                byte[] combinedBytes = iv.Concat(encryptedBytes).ToArray();
                                string encryptedPassword = Convert.ToBase64String(combinedBytes);

                                return encryptedPassword;
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static string Decrypt(string encryptedValue, string cryptoKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedValue))
                    return string.Empty;

                byte[] privateKeyBytes = Convert.FromBase64String(cryptoKey);
                byte[] combinedBytes = Convert.FromBase64String(encryptedValue);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = privateKeyBytes;
                    aesAlg.Mode = CipherMode.CBC;

                    // Extract IV from combined bytes
                    byte[] iv = combinedBytes.Take(aesAlg.IV.Length).ToArray();
                    byte[] encryptedBytes = combinedBytes.Skip(aesAlg.IV.Length).ToArray();

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    string decryptedValue = srDecrypt.ReadToEnd();
                                    return decryptedValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Encode a string to Base64
        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        // Decode a Base64 encoded string
        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static string Decrypt2(string cipherText, string password)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    // extract salt (first 16 bytes)
                    var salt = cipherBytes.Take(16).ToArray();
                    // extract iv (next 16 bytes)
                    var iv = cipherBytes.Skip(16).Take(16).ToArray();
                    // the rest is encrypted data
                    var encrypted = cipherBytes.Skip(32).ToArray();
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, 100);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.Padding = PaddingMode.PKCS7;
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.IV = iv;
                    // you need to decrypt this way, not the way in your question
                    using (MemoryStream ms = new MemoryStream(encrypted))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var reader = new StreamReader(cs, Encoding.UTF8))
                            {
                                var res = reader.ReadToEnd();
                                return res;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}