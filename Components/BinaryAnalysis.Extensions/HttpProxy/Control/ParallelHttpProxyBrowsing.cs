using System;
using System.Net;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Proxies;

namespace BinaryAnalysis.Extensions.HttpProxy.Control
{
    class ParallelHttpProxyBrowsing : DirectBrowsing
    {
        private ParallelHttpProxyDecorator parallelHttpProxyDecorator;

        public ParallelHttpProxyBrowsing(ParallelHttpProxyDecorator parallelHttpProxyDecorator)
        {
            this.parallelHttpProxyDecorator = parallelHttpProxyDecorator;
        }

        public override string GetCurrentIp(Uri httpUrl, IBrowsingSession info)
        {
            var proxy = parallelHttpProxyDecorator.GetCurrentProxyFor(info, httpUrl.Host);
            return proxy.Host;
        }

        public override IBrowsingResponse GetResponse(Uri httpUri, IBrowsingSession info)
        {
            var result = base.GetResponse(httpUri, info);
            return result;
        }

        protected override HttpWebRequest CreateRequest(Uri httpUrl, IBrowsingSession info)
        {
            var req = base.CreateRequest(httpUrl, info);
            var proxyIp = parallelHttpProxyDecorator.GetCurrentProxyFor(info, httpUrl.Host);

            var proxy = new WebProxy(proxyIp);
            req.Proxy = proxy;

            return req;
        }
    }
}
