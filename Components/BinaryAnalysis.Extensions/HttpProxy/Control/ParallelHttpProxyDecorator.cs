using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Data;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using log4net;

namespace BinaryAnalysis.Extensions.HttpProxy.Control
{
    public class ParallelHttpProxyDecorator : HttpProxyDecoratorBase
    {
        private IBrowsingProxy proxySwitcher;

        public ParallelHttpProxyDecorator(HttpProxyRepository repo, TaxonomyTree tree, StateService stateService, ILog log) : base(repo, tree, stateService, log)
        {
            proxySwitcher = new ParallelHttpProxyBrowsing(this);
        }

        internal Dictionary<int, Uri> ThreadProxies = new Dictionary<int, Uri>();
        
        protected override Uri GetNextProxyFor(IBrowsingSession session, string descriminator)
        {
            ThreadProxies[Thread.CurrentThread.ManagedThreadId] = base.GetNextProxyFor(session, descriminator);
            return ThreadProxies[Thread.CurrentThread.ManagedThreadId];
        }
        internal Uri GetCurrentProxyFor(IBrowsingSession session, string descriminator)
        {
            if(!ThreadProxies.Keys.Contains(Thread.CurrentThread.ManagedThreadId))
            {
                if (session == null || descriminator == null) return null;
                 ThreadProxies[Thread.CurrentThread.ManagedThreadId] = base.GetNextProxyFor(session,descriminator);
            }
            return ThreadProxies[Thread.CurrentThread.ManagedThreadId];
        }

        public override bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            if (String.IsNullOrEmpty(Descriminator)) Descriminator = uri.Host;
            if (session.CurrentProxy == null) session.CurrentProxy = proxySwitcher;
            return base.OnBeforeRequestStop(session, uri);
        }

        public override bool OnSwitchProxy(IBrowsingSession browsingSession, Uri uri)
        {
            if (String.IsNullOrEmpty(Descriminator)) Descriminator = uri.Host;
            ThreadProxies[Thread.CurrentThread.ManagedThreadId] = base.GetNextProxyFor(browsingSession, Descriminator);
            return true;
        }
    }
}
