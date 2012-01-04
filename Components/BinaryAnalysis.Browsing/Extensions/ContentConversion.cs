using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using BinaryAnalysis.Browsing.Windowless;
using System.IO;
using System.Xml.Linq;
using Sgml;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace BinaryAnalysis.Browsing.Extensions
{
    public static class ContentConversion
    {
        /// <summary>
        /// http://james.newtonking.com/projects/json/help/
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static JObject AsJSON(this IBrowsingResponse response)
        {
            return JObject.Parse(response.ResponseContent);
        }
        public static XDocument AsXML(this IBrowsingResponse response)
        {
            return XDocument.Parse(response.ResponseContent);
        }
        /// <summary>
        /// http://htmlagilitypack.codeplex.com/wikipage?title=Examples
        /// http://www.w3schools.com/xpath/xpath_syntax.asp
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static HtmlDocument AsHtmlDocument(this IBrowsingResponse response)
        {
            return AsHtmlDocument(response.ResponseContent);
        }        
        
        public static HtmlDocument AsHtmlDocument(this string response)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            return doc;
        }



        public static string InnerText(this XElement el)
        {
            StringBuilder str = new StringBuilder();
            foreach (XNode element in el.DescendantNodes().Where(x=>x.NodeType==XmlNodeType.Text))
            {
                str.Append(element.ToString());
            }
            return str.ToString();
        }
        public static string ValueText(this XElement el)
        {
            StringBuilder str = new StringBuilder();
            foreach (XNode element in el.Nodes().Where(x => 
                x.NodeType == XmlNodeType.Comment 
                || x.NodeType == XmlNodeType.Text 
                || x.NodeType == XmlNodeType.CDATA))
            {
                str.Append(element.ToString());
            }
            return str.ToString();            
        }

        public static XDocument AsFixedXML(this IBrowsingResponse response)
        {
            try
            {
                return AsFixedXML(response.ResponseContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var doc = AsFixedXML(HtmlDocument.HtmlEncode(response.ResponseContent));
                return doc;
            }
        }

        static Regex xmlnsRegex = new Regex(@"(xmlns:?[^=]*=[""][^""]*[""])", RegexOptions.Compiled);
        public static XDocument AsFixedXML(this string responseContent)
        {
            if(String.IsNullOrWhiteSpace(responseContent)) return new XDocument();

            string data = xmlnsRegex.Replace(responseContent, "");
            StringReader sr = new System.IO.StringReader(data);
            SgmlReader reader = new SgmlReader();
            reader.DocType = "html";
            reader.CaseFolding = CaseFolding.ToLower;
            reader.InputStream = sr;
            var doc = XDocument.Load(reader);
            return doc;
        }

        public static void SaveToFile(this IBrowsingResponse response, string filename) 
        {
            var file = File.Open(filename, FileMode.Create);
            var responseStream = response.ResponseStream;
            Byte[] buffer = new Byte[4096];
            var startPoint = 0;
            var endLoad = false;

            do
            {
                int bytesSize = responseStream.Read(buffer, 0, buffer.Length);
                if (bytesSize>0)
                {
                    file.Write(buffer, 0, bytesSize);
                    startPoint += bytesSize;
                }
                else
                {
                    endLoad = true;
                }
            } while (!endLoad);
            file.Close();
        }
    }
}
