using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using BinaryAnalysis.Browsing.Windowless;
using Fiddler;

namespace BA.Examples.ScriptingHelper.Logic
{
    public class FiddlerSessionBrowsingResponse : IBrowsingResponse
    {
        private Fiddler.Session session;
        public Fiddler.Session FiddlerSession { get { return session; } }

        public FiddlerSessionBrowsingResponse(Fiddler.Session session)
        {
            this.session = session;
        }
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode)session.oResponse.headers.HTTPResponseCode; }
        }

        public string ResponseContent
        {
            get { return session.GetResponseBodyAsString(); }
        }

        public Stream ResponseStream
        {
            get { 
                var responseStream = new MemoryStream(session.responseBodyBytes);
                return responseStream;
            }
        }

        public NameValueCollection Headers
        {
            get
            {
                var ret = new NameValueCollection();
                foreach (HTTPHeaderItem header in session.oResponse.headers)
                {
                    ret.Add(header.Name, header.Value);
                }
                return ret;
            }
        }

        public Uri ResponseUrl
        {
            get { return new Uri(session.fullUrl); }
        }

        public TimeSpan GenerationTime
        {
            get { return TimeSpan.Zero; }
        }

        public void Dispose()
        {
            
        }
    }
}
