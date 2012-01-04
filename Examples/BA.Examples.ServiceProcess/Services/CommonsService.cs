using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Autofac;
using BA.Examples.ServiceProcess.Services.Core;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Classification;
using log4net;

namespace BA.Examples.ServiceProcess.Services
{
    class CommonsServiceEventsPublisher : PublishService<ICommonsServiceEvents>, ICommonsServiceEvents
    {
        public void OnTick(DateTime serverTime)
        {
            FireEvent(serverTime);
        }
        public void OnBroadcastText(string text)
        {
            FireEvent(text);
        }
    }

    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeRegistry))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CommonsService : SubscriptionManager<ICommonsServiceEvents>, ICommonsService, IDisposable 
    {
        CommonsServiceEventsPublisher thisPublisher = new CommonsServiceEventsPublisher();

        private ILog log;
        private readonly TaxonomyTree _tree;
        private readonly RelationRepository _relRepo;
        private IComponentContext context;

        public CommonsService(ILog log, IComponentContext context,
            TaxonomyTree tree, RelationRepository relRepo)
        {
            this.context = context;
            this.log = log;
            _tree = tree;
            _relRepo = relRepo;

            SetupTimer();
        }



        #region Taxonomy
        public TaxonomyNodeBoxMap GetRoot()
        {
            return null; // _tree.FindOne("/").WrapToContract();
        }
        public TaxonomyNodeBoxMap FindOne(string query)
        {
            return null; // _tree.FindOne(query).WrapToContract();
        }

        public IEnumerable<TaxonomyNodeBoxMap> ChildrenOf(TaxonomyNodeBoxMap tax)
        {
            var taxNode = _tree.FindOne(tax.Path);
            if (taxNode == null) return null;
            return WrapToContract(taxNode.Children);
        }

        public IEnumerable<string> GetRelationsByType(TaxonomyNodeBoxMap tax)
        {
            var taxNode = _tree.FindOne(tax.Path);
            if (taxNode == null) return null;
            return _relRepo.GetRelationsByType(taxNode).Select(x => x.ToString());
        }
        public IEnumerable<string> GetClassified(TaxonomyNodeBoxMap tax)
        {
            var taxNode = _tree.FindOne(tax.Path);
            if (taxNode == null) return null;
            return _relRepo.GetAllRelationsFor(taxNode).Select(x => x.ToString());
        }



        IEnumerable<TaxonomyNodeBoxMap> WrapToContract(IEnumerable<TaxonomyNode> nodes)
        {
            return null;// nodes.Select(n => n.WrapToContract());
        }
        
        #endregion

        #region Basic event model
        public void BroadcastText(string text)
        {
            thisPublisher.OnBroadcastText(text);
        }

        private Timer timer;
        private void SetupTimer()
        {
            timer = new Timer((_) =>
            {
                log.Debug("Timer tick");
                thisPublisher.OnTick(DateTime.Now);
            }, null, 1000, 1000);
        } 
        #endregion

        public void Dispose()
        {
            log.Debug("Disposed");
            timer.Dispose();
        }
    }
}
