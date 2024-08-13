using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityLibrary.Extensions
{
    public class JsonExtension
    {
        public static string ToJson(object objectToJson)
        {
            var result = JsonConvert.SerializeObject(objectToJson, Formatting.Indented, new JsonSerializerSettings());
            return result;
        }

        public static string XmlToJson(string xmlString)
        {
            var validateJson = IsValidJson(xmlString);
            if (validateJson) return xmlString;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xmlString);
            var con = doc.LastChild;
            var jsonString = JsonConvert.SerializeXmlNode(con);
            return jsonString;
        }

        public static bool IsValidJson(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }


    }
}
