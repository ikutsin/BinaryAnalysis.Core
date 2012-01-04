using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BinaryAnalysis.Browsing.Windowless.Proxies
{
    public class HttpProxyBrowsing : DirectBrowsing
    {
        Uri proxyAddr;
        public HttpProxyBrowsing(Uri proxyAddr)
        {
            this.proxyAddr = proxyAddr;
        }

        public override string GetCurrentIp(Uri httpUrl, IBrowsingSession info)
        {
            return proxyAddr.Host;
        }
        protected override HttpWebRequest CreateRequest(Uri httpUrl, IBrowsingSession info)
        {
            var req = base.CreateRequest(httpUrl, info);

            var proxy = new WebProxy(proxyAddr);
            req.Proxy = proxy;

            return req;
        }
    }
}
