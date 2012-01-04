using System;
using System.Net;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Proxies;
using BinaryAnalysis.Data;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using log4net;

namespace BinaryAnalysis.Extensions.HttpProxy.Control
{
    public class HttpProxyDecorator : HttpProxyDecoratorBase
    {
        public HttpProxyDecorator(HttpProxyRepository repo, TaxonomyTree tree, StateService stateService, ILog log) : base(repo, tree, stateService, log)
        {
            Attempts = 0;
        }

        public int Attempts { get; set; }
        public override void OnInit(IBrowsingSession session)
        {
            WorkingProxies = _repo.GetWorking();
            if(WorkingProxies.Count==0) throw new Exception("No working proxies found");
        }

        public override bool OnSwitchProxy(IBrowsingSession browsingSession, Uri uri)
        {
            if (String.IsNullOrEmpty(Descriminator)) Descriminator = uri.Host;
            browsingSession.CurrentProxy = new HttpProxyBrowsing(GetNextProxyFor(browsingSession, Descriminator));
            return true;
        }

        private int retries;
        public override bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            if (String.IsNullOrEmpty(Descriminator)) Descriminator = uri.Host;
            if (session.CurrentProxy == null) session.CurrentProxy = new HttpProxyBrowsing(GetNextProxyFor(session, Descriminator));
            retries = 0;
            return base.OnBeforeRequestStop(session, uri);
        }
        public override bool OnAfterRequestRerun(IBrowsingSession session, Uri uri, IBrowsingResponse response)
        {
            if (retries < Attempts&&response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                retries++;
                session.CurrentProxy = new HttpProxyBrowsing(GetNextProxyFor(session, Descriminator));
                return true;
            }
            return base.OnAfterRequestRerun(session, uri, response);
        }
    }
}
