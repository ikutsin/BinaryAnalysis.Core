/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 17.02.2011
 * Time: 18:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Sgml;
using System.Xml;


namespace BinaryAnalysis.Browsing
{
    public class SimpleBrowser
    {
        public CookieContainer cookieContainer;
        public string USER_AGENT = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:Yo!) Gecko/20090729 Firefox/3.5";

        static SimpleBrowser()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate(object sender,
                        System.Security.Cryptography.X509Certificates.X509Certificate pCertificate,
                        System.Security.Cryptography.X509Certificates.X509Chain pChain,
                        System.Net.Security.SslPolicyErrors pSSLPolicyErrors)
                {
                    //Console.WriteLine(sender);
                    //Console.WriteLine(pCertificate);
                    //Console.WriteLine(pChain);
                    //Console.WriteLine(pSSLPolicyErrors);
                    return true;
                };
        }

        public SimpleBrowser()
        {
            cookieContainer = new CookieContainer();
        }
        public virtual HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.CookieContainer = cookieContainer;
            req.UserAgent = USER_AGENT;
            req.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            req.KeepAlive = false;
            return req;
        }
        public HttpWebResponse GetResponseFromRequest(HttpWebRequest req, int attempts) 
        {
            HttpWebResponse resp = null;
            do
            {
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Attempt:"+attempts);
                    Debug.WriteLine(ex);
                    Thread.Sleep(1000);
                    if (attempts == 0) throw new Exception("No attempts left", ex);
                }
                attempts--;
            } while (resp == null);
            return resp;
        }
        public String GetContentFromResponse(HttpWebResponse resp)
        {
            return GetContentFromResponse(resp, Encoding.Default);
        }
        public String GetContentFromResponse(HttpWebResponse resp, Encoding encoding)
        {
            Stream stream = resp.GetResponseStream();
            string contents;
            StreamReader reader = new StreamReader(stream, encoding);
            contents = reader.ReadToEnd();
            reader.Close();
            return contents;
        }
        public void AddCertificateToRequest(HttpWebRequest req, String path, String pwd)
        {
            /*
            PfxOpen pfx = new PfxOpen();
            //pfx.LoadPfx(@"D:\cert.pfx", ref pswd);
            string passwd = pwd.ToString();
            pfx.LoadPfx(path, ref passwd);
            req.ClientCertificates.Add(pfx.cert);
             */
            req.ClientCertificates.Add(
                new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    path, pwd));
        }
        public HttpWebRequest CreatePostRequest(string url, Dictionary<String, String> paramz)
        {
            HttpWebRequest req = CreateRequest(url);
            req.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = "";
            if (paramz.Count > 0)
            {
                foreach (KeyValuePair<String, String> kvp in paramz)
                {
                    postData += kvp.Key + "=" + System.Web.HttpUtility.UrlEncode(kvp.Value) + "&";
                }
                postData = postData.Substring(0, postData.Length - 1);
            }
            byte[] data = encoding.GetBytes(postData);

            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream newStream = req.GetRequestStream();

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            return req;
        }
        public Stream GetStreamFromUrl(string targetUrl)
        {
            HttpWebRequest req = CreateRequest(targetUrl);
            return GetResponseFromRequest(req, 1).GetResponseStream();            
        }
        public void SaveUrlToFile(string targetUrl, string filename)
        {
            var file = File.Open(filename, FileMode.Create);

            HttpWebRequest req = CreateRequest(targetUrl);
            var response = GetResponseFromRequest(req, 1);
            var responseStream = response.GetResponseStream();
            var fileSize = response.ContentLength;

            Byte[] buffer = new Byte[4096];
            var startPoint = 0;

            var endLoad = false;

            do
            {
                int bytesSize = responseStream.Read(buffer, 0, buffer.Length);
                if (bytesSize + startPoint < fileSize)
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
            response.Close();
        }
        public String GetStringFromUrl(string targetUrl)
        {
            HttpWebRequest req = CreateRequest(targetUrl);
            return GetContentFromResponse(GetResponseFromRequest(req, 1));
        }
        public SgmlReader GetSgmlFromUrl(string targetUrl) 
        {
            HttpWebRequest req = CreateRequest(targetUrl);
            string data = GetContentFromResponse(GetResponseFromRequest(req, 1));
            StringReader sr = new System.IO.StringReader(data);
            SgmlReader reader = new SgmlReader();
            reader.DocType = "html";
            reader.InputStream = sr;
            return reader;
        }
        public XmlDocument GetXMLFromUrl(string targetUrl)
        {
            return GetXMLFromUrl(targetUrl, Encoding.Default);
        }
        public XmlDocument GetXMLFromUrl(string targetUrl, Encoding enc)
        {
            HttpWebRequest req = CreateRequest(targetUrl);
            string xmlString = GetContentFromResponse(GetResponseFromRequest(req, 1), enc);
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            return xml;
        }

        public void Describe(HttpWebResponse resp)
        {
            Console.WriteLine("resp.Method=" + resp.Method);
            Console.WriteLine("resp.StatusCode=" + resp.StatusCode);
            Console.WriteLine("resp.StatusDescription=" + resp.StatusDescription);
            //Console.WriteLine("*Headers ({0}):", resp.Header.Count);
            foreach (String key in resp.Headers.AllKeys)
            {
                Console.WriteLine(key + "=" + resp.Headers[key]);
            }
            Console.WriteLine("*Cookies ({0}):", resp.Cookies.Count);
            foreach (Cookie key in resp.Cookies)
            {
                Console.WriteLine(key);
            }
            Console.WriteLine("*CookiesCtn ({0}):", cookieContainer.Count);
        }
        public void Describe(HttpWebRequest resp)
        {
            Console.WriteLine("*Cookies ({0}):", resp.CookieContainer == null ? "null" : "" + resp.CookieContainer.Count);
            Console.WriteLine("req.Method=" + resp.Method);
            Console.WriteLine("*Headers ({0}):", resp.Headers.Count);
            foreach (String key in resp.Headers.AllKeys)
            {
                Console.WriteLine(key + "=" + resp.Headers[key]);
            }
        }
        public string ClearContent(string content)
        {
            content = content.Replace("&nbsp;", " ");
            content = content.Replace("\r\n", "");
            content = content.Replace("\t", "");
            return content;
        }

    }
}
