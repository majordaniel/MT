

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace UtilityLibrary.Extensions
{
    public static partial class Xtenxion
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T CastTo<T>(this object val)
        {
            T x = default(T);
            try
            {
                x = (T)Convert.ChangeType(val, typeof(T));
                return x;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        //public static string ImageToBase64(this System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        // Convert Image to byte[]
        //        image.Save(ms, format);
        //        byte[] imageBytes = ms.ToArray();

        //        // Convert byte[] to Base64 String
        //        string base64String = Convert.ToBase64String(imageBytes);
        //        return base64String;
        //    }
        //}

        /// <summary>
        /// Read file from path into base64 string
        /// </summary>
        /// <param name="filepath">the file path</param>
        /// <returns></returns>
        public static string FileToBase64(this string filepath)
        {

            Byte[] bytes = File.ReadAllBytes(filepath);

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(bytes);
            return base64String;
        }



        /// <summary>
        /// Convert from base64 string to decoded string using UTF8
        /// </summary>
        /// <param name="StrVal">the string value</param>
        /// <returns></returns>
        public static string ToBase64_UTF8(this string StrVal)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(StrVal));
        }

        /// <summary>
        /// Convert from base64 string to decoded string using ASCII
        /// </summary>
        /// <param name="StrVal">the string value</param>
        /// <returns></returns>
        public static string ToBase64_ASCII(this string StrVal)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(StrVal));
        }


        /// <summary>
        /// Convert from base64 string to decoded string
        /// </summary>
        /// <param name="Base64Str">the base64 string</param>
        /// <returns></returns>
        public static string FromBase64(this string Base64Str)
        {
            byte[] data = Convert.FromBase64String(Base64Str);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }



        /// <summary>
        /// Save base64 string filepath into 
        /// </summary>
        /// <param name="base64String">the base64 string</param>
        /// <param name="filepath">the file path</param>
        /// <returns></returns>
        public static bool SaveBase64(this string base64String, string filepath)
        {
            Byte[] bytes = Convert.FromBase64String(base64String);

            // get the directory
            var folder = Path.GetDirectoryName(filepath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllBytes(filepath, bytes);

            return true;
        }


        public static void PrintFile()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.NoDelay = true;

            IPAddress ip = IPAddress.Parse("192.168.192.6");
            IPEndPoint ipep = new IPEndPoint(ip, 9100);
            clientSocket.Connect(ipep);

            byte[] fileBytes = File.ReadAllBytes("test.txt");

            clientSocket.Send(fileBytes);
            clientSocket.Close();
        }

        #region CollectionHelperExtension

        public static IEnumerable<T> Add<T>(this IEnumerable<T> sequence, T item)
        {
            return (sequence ?? Enumerable.Empty<T>()).Concat(new[] { item });
        }

        public static T[] AddRangeToArray<T>(this T[] sequence, T[] items)
        {
            return (sequence ?? Enumerable.Empty<T>()).Concat(items).ToArray();
        }

        public static T[] AddToArray<T>(this T[] sequence, T item)
        {
            return Add(sequence, item).ToArray();
        }

        public static void Resize<T>(ref T[] array, int newSize)
        {
            if (newSize < 0)
            {
                throw new ArgumentOutOfRangeException("newSize");
            }
            T[] sourceArray = array;
            if (sourceArray == null)
            {
                array = new T[newSize];
            }
            else if (sourceArray.Length != newSize)
            {
                T[] destinationArray = new T[newSize];
                Array.Copy(sourceArray, 0, destinationArray, 0, (sourceArray.Length > newSize) ? newSize : sourceArray.Length);
                array = destinationArray;
            }
        }

        private static T[] MergeArray<T>(this T[] originalArray, T[] newArray)
        {
            int startIndexForNewArray = originalArray.Length;
            Array.Resize<T>(ref originalArray, originalArray.Length + newArray.Length);
            newArray.CopyTo(originalArray, startIndexForNewArray);
            return originalArray;
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            return source.Union(new TSource[] { item });
        }

        #endregion

        public static string RemoveSpecialCharacters(this string str, params char[] special_xters)
        {

            try
            {
                foreach (char c in special_xters)
                {
                    try
                    {
                        if (special_xters.Contains(c))
                        {
                            str = str.Replace(c.ToString(), String.Empty);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }

            return str;
        }

        public static string TrimSpecialCharacters(this string str)
        {
            try
            {
                return Regex.Replace(str, @"[^a-zA-Z0-9_\.]+&-", "", RegexOptions.Compiled);
            }
            catch (Exception)
            {

            }

            return str;
        }

        /// <summary>
        /// method to get Client ip address
        /// </summary>
        /// <param name="GetLan"> set to true if want to get local(LAN) Connected ip address</param>
        /// <returns></returns>
        public static string GetVisitorIPAddress(bool GetLan = false)
        {
            string ipAddr = null;

            try
            {
               // ipAddr = new NetworkService().GetVisitorIPAddress(GetLan);
            }
            catch (Exception)
            {

            }

            return ipAddr;
        }

        /// <summary>
        /// Get the IPv4address of a remote client machine.
        /// </summary>
        /// <returns></returns>
        public static string GetIPv4Address()
        {
            string IPv4Address = string.Empty;
            string strHostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry iphe = System.Net.Dns.GetHostEntry(strHostName);

            foreach (System.Net.IPAddress ipheal in iphe.AddressList)
            {
                if (ipheal.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPv4Address = ipheal.ToString();
                }
            }
            return IPv4Address;
        }

        /// <summary>
        /// Get the MAC address of a remote client machine within the same network as the host.
        /// </summary>
        /// <returns></returns>
        private static string GetMAC()
        {
            string macAddresses = null;
            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        macAddresses += nic.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }

            return macAddresses;
        }

        /// <summary>
        /// Returns the minimum of two dates.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static DateTime? GetMin(DateTime? t1, DateTime? t2)
        {
            if (DateTime.Compare(t1.Value, t2.Value) > 0)
            {
                return t2;
            }
            return t1;
        }

        /// <summary>
        /// Returns the maximum of two dates.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static DateTime? GetMax(DateTime? t1, DateTime? t2)
        {
            if (DateTime.Compare(t1.Value, t2.Value) < 0)
            {
                return t2;
            }
            return t1;
        }

        /// <summary>
        /// Compares two values of type T and returns the maximum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static T MaxVal<T>(T value1, T value2) where T : struct, IComparable<T>
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        /// <summary>
        /// Compares two values of type T and returns the minimum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static T MinVal<T>(T value1, T value2) where T : struct, IComparable<T>
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        /// <summary>
        /// Compares two values and returns the maximum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static T Max<T>(this T value1, T value2) where T : struct, IComparable<T>
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        /// <summary>
        /// Compares two values and returns the minimum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static T Min<T>(this T value1, T value2) where T : struct, IComparable<T>
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        /// <summary>
        /// Gets the maximum value of dates in an array or list of values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="other_vals"></param>
        /// <returns></returns>
        public static T MaxOf<T>(this T val, params T[] other_vals) //where T : struct, IComparable, IEnumerable, ICollection, IList, IQueryable, IDictionary
        {
            T max_val = default(T);

            if (other_vals != null)
            {
                other_vals = other_vals.AddToArray(val);
                return other_vals.Max<T>();
            }

            return max_val;
        }

        // <summary>
        /// Gets the minimum value of dates in an array or list of values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="other_vals"></param>
        /// <returns></returns>
        public static T MinOf<T>(this T val, params T[] other_vals) //where T : struct, IComparable, IEnumerable, ICollection, IList, IQueryable, IDictionary
        {
            T max_val = default(T);

            if (other_vals != null)
            {
                other_vals = other_vals.AddToArray(val);
                return other_vals.Max<T>();
            }

            return max_val;
        }


        /// <summary>
        /// Many times you may wish to impose boundaries on what a certain variable can be. 
        /// <para />This is especially useful for validating user input. For any comparable, it simply returns the value, truncated by a minimum or maximum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T ConstrainToRange<T>(this T d, T min, T max) where T : IComparable
        {
            if (d.CompareTo(min) < 0) return min;
            else if (d.CompareTo(max) > 0) return max;
            else return d;
        }

        /// <summary>
        /// Works on Comparables to check whether the checked value is between two other values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static bool Between<T>(this T me, T lower, T upper) where T : IComparable<T>
        {
            return me.CompareTo(lower) >= 0 && me.CompareTo(upper) < 0;
        }

        /// <summary>
        /// Returns a boolean value of true if the item being compared is less than the value of the parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparable"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// This example shows how to use the <see cref="LessThan"/>extension method.
        public static bool LessThan<T>(this IComparable<T> comparable, T other)
        {
            return comparable.CompareTo(other) < 0;
        }

        /// <summary>
        /// Limits a value to a maximum. For example this is usefull if you want to feed a progressBar 
        /// with values from a source which eventually might exceed an expected maximum. 
        /// <para />This is a generic extension method with IComparable&lt;T&gt; constraint. 
        /// So every type which implements the IComparable interface benefits from this extension.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        /// <example>
        ///  This sample shows how to call the <see cref="Limit"/> method.
        /// <code>
        /// class TestClass 
        /// {
        ///     static void TestMethod() 
        ///     {
        ///          int testValue = 150;
        ///          int limitedResult = testValue.Limit(100);
        ///         //limitedResult will be 100 because it's the given maximum
        ///     }
        /// } 
        /// </code>
        /// </example>
        public static T Limit<T>(this T value, T maximum) where T : IComparable<T>
        {
            return value.CompareTo(maximum) < 1 ? value : maximum;
        }

        /// <summary>
        /// Returns the most detailed exception message in an exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ExceptionMessage(this Exception ex)
        {
            string errMsg = null;
            if (ex.InnerException != null)
            {
                if (ex.InnerException.InnerException != null)
                {
                    errMsg = ex.InnerException.InnerException.Message.Replace("\n", null);
                }
                else
                {
                    errMsg = ex.InnerException.Message.Replace("\n", null);
                }
            }
            else
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        //public static bool IsConnected(this DbContext db)
        //{
        //    if (db.Database != null)
        //    {
        //        if (db.Database.Connection != null)
        //        {
        //            if (db.Database.Connection.State == System.Data.ConnectionState.Open
        //                || db.Database.Connection.State == System.Data.ConnectionState.Executing)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        public static bool IN(this object search_val, string search_list)
        {
            var x = false;

            if (search_list != null && search_val != null)
            {

                var spp = search_list.ToLower().Split(new char[] { ',', ';', '|', '\\', '/', '!' });

                if (spp != null)
                {
                    var v = spp.Where(y =>
                        y.Equals(search_val.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();

                    if (v.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IN<T>(this object search_val, List<T> search_list)
        {
            var x = false;

            if (search_list != null && search_val != null)
            {
                var v = search_list.Where(y =>
                    y.ToString().Equals(search_val.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (v.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static object GetPropertyValue(this object o, string propertyName)
        {
            Type type = o.GetType();

            try
            {
                PropertyInfo info = (from x in type.GetProperties() where x.Name.ToLower() == propertyName.ToLower() select x).First();
                object value = info.GetValue(o, null);
                return value;
            }
            catch (Exception)
            {
                return default(object);
            }
        }

        public static T GetFieldValue<T>(this object o, string propertyName) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            try
            {
                var val = GetPropertyValue(o, propertyName);
                return (T)val;
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        public static bool IsNumeric_2(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static bool IsNumeric_3(this System.Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal ||
                Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                {
                    Double.Parse(Expression as string);
                }
                else
                {
                    Double.Parse(Expression.ToString());
                }
                return true;
            }
            catch
            {
            } // just dismiss errors but return false

            return false;
        }

        static private List<String> AcceptableDateFormats = new List<String>(180);
        static Boolean IsDate_2(Object value, DateTimeFormatInfo formatInfo)
        {
            if (AcceptableDateFormats.Count == 0)
            {
                foreach (var dateFormat in new[] { "d", "dd" })
                {
                    foreach (var monthFormat in new[] { "M", "MM", "MMM" })
                    {
                        foreach (var yearFormat in new[] { "yy", "yyyy" })
                        {
                            foreach (var separator in new[] { "-", "/", formatInfo.DateSeparator })
                            {
                                String shortDateFormat;
                                shortDateFormat = dateFormat + separator + monthFormat + separator + yearFormat;
                                AcceptableDateFormats.Add(shortDateFormat);
                                AcceptableDateFormats.Add(shortDateFormat + " " + "HH:mm");
                                AcceptableDateFormats.Add(shortDateFormat + " " + "HH:mm:ss");
                                AcceptableDateFormats.Add(shortDateFormat + " " + "HH" + formatInfo.TimeSeparator + "mm");
                                AcceptableDateFormats.Add(shortDateFormat + " " + "HH" + formatInfo.TimeSeparator + "mm" + formatInfo.TimeSeparator + "ss");
                            }
                        }
                    }
                }
                AcceptableDateFormats = AcceptableDateFormats.Distinct().ToList();
            }

            DateTime unused;
            return DateTime.TryParseExact(value.ToString(), AcceptableDateFormats.ToArray(), formatInfo, DateTimeStyles.AllowWhiteSpaces, out unused);
        }

        public static bool IsDate_3(object sdate)
        {
            DateTime dt;
            bool isDate = true;

            try
            {
                dt = DateTime.Parse(string.Format("{0}", sdate));
            }
            catch
            {
                isDate = false;
            }

            return isDate;
        }

        public static string GenerateUniqueID(int length = 0, string prefix = null)
        {
            var id = Guid.NewGuid().ToString().Replace("-", string.Empty);
            if (length == 0 || string.IsNullOrEmpty((length.ToString())))
            {
                length = id.Length;
            }
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix) || prefix == null)
            {
                return id.Substring(0, length).ToUpper();
            }
            else
            {
                return prefix + id.Substring(0, length).ToUpper();
            }
        }

        public static string GenerateUniqueIDFromDate(int length = 0, string prefix = null)
        {
            var id = DateTime.Now.Ticks.ToString();
            if (length == 0 || string.IsNullOrEmpty((length.ToString())))
            {
                length = id.Length;
            }

            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix) || prefix == null)
            {
                return id.Substring(0, length).ToUpper();
            }
            else
            {
                return prefix + id.Substring(0, length).ToUpper();
            }
        }

        public static string ToFinacleDate(this DateTime d)
        {
            try
            {
                string resp = d.ToString("yyyy-MM-dd'T'HH:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd'T'HH:mm:ss.fff");
            }
        }

        //public static bool IsNull(this string d)
        //{
        //    try
        //    {
        //        return string.IsNullOrEmpty(d);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return true;
        //}

        //public static bool IsNull(this object d)
        //{
        //    try
        //    {
        //        return (d == null);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return true;
        //}

        public static bool IsNumeric(this object d)
        {
            try
            {
                string s = Convert.ToString(d);
                long p = 0;

                if (long.TryParse(s, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool IsDateString(this string d)
        {
            try
            {
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(d, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool isEmail(this string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    email = null;
                    return false;
                }
                else
                {
                    try
                    {
                        MailAddress m = new MailAddress(email);

                        return true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }

            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool isEmail(this object email)
        {
            try
            {
                if (email == null)
                {
                    email = null;
                }
                else
                {
                    try
                    {
                        MailAddress m = new MailAddress(email.ToString());

                        return true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static string ToFinacleDate(this DateTime? d)
        {
            try
            {
                string resp = d.Value.ToString("yyyy-MM-dd'T'hh:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd'T'hh:mm:ss.fff");
            }

        }

        public static string ToFinacleDate(this object d)
        {
            try
            {
                string resp = ((DateTime)d).ToString("yyyy-MM-dd'T'hh:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd'T'hh:mm:ss.fff");
            }

        }

        public static string ToSQLDate(this DateTime? d)
        {
            try
            {
                string resp = d.Value.ToString("yyyy-MM-dd hh:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd hh:mm:ss.fff");
            }

        }

        public static string ToSQLDate(this object d)
        {
            try
            {
                string resp = ((DateTime)d).ToString("yyyy-MM-dd hh:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd hh:mm:ss.fff");
            }

        }

        public static string ToSalesforceDate(this DateTime? d)
        {
            try
            {
                string resp = d.Value.ToString("yyyy-MM-dd hh:mm:ss.fff");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd hh:mm:ss.fff");
            }

        }

        public static string ToSalesforceDate(this object d)
        {
            try
            {
                string resp = ((DateTime)d).ToString("YYYY-MM-SSTHH:mm:ss.0Z");
                return resp;
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString("YYYY-MM-SSTHH:mm:ss.0Z");
            }

        }

        #region Added these extensions for date (10-JAN-2017)

        /// <summary>
        /// This extension method allows you to convert a nullable datetime to the specified string date format (e.g. dd-MM-yyyy)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="date_format"></param>
        /// <returns>string value in the specified date format</returns>
        public static string Format(this DateTime? d, string date_format)
        {
            try
            {
                string s = string.Format("{0:" + date_format + "}", d.Value);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert a datetime to the specified string date format (e.g. dd-MM-yyyy)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="date_format"></param>
        /// <returns>string value in the specified date format</returns>
        public static string Format(this DateTime d, string date_format)
        {
            try
            {
                string s = string.Format("{0:" + date_format + "}", d);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert any object of type datetime to the specified string date format (e.g. dd-MM-yyyy)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="date_format"></param>
        /// <returns>string value in the specified date format</returns>
        public static string Format(this object d, string date_format)
        {
            try
            {
                string str_date = Convert.ToString(((DateTime)d));
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(str_date, out p))
                {
                    string s = string.Format("{0:" + date_format + "}", p);
                    return s;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }
        #endregion


        #region Added these extensions for numbers (10-JAN-2017)
        /// <summary>
        /// This extension method allows you to convert a nullable decimal value to the specified string number format (e.g. N2, C2, etx)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="number_format"></param>
        /// <returns>string value in the specified number format</returns>
        public static string Format(this decimal? d, string number_format)
        {
            try
            {
                string s = string.Format("{0:" + number_format + "}", d.Value);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert a decimal value to the specified string number format (e.g. N2, C2, etx)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="number_format"></param>
        /// <returns>string value in the specified number format</returns>
        public static string Format(this decimal d, string number_format)
        {
            try
            {
                string s = string.Format("{0:" + number_format + "}", d);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert a nullablle integer value to the specified string number format (e.g. N2, C2, etx)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="number_format"></param>
        /// <returns>string value in the specified number format</returns>
        public static string Format(this int? d, string number_format)
        {
            try
            {
                string s = string.Format("{0:" + number_format + "}", d.Value);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert an integer value to the specified string number format (e.g. N2, C2, etx)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="number_format"></param>
        /// <returns>string value in the specified number format</returns>
        public static string Format(this int d, string number_format)
        {
            try
            {
                string s = string.Format("{0:" + number_format + "}", d);
                return s;
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// This extension method allows you to convert any object of number value to the specified string number format (e.g. N2, C2, etx)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="number_format"></param>
        /// <returns>string value in the specified number format</returns>
        public static string FormatNumber(this object d, string number_format)
        {
            try
            {
                if (IsNumeric(d))
                {
                    decimal val = (decimal)d;
                    string s = string.Format("{0:" + number_format + "}", val);
                    return s;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        #endregion

        public static bool IsDate(this DateTime d)
        {
            try
            {
                string s = Convert.ToString(d);
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(s, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool IsDate(this DateTime? d)
        {
            try
            {
                string s = Convert.ToString(d.Value);
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(s, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool IsDate(this object d)
        {
            try
            {
                string s = Convert.ToString(((DateTime)d));
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(s, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }


        /// <summary>
        ///  Converts the string to a datetime using a date format and a culture type
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        //public static DateTime? ToDate(this string d, string dateFormat = null)
        //{
        //    DateTime p = DateTime.MinValue;

        //    if (d == null || string.IsNullOrEmpty(d))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        if (dateFormat == null)
        //        {
        //            var formats = new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "yyyy-MM-dd", "yyyy-MM-ddT00:00:00.000" };
        //            if (DateTime.TryParseExact(d.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
        //            {
        //                Console.WriteLine("Converted '{0}' to {1} ({2}).", d, p, p.Kind);
        //                return p;
        //            }
        //            else
        //            {
        //                Console.WriteLine("'{0}' is not in an acceptable format.", d);
        //                throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));

        //            }
        //        }
        //        else
        //        {
        //            if (DateTime.TryParseExact(d.Trim(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
        //            {
        //                Console.WriteLine("Converted '{0}' to {1} ({2}).", d, p, p.Kind);
        //                return p;
        //            }
        //            else
        //            {
        //                Console.WriteLine("'{0}' is not in an acceptable format.", d);
        //                throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("invalid date: " + ex.ExceptionMessage());
        //    }
        //    //return p;
        //}

        /// <summary>
        /// Converts the string to a datetime using a date format and a culture type
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dateFormat"></param>
        /// <param name="culture"> e.g DateCulture.en_US;</param>
        /// <returns></returns>
        //public static DateTime? ToDate(this string d, string dateFormat, DateCulture culture)
        //{
        //    DateTime p = DateTime.MinValue;
        //    CultureInfo culture_info = CultureInfo.InvariantCulture;

        //    if (d == null || string.IsNullOrEmpty(d))
        //    {
        //        return null;
        //    }

        //    switch (culture)
        //    {
        //        case DateCulture.en_GB:
        //            {
        //                culture_info = new CultureInfo("en_GB");
        //                break;
        //            }
        //        case DateCulture.en_US:
        //            {
        //                culture_info = new CultureInfo("en_US");
        //                break;
        //            }
        //        default:
        //            {
        //                culture_info = new CultureInfo("en_US");
        //                break;
        //            }
        //    }

        //    try
        //    {
        //        if (DateTime.TryParseExact(d.Trim(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
        //        {
        //            Console.WriteLine("Converted '{0}' to {1} ({2}).", d, p, p.Kind);
        //            return p;
        //        }
        //        else
        //        {
        //            Console.WriteLine("'{0}' is not in an acceptable format.", d);
        //            throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //return p;
        //}

        /// <summary>
        /// Converts the string to a datetime using a date format and a culture type
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dateFormat"></param>
        /// <param name="culture_info"> e.g CultureInfo enUS = new CultureInfo("en-US");</param>
        /// <returns></returns>
        //public static DateTime? ToDate(this string d, string dateFormat, CultureInfo culture_info)
        //{
        //    DateTime p = DateTime.MinValue;

        //    if (d == null || string.IsNullOrEmpty(d))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        if (DateTime.TryParseExact(d.Trim(), dateFormat, culture_info, DateTimeStyles.None, out p))
        //        {
        //            Console.WriteLine("Converted '{0}' to {1} ({2}).", d, p, p.Kind);
        //            return p;
        //        }
        //        else
        //        {
        //            Console.WriteLine("'{0}' is not in an acceptable format.", d);
        //            throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //return p;
        //}

        /// <summary>
        ///  Converts the string to a datetime using a date format and a culture type
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static DateTime? ToDate4Object(this object val, string dateFormat = null)
        {
            DateTime p = DateTime.MinValue;
            var d = val.ToString();

            if (d == null || string.IsNullOrEmpty(d))
            {
                return null;
            }

            try
            {
                if (dateFormat == null)
                {
                    var formats = new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "yyyy-MM-dd" };
                    if (DateTime.TryParseExact(d, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
                    {
                        return p;
                    }
                    else
                    {
                        throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));
                    }
                }
                else
                {
                    if (DateTime.TryParseExact(d, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out p))
                    {
                        return p;
                    }
                    else
                    {
                        throw new Exception(string.Format("'{0}' is not in an acceptable format.", d));

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return p;
        }

        /// <summary>
        ///  Converts the Excel AO value to date format and a culture type
        /// </summary>
        /// <param name="d">the date string</param>  
        /// <param name="date_system">the date system enabled in the excel file</param>
        /// <returns></returns>
        //public static DateTime? OAToDate(this string d, DateSystem date_system)
        //{
        //    if (d == null || string.IsNullOrEmpty(d))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        //Don't ask me how, just ask microsoft: follow the link below
        //        /* Reference website: https://support.microsoft.com/en-in/help/214330/differences-between-the-1900-and-the-1904-date-system-in-excel
        //                                          * By default, Microsoft Excel for Windows uses the 1900 date system. 
        //                                        * use this for excels created on MS-DOS and Windows platforms
        //                                        */
        //        // DateTime oaEpoch = new DateTime(1900, 1, 1); 

        //        /* This mock data excel file has the 'use 1904 data system' turned on when I checked the advanced settings.
        //         * It was probably created on a Mac system because  
        //         * use this for excel files created on the macintosh platform
        //         */

        //        DateTime oaEpoch = new DateTime(1900, 1, 1);

        //        if (date_system == DateSystem.Mac)
        //        {
        //            oaEpoch = new DateTime(1904, 1, 1);
        //        }

        //        double x = d.ToDate().Value.ToOADate();
        //        var val = oaEpoch + TimeSpan.FromTicks(Convert.ToInt64(x * TimeSpan.TicksPerDay));
        //        return val;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    //return p;
        //}


        /// <summary>
        ///  Converts the Excel AO value to date format and a culture type
        /// </summary>
        /// <param name="d">the date value</param>  
        /// <param name="date_system">the date system enabled in the excel file</param>
        /// <returns></returns>
        //public static DateTime? OAToDate(this object d, DateSystem date_system)
        //{
        //    if (d == null)
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        var val = OAToDate(d.ToString(), date_system);
        //        return val;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    //return p;
        //}



        /// <summary>
        /// Find the difference between two dates and return the value in days  
        /// </summary>
        /// <param name="FromDate">The start date</param>
        /// <param name="ToDate">The end date</param>
        /// <returns>difference in days</returns>
        public static int DaysBetween(this DateTime FromDate, DateTime ToDate)
        {
            TimeSpan span = ToDate.Subtract(FromDate);
            return (int)span.TotalDays;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in hours  
        /// </summary>
        /// <param name="FromDate">The nullable start date</param>
        /// <param name="ToDate">The nullable end date</param>
        /// <returns>difference in hours</returns>
        public static int HoursBetween(this DateTime FromDate, DateTime ToDate)
        {
            TimeSpan span = ToDate.Subtract(FromDate);
            return (int)span.TotalHours;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in minutes 
        /// </summary>
        /// <param name="FromDate">The nullable start date</param>
        /// <param name="ToDate">The nullable end date</param>
        /// <returns>difference in minutes</returns>
        public static int MinutesBetween(this DateTime FromDate, DateTime ToDate)
        {
            TimeSpan span = ToDate.Subtract(FromDate);
            return (int)span.TotalMinutes;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in seconds 
        /// </summary>
        /// <param name="FromDate">The nullable start date</param>
        /// <param name="ToDate">The nullable end date</param>
        /// <returns>difference in seconds</returns>
        public static long SecondsBetween(this DateTime FromDate, DateTime ToDate)
        {
            TimeSpan span = ToDate.Subtract(FromDate);
            return (int)span.TotalSeconds;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in milliseconds 
        /// </summary>
        /// <param name="FromDate">The nullable start date</param>
        /// <param name="ToDate">The nullable end date</param>
        /// <returns>difference in milliseconds</returns>
        public static long MilliSecsBetween(this DateTime FromDate, DateTime ToDate)
        {
            TimeSpan span = ToDate.Subtract(FromDate);
            return (int)span.TotalMilliseconds;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in days  
        /// </summary>
        /// <param name="FromDate">The nullable start date</param>
        /// <param name="ToDate">The nullable end date</param>
        /// <returns>difference in days</returns>
        public static int DaysBetween(this DateTime? FromDate, DateTime? ToDate)
        {
            TimeSpan span = ToDate.Value.Subtract(FromDate.Value);
            return (int)span.TotalDays;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in hours 
        /// </summary>
        /// <param name="FromDate">The start date</param>
        /// <param name="ToDate">The end date</param>
        /// <returns>difference in hours</returns>
        public static long HoursBetween(this DateTime? FromDate, DateTime? ToDate)
        {
            TimeSpan span = ToDate.Value.Subtract(FromDate.Value);
            return (int)span.TotalHours;
        }

        /// <summary>
        /// Find the difference between two dates and return the value in minutes 
        /// </summary>
        /// <param name="FromDate">The start date</param>
        /// <param name="ToDate">The end date</param>
        /// <returns>difference in minutes</returns>
        public static long MinutesBetween(this DateTime? FromDate, DateTime? ToDate)
        {
            TimeSpan span = ToDate.Value.Subtract(FromDate.Value);
            return (int)span.TotalMinutes;
        }

        /// <summary>
        /// Find the difference between two dates and return the seconds value 
        /// </summary>
        /// <param name="FromDate">The start date</param>
        /// <param name="ToDate">The end date</param>
        /// <returns>difference in seconds</returns>
        public static long SecondsBetween(this DateTime? FromDate, DateTime? ToDate)
        {
            TimeSpan span = ToDate.Value.Subtract(FromDate.Value);
            return (int)span.TotalSeconds;
        }

        /// <summary>
        /// Find the difference between two dates and return the milliseconds value 
        /// </summary>
        /// <param name="FromDate">The start date</param>
        /// <param name="ToDate">The end date</param>
        /// <returns>difference in milliseconds</returns>
        public static long MilliSecsBetween(this DateTime? FromDate, DateTime? ToDate)
        {
            TimeSpan span = ToDate.Value.Subtract(FromDate.Value);
            return (int)span.TotalMilliseconds;
        }

        public static DateTime StartOfYear(this DateTime? d)
        {
            try
            {
                DateTime resp = new DateTime(d.Value.Year, 1, 1, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, 1, 1, 0, 0, 0);
                return resp;
            }

        }

        public static DateTime StartOfMonth(this DateTime? d)
        {
            try
            {
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, 1, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, 1, 0, 0, 0);
                return resp;
            }

        }

        public static DateTime StartOfDay(this DateTime? d)
        {
            try
            {
                if (!d.HasValue)
                {
                    d = DateTime.Now;
                }
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 0, 0, 0);
                return resp;
            }
        }

        public static DateTime StartOfYear(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, 1, 1, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, 1, 1, 0, 0, 0);
                return resp;
            }
        }

        public static DateTime StartOfMonth(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, d.Month, 1, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, d.Month, 1, 0, 0, 0);
                return resp;
            }

        }

        public static DateTime StartOfDay(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
                return resp;
            }

        }


        public static DateTime EndOfYear(this DateTime? d)
        {
            try
            {
                DateTime resp = new DateTime(d.Value.Year, 12, 31, 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, 12, 31, 23, 59, 59);
                return resp;
            }

        }

        public static DateTime EndOfMonth(this DateTime? d)
        {
            try
            {
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, DateTime.DaysInMonth(d.Value.Year, d.Value.Month), 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, 1, 23, 59, 59);
                return resp;
            }

        }


        public static DateTime EndOfDay(this DateTime? d)
        {
            try
            {
                if (!d.HasValue)
                {
                    d = DateTime.Now;
                }
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 23, 59, 59);
                return resp;
            }

        }


        public static DateTime EndOfYear(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, 12, 31, 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, 12, 31, 23, 59, 59);
                return resp;
            }

        }


        public static DateTime EndOfMonth(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month), 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, d.Month, 1, 23, 59, 59);
                return resp;
            }

        }

        public static DateTime EndOfDay(this DateTime d)
        {
            try
            {
                DateTime resp = new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
                return resp;
            }
            catch (Exception)
            {
                d = DateTime.Now;
                DateTime resp = new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
                return resp;
            }

        }

        public static int ToInt(this object val)
        {
            try
            {
                int x = default(int);
                var b = int.TryParse(val.ToString(), out x);

                return x;
            }
            catch (Exception)
            {
                return default(int);
            }
        }

        public static long ToLong(this object val)
        {
            try
            {
                long x = default(long);
                var b = long.TryParse(val.ToString(), out x);

                return x;
            }
            catch (Exception)
            {
                return default(long);
            }
        }

        public static decimal ToDecimal(this object val)
        {
            try
            {
                decimal x = default(decimal);
                var b = decimal.TryParse(val.ToString(), out x);

                return x;
            }
            catch (Exception)
            {
                return default(decimal);
            }
        }

        public static float ToFloat(this object val)
        {
            try
            {
                float x = default(float);
                var b = float.TryParse(val.ToString(), out x);

                return x;
            }
            catch (Exception)
            {
                return default(float);
            }
        }

        public static bool ToBool(this object val)
        {
            try
            {
                bool x = default(bool);
                var b = bool.TryParse(val.ToString(), out x);

                return x;
            }
            catch (Exception)
            {
                return default(bool);
            }
        }


        #region Added this extension for enums - 08-feb-2017

        public static int ToInt(this Enum e)
        {
            return e.GetValue<int>();
        }

        public static long ToInt64Value(this Enum e)
        {
            return e.GetValue<Int64>();
        }

        public static byte ToByte(this Enum e)
        {
            return e.GetValue<byte>();
        }

        public static T GetValue<T>(this Enum e) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            try
            {
                return (T)(object)e;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T ParseEnum<T>(this int? value)
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value.Value); ;
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        public static T ParseEnum<T>(this int value)
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value); ;
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        public static T ParseEnum<T>(this object value)
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value); ;
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        public static T ParseEnum<T>(this string value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true); ;
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        public static T ParseEnumString<T>(this object value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value.ToString(), true);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static bool IsValidEnum<T>(this object value)
        {
            try
            {
                var bb = (T)Enum.Parse(typeof(T), value.ToString(), true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        public static bool IsValidDate(object d)
        {
            try
            {
                string s = Convert.ToString(d);
                DateTime p = DateTime.MinValue;
                //CultureInfo.InvariantCulture
                if (DateTime.TryParse(s, out p))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static string GenerateCIF(int BankCode, long SequenceNo)
        {
            string nub = Generate(BankCode, 3, SequenceNo, 8);
            return nub;
        }

        public static string GenerateNuban(int BankCode, long SequenceNo)
        {
            string nub = Generate(BankCode, 3, SequenceNo, 9);

            return nub;
        }

        public static string Generate(int BankCode, int BankCodeMaxLength, long SequenceNo, int SequenceMaxLength = 9)
        {
            //Algorithm
            // -- Bank Code = ABC
            //-- NUBAN 9 Serial No = DEFGH;IJKL
            //-- Step 1. Calculate A*3+B*7+C*3+D*3+E*7+F*3+G*3+H*7+I*3+J*3+K*7+L*3
            //-- Step 2. Calculate Modulo 10 of your result i.e. the remainder after dividing by 10
            //-- Step 3. Subtract your result from 10 to get the Check Digit
            // nSumDigit = 10 - (nSumDigit % 10)
            //-- Step 4. If your result is 10, then use 0 as your check digit
            // if nSumDigit = 10
            //        nSumDigit = 0

            string nub = null;
            string bank = BankCode.ToString();

            if (bank.Length > 3)
            {
                throw new Exception("Bank code must be 3 digit CBN approved bank code.");
            }

            try
            {
                if (bank.Length < BankCodeMaxLength)
                {
                    bank = bank.PadLeft(BankCodeMaxLength, '0');
                }

                string seq = SequenceNo.ToString();

                if (seq.Length < SequenceMaxLength)
                {
                    seq = seq.PadLeft(SequenceMaxLength, '0');
                }


                string digi = string.Concat(bank, seq);
                var multi = new int[] { 3, 7, 3, 3, 7, 3, 3, 7, 3, 3, 7, 3 };

                int k = 0;
                long sum = 0;

                for (k = 0; k <= digi.Length - 1; k++)
                {
                    sum += (int)digi[k] * multi[k];
                }

                var nSumDigit = 10 - (sum % 10);

                if (nSumDigit == 10)
                {
                    nub = string.Concat(seq, "0");
                }
                else
                {
                    nub = string.Concat(seq, nSumDigit.ToString());
                }

            }
            catch (Exception)
            {

            }
            return nub;
        }

        public static string Mask(this string value, int start_index, int mask_length, char mask_xter = '*')
        {
            string masked_val = null;

            if (!string.IsNullOrEmpty(value))
            {
                if ((value.Length - start_index) > mask_length)
                {
                    masked_val = string.Concat(
                           value.Substring(0, start_index).PadRight(start_index + mask_length, mask_xter),
                           value.Substring(start_index + mask_length)
                       );
                }
                else
                {
                    masked_val = string.Concat(
                           value.Substring(0, start_index).PadRight(start_index + mask_length, mask_xter),
                           value.Substring(value.Length)
                       );
                }

            }

            return masked_val;
        }

        public static string MergeValues(this string value, Dictionary<string, object> MergeVals)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (var item in MergeVals)
                {
                    try
                    {
                        if (item.Value == null)
                        {
                            value = value.Replace(string.Concat("#", item.Key, "#"), null);
                        }
                        else
                        {
                            value = value.Replace(string.Concat("#", item.Key, "#"), item.Value == null ? null : item.Value.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        value = value.Replace(string.Concat("#", item.Key, "#"), null);
                    }
                }
            }
            return value;
        }

        public static object MergeObjValues(this string value, Dictionary<string, object> MergeVals)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (var item in MergeVals)
                {
                    try
                    {
                        if (item.Value == null)
                        {
                            value = value.Replace(string.Concat("#", item.Key, "#"), null);
                        }
                        else
                        {
                            value = value.Replace(string.Concat("#", item.Key, "#"), item.Value == null ? null : item.Value.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        value = value.Replace(string.Concat("#", item.Key, "#"), null);
                    }
                }
            }

            return value;
        }

        public static string ToTitleCaseString(this string s)
        {
            return new string(s.CharsToTitleCase().ToArray());
        }

        public static IEnumerable<char> CharsToTitleCase(this string s)
        {
            bool newWord = true;
            foreach (char c in s)
            {
                if (newWord) { yield return Char.ToUpper(c); newWord = false; }
                else yield return Char.ToLower(c);
                if (c == ' ') newWord = true;
            }
        }

        public static string ToTitleCase(this string s)
        {
            string txt = null;

            try
            {
                //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                //txt = textInfo.ToTitleCase(s);

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
                txt = textInfo.ToTitleCase(s);
            }
            catch (Exception)
            {

            }

            return txt;
        }

        public static string ToStringItems<T>(this IEnumerable<T> items, string separator = ",")
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

        /// <summary>
        /// convert list of values to a comma delimited string in the specified quote character
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="items">values seperated by comma or semi colon</param>
        /// <param name="quote">the type of quote</param>
        /// <returns></returns>
        public static string ToQuotedDelimitedValues<T>(this IEnumerable<T> items, string quote = "'")
        {
            string xList = null;
            try
            {
                if (items != null)
                {
                    if (items != null)
                    {
                        foreach (T x in items)
                        {
                            xList = xList + string.Format("{0}{1}{0},", quote, x.ToString());
                        }
                        if (!string.IsNullOrEmpty(xList))
                        {
                            xList = xList.TrimEnd(',');
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return xList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strList">the list of items separated by comma</param>
        /// <param name="delimiter">the seperator character</param>
        /// <param name="quote">the quote character</param>
        /// <returns></returns>
        public static string ToQuotedDelimitedString(this string strList, char[] delimiter = null, string quote = "'")
        {
            string xList = null;

            strList = strList.Replace("\r\n", null);

            try
            {
                if (strList != null)
                {
                    if (!string.IsNullOrEmpty(strList))
                    {
                        if (delimiter == null)
                        {
                            delimiter = new char[] { ',', ';', '\r', '\n' };
                        }

                        var items = strList.Split(delimiter);
                        foreach (string x in items)
                        {
                            xList = xList + string.Format("{0}{1}{0},", quote, x.ToString());
                        }
                        if (!string.IsNullOrEmpty(xList))
                        {
                            xList = xList.TrimEnd(delimiter);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return xList;
        }


        public static List<string> ToStringList(this string strList, char[] delimiter = null)
        {
            List<string> xList = new List<string>();

            strList = strList.Replace("\r\n", null);

            try
            {
                if (strList != null)
                {
                    if (!string.IsNullOrEmpty(strList))
                    {
                        if (delimiter == null)
                        {
                            delimiter = new char[] { ',', ';', '\r', '\n' };
                        }

                        var items = strList.Split(delimiter);
                        foreach (string x in items)
                        {
                            xList.Add(x);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return xList;
        }

        public static string ToWords(this object value, bool IsMoney = false, CultureInfo culture = null)
        {
            String numb = string.Format("{0:N2}", Convert.ToDecimal(value)).Replace(",", null);

            String val = "", wholeNo = numb, points = "", andStr = "";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                }

                if (IsMoney)
                {
                    var str_kobo = Convert.ToInt32(points) > 0 ? "Kobo" : null;

                    val = String.Format("{0} {1} Naira {2} {3}", translateWholeNumber(wholeNo, culture).Trim(),
                        andStr, translateWholeNumber(points), str_kobo);
                }
                else
                {
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = ("point");
                    }
                    val = String.Format("{0} {1} {2}", translateWholeNumber(wholeNo, culture).Trim(),
                        andStr, translateWholeNumber(points));
                }

            }
            catch (Exception)
            {

            }

            return val;
        }

        public static string ToWords(this object value, string currency = null, string sub_currency = null, CultureInfo culture = null)
        {
            String numb = string.Format("{0:N2}", Convert.ToDecimal(value)).Replace(",", null);

            String val = "", wholeNo = numb, points = "", andStr = "";
            try
            {
                int decimalPlace = numb.IndexOf(".");

                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                }

                if (!string.IsNullOrEmpty(currency))
                {
                    if (string.IsNullOrEmpty(sub_currency))
                    {
                        val = String.Format("{0} {1} {2} {3} {4}", translateWholeNumber(wholeNo).Trim(),
                                                andStr, currency, translateWholeNumber(points));
                    }
                    else
                    {
                        var str_sub_curr = Convert.ToInt32(points) > 0 ? sub_currency : null;

                        val = String.Format("{0} {1} {2} {3} {4}", translateWholeNumber(wholeNo).Trim(),
                            andStr, currency, translateWholeNumber(points), str_sub_curr);
                    }
                }
                else
                {
                    var str_kobo = Convert.ToInt32(points) > 0 ? "kobo" : null;

                    val = String.Format("{0} {1} Naira {2} {3}", translateWholeNumber(wholeNo, culture).Trim(),
                        andStr, translateWholeNumber(points), str_kobo);
                }

            }
            catch (Exception)
            {

            }

            return val;
        }

        private static String translateWholeNumber(String number, CultureInfo culture = null)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))

                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");
                    int numDigits = number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;

                            if (!beginsZero)
                            {
                                place = " Hundred ";
                            }
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        case 11://Billions's range
                            pos = (numDigits % 13) + 1;
                            place = " Trillion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }

                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));
                        //check for trailing zeros

                        if (culture != null)
                        {
                            switch (culture.Name)
                            {
                                case "en-US":
                                    {
                                        word = " and " + word.Trim();
                                        break;
                                    }
                                case "en-UK":
                                    {
                                        word = word.Trim();
                                        break;
                                    }
                                default:
                                    {
                                        word = word.Trim();
                                        break;
                                    }
                            }

                        }
                        else
                        {
                            word = word.Trim();
                        }
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch
            {
                ;
            }
            return word.Trim();
        }

        private static String tens(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Forty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static String ones(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        public static string USPhoneNumberFormat(this string value)
        {
            if (value.Length == 10)
            {
                StringBuilder phoneNumber = new StringBuilder();

                for (int i = 0; i < 10; i++)
                {
                    if (i == 0)
                    {
                        phoneNumber.Append("(");
                        phoneNumber.Append(value[i]);
                    }
                    else if (i == 3)
                    {
                        phoneNumber.Append(")-(");
                        phoneNumber.Append(value[i]);
                    }
                    else if (i == 7)
                    {
                        phoneNumber.Append(")-");
                        phoneNumber.Append(value[i]);
                    }
                    else
                    {
                        phoneNumber.Append(value[i]);
                    }
                }
                return phoneNumber.ToString();
            }
            return value;
        }
    }
}
