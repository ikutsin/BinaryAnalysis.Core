/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 17.02.2011
 * Time: 20:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using BinaryAnalysis.Helpers.Network;

namespace BinaryAnalysis.Browsing.Windowless.Proxies
{
    /// <summary>
    /// Description of DirectBrowsing.
    /// </summary>
    public class DirectBrowsing : IBrowsingProxy
    {
        protected virtual HttpWebRequest CreateRequest(Uri httpUrl, IBrowsingSession info)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(httpUrl);

            req.Timeout = (int)info.Timeout;

            req.Credentials = System.Net.CredentialCache.DefaultCredentials;
            req.CookieContainer = info.CookieContainer;
            foreach(var hKey in info.Headers.AllKeys) {
                switch (hKey)
                {
                    //case "Date": break;
                    //case "Host": break;
                    //case "Range": break;
                    //case "Proxy-Connection": break;
                    //case "Version": break;
                    //case "Content-Length": req.ContentLength = info.Headers[hKey]; break;
                    //case "If-Modified-Since": req.IfModifiedSince = info.Headers[hKey]; break;
                    case "Accept": req.Accept = info.Headers[hKey]; break;
                    case "Connection": req.Connection = info.Headers[hKey]; break;
                    case "Content-Type": req.ContentType = info.Headers[hKey]; break;
                    case "Expect": req.Expect = info.Headers[hKey]; break;
                    case "Referer": req.Referer = info.Headers[hKey]; ; break;
                    case "Transfer-Encoding": req.TransferEncoding = info.Headers[hKey]; break;
                    case "User-Agent": req.UserAgent = info.Headers[hKey]; break;
                    default:
                        req.Headers.Add(hKey, info.Headers[hKey]);
                        break;
                }
                
            }
            return req;
        }
        protected HttpWebResponse GetResponseFromRequest(HttpWebRequest req, int attempts = 0)
        {
            HttpWebResponse resp = null;
            do
            {
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException ex)
                {
                    if (attempts == 0) throw new WebException("No attempts left", ex);
                }
                attempts--;
            } while (resp == null);
            return resp;
        }
        protected IBrowsingResponse PrepareBrowsingResponse(HttpWebRequest req, IBrowsingSession info)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                var response = GetResponseFromRequest(req);
                stopWatch.Stop();
                return new HttpBrowsingResponse(response, info, stopWatch.Elapsed);
            }
            catch (WebException ex)
            {
                stopWatch.Stop();
                //if (ex.Status == WebExceptionStatus.ProtocolError)
                if (ex.Response != null)
                {
                    return new HttpBrowsingResponse((HttpWebResponse)ex.Response, info, stopWatch.Elapsed);
                }
                else
                {
                    return new ErrorBrowsingResponse()
                    {
                        GenerationTime = stopWatch.Elapsed,
                        Headers = info.Headers,
                        ResponseContent = ex.Message,
                        ResponseUrl = req.RequestUri,
                        StatusCode = HttpStatusCode.ServiceUnavailable
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PrepareBrowsingResponse failed", ex);
            }
        }

        #region IBrowsingProxy        
        public virtual IBrowsingResponse GetResponse(Uri httpUri, IBrowsingSession info)
        {
            var req = CreateRequest(httpUri, info);
            req.Method = "GET";
            return PrepareBrowsingResponse(req, info);
        }

        public virtual IBrowsingResponse PostResponse(Uri httpUri, System.Collections.Specialized.NameValueCollection postParamz, IBrowsingSession info)
        {
            var req = CreateRequest(httpUri, info);
            req.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = "";
            if (postParamz.Count > 0)
            {
                postData = string.Join("&", Array.ConvertAll(postParamz.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(postParamz[key]))));
            }
            byte[] data = encoding.GetBytes(postData);

            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream newStream = req.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            return PrepareBrowsingResponse(req, info);
        }

        public virtual IBrowsingResponse FilePostResponse(Uri httpUri, System.Collections.Specialized.NameValueCollection postParamz, List<Tuple<string, string, System.IO.Stream>> files, IBrowsingSession info)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest req = CreateRequest(httpUri, info);
            req.Timeout = req.Timeout*(files.Count + 1);
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Method = "POST";
            req.KeepAlive = true;
            Stream memStream = new System.IO.MemoryStream();

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            //--- params
            foreach (string key in postParamz.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, postParamz[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }
            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            // -- files
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";
            foreach (var fileData in files)
            {
                //string header = string.Format(headerTemplate, "file" + i, fileData.Key);
                string header = string.Format(headerTemplate, fileData.Item1, fileData.Item2);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileData.Item3.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
            }


            req.ContentLength = memStream.Length;
            Stream requestStream = req.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            return PrepareBrowsingResponse(req, info);
        }
        #endregion


        public virtual string GetCurrentIp(Uri httpUrl, IBrowsingSession info)
        {
             return NetworkCommons.GetAllUnicastAddresses()[0].ToString();
        }
    }
}
