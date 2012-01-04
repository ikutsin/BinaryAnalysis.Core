using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Browsing.Windowless.Proxies
{
    public class HttpBrowsingResponse : IBrowsingResponse
    {
        public HttpBrowsingResponse(HttpWebResponse response, IBrowsingSession info, TimeSpan generationTime)
        {
            this.info = info;
            this.response = response;
            this.generationTime = generationTime;
        }
        IBrowsingSession info;
        HttpWebResponse response;
        MemoryStream responseStream;

        public HttpStatusCode StatusCode
        {
            get { return response.StatusCode; }
        }

        string responseContent = null;
        public string ResponseContent
        {
            get
            {
                if (responseContent == null)
                {
                    StreamReader reader = null;
                    if(info.ResponseEncoding==Encoding.Default)
                    {
                        reader = new StreamReader(ResponseStream);
                    }
                    else
                    {
                        reader = new StreamReader(ResponseStream, info.ResponseEncoding);
                    }
                    responseContent = reader.ReadToEnd();
                }
                return responseContent;
            }
        }

        public Stream ResponseStream
        {
            get
            {
                if (responseStream == null)
                {
                    Stream stream = response.GetResponseStream();
                    responseStream = new MemoryStream((int)response.ContentLength+1);

                    Byte[] buffer = new Byte[4096];
                    var startPoint = 0;

                    var endLoad = false;

                    do
                    {
                        int bytesSize = stream.Read(buffer, 0, buffer.Length);
                        if (bytesSize>0)
                        {
                            responseStream.Write(buffer, 0, bytesSize);
                            startPoint += bytesSize;
                        }
                        else
                        {
                            endLoad = true;
                        }
                    } while (!endLoad);
                    stream.Close();
                }
                responseStream.Position = 0;
                return responseStream;
            }
        }

        protected void DisposeResponse()
        {
            response.Close();
        }

        public void Dispose()
        {
            if (responseStream != null) {
                responseStream.Dispose();
            }
            DisposeResponse();
        }

        NameValueCollection _headers;
        private TimeSpan generationTime;
        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null)
                {
                     _headers = new NameValueCollection();
                     foreach (string hKey in response.Headers.Keys)
                     {
                         _headers.Add(hKey, response.Headers[hKey]);
                     }
                }
                return _headers;
            }
        }
        public Uri ResponseUrl
        {
            get { return response.ResponseUri; }
        }

        public TimeSpan GenerationTime
        {
            get { return generationTime; }
        }
    }
}
