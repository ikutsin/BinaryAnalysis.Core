using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Decorators;
using BinaryAnalysis.Data;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using log4net;

namespace BinaryAnalysis.Extensions.HttpProxy.Control
{
    public class HttpProxyDecoratorBase : EmptyDecorator
    {
        public const string STATE_TRIGGER_PATH = "/BinaryAnalysis.Core/HttpProxyDescriminators";
        const string message = "Trigger to flush '{0}' proxyList.";

        protected TaxonomyNode RootDescriminatorStateTriggers;
        public TimeSpan StoringDuration { get; set; }

        protected readonly HttpProxyRepository _repo;
        protected readonly TaxonomyTree _tree;
        protected readonly StateService _stateService;
        protected readonly ILog _log;

        public string Descriminator { get; set; }

        protected IList<HttpProxyEntity> WorkingProxies;

        public HttpProxyDecoratorBase(
            HttpProxyRepository repo, 
            TaxonomyTree tree,
            StateService stateService, 
            ILog log)
        {
            _repo = repo;
            _tree = tree;
            _stateService = stateService;
            _log = log;

            StoringDuration = TimeSpan.FromHours(3); //default storing duration 3 hours //one day
            RootDescriminatorStateTriggers =
                tree.GetOrCreatePath(STATE_TRIGGER_PATH, "Trigger to flush all Browsing cache");
        }

        public override void OnInit(IBrowsingSession session)
        {
            WorkingProxies = _repo.GetWorking();
            if(WorkingProxies.Count==0) throw new Exception("No working proxies found");
        }

        protected object getNextProxyForLocker = new object();
        protected virtual Uri GetNextProxyFor(IBrowsingSession session, string descriminator)
        {
            //TODO: optimize by caching the state
            lock (getNextProxyForLocker)
            {
                var ignoreIds = _stateService.Get<List<int>>("proxy_" + descriminator);
                if (ignoreIds == null) ignoreIds = new List<int>();
                var proxy = WorkingProxies.FirstOrDefault(x => !ignoreIds.Contains(x.Id));
                if (proxy == null)
                {
                    _log.Warn("Proxy list ended and restared for " + descriminator);
                    proxy = WorkingProxies.First();
                    ignoreIds = new List<int>();
                }
                ignoreIds.Add(proxy.Id);

                var hostTrigger = RootDescriminatorStateTriggers
                    .GetOrAddChild(descriminator, String.Format(message, descriminator));

                _stateService.Put<List<int>>("proxy_" + descriminator,
                    ignoreIds, hostTrigger, (long) StoringDuration.TotalSeconds);

                //_log.Debug(String.Format("Proxy changed for {0} to {1} at {2}",
                //    descriminator, proxy, Thread.CurrentThread.ManagedThreadId));

                session.ClearCookieContainer();
                return new Uri(proxy.IP);
            }
        }
    }
}
