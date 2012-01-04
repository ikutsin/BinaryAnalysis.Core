using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace BinaryAnalysis.Browsing.Windowless.Proxies
{
    public class ErrorBrowsingResponse : IBrowsingResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public NameValueCollection Headers { get; set; }
        public Uri ResponseUrl { get; set; }
        public TimeSpan GenerationTime { get; set; }

        public System.IO.Stream ResponseStream
        {
            get { return null; }
        }

        public void Dispose() { }
    }
}
