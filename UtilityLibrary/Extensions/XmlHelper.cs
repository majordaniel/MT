using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UtilityLibrary.Extensions
{
    public static class XmlHelper
    {
        public static T DeserializeFromXml<T>(string xml)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        public static string Serialize<T>(T data)
        {
            try
            {
                using (StringWriter stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringwriter, data);
                    return stringwriter.ToString();
                }
            }
            catch
            {
                throw;
            }
        }


        /*
         * To do:  Devlop a method that will the user specify the namespaces
         */
        public static string Serialize<T>(T data, bool indent = false, bool OmitXmlDeclaration = true, bool include_namespace = false) //, bool exclude_xsi = false, bool exclude_xsd = false)
        {

            string xmlString;

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = indent;
            settings.OmitXmlDeclaration = OmitXmlDeclaration;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            if (!include_namespace)
            {
                // exclude xsi and xsd namespaces by adding the following:
                ns.Add(string.Empty, string.Empty);
            }

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, data, ns);
                }
                xmlString = textWriter.ToString(); //This is the output as a string
            }

            return xmlString;

        }




        public static T Deserialize<T>(string xmlStr)
        {
            try
            {
                using (StringReader stringReader = new System.IO.StringReader(xmlStr))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var res = serializer.Deserialize(stringReader);
                    return (T)res;
                }
            }
            catch
            {
                throw;
            }
        }

        public static XmlDocument CreateXmlDoc(string xmlStr)
        {
            XmlDocument xml = new XmlDocument();
            try
            {

                xml.LoadXml(xmlStr);
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex);
            }

            return xml;
        }


        public static XmlNodeList SearchXml(string xmlStr, string filter)
        {
            XmlDocument xml = new XmlDocument();
            XmlNodeList xnList;
            try
            {
                xml.LoadXml(xmlStr);
                xnList = xml.SelectNodes(filter);
            }
            catch (Exception ex)
            {
                xnList = null;
                //Logger.LogError(ex);
            }

            return xnList;
        }


    }
}
