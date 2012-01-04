using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Data.Mixin;
using log4net;

namespace BinaryAnalysis.Data
{
    public class MetricsService
    {
        public const string RELATIONS_PATH = @"BinaryAnalysis.Core/MetricsRelation";

        MetricsRepository metricsRepo;
        private readonly MetricsEntryRepository metricsEntryRepo;
        private RelationService relationService;
        TaxonomyNode relationNode;
        ILog log;
        object locker = new object();

        public MetricsService(
            MetricsRepository metricsRepo,
            MetricsEntryRepository metricsEntryRepo,
            RelationService relationService,
            TaxonomyTree taxonomyTree,
            ILog log)
        {
            this.metricsRepo = metricsRepo;
            this.metricsEntryRepo = metricsEntryRepo;
            this.relationService = relationService;
            this.log = log;

            relationNode = taxonomyTree.GetOrCreatePath(RELATIONS_PATH, "MetricsRepository relations");
        }

        public IList<MetricsEntity> GetFor(IClassifiable entity)
        {
            return relationService.GetRelated<MetricsEntity>(entity, RelationDirection.Forward, relationNode);
        }
        public MetricsEntity GetFor(IClassifiable entity, string name)
        {
            var ids = relationService.GetRelatedIds(entity, RelationDirection.Forward, relationNode);
            return metricsRepo.FindByNameAndRelationIds(name, ids.Where(t => t.Name == MetricsEntity.OBJECT_NAME).Select(t=>t.Id).ToArray());
        }
        public MetricsEntity GetOrCreateFor(IClassifiable entity, string name = null, string descr = null)
        {
            lock (locker)
            {
                name = name ?? "default";
                var metrics = GetFor(entity, name);
                if (metrics == null)
                {
                    metrics = CreateMetrics(name, entity, descr);
                }
                return metrics;
            }
        }
        protected int GetIdFor(IClassifiable entity, string name)
        {
            var ids = relationService.GetRelatedIds(entity, RelationDirection.Forward, relationNode);
            var metricsid = metricsRepo.FindIdByNameAndRelationIds(name, ids.Where(t => t.Name == MetricsEntity.OBJECT_NAME).Select(t => t.Id).ToArray());
            return metricsid;
        }

        public decimal GetValueAt(IClassifiable entity, string name, DateTime at)
        {
            var metricsid = GetIdFor(entity, name);
            if (metricsid == 0)
            {
                return 0;
            }
            return metricsEntryRepo.GetValueAt(metricsid, at);
        }
        public bool AddValueIfChanged(IClassifiable entity, string name, decimal value, DateTime at, string descr = null)
        {
            var metricsid = GetIdFor(entity, name);
            if (metricsid == 0)
            {
                var metrics = CreateMetrics(name, entity, descr);
                metrics.AddEntry(value, at);
                metricsRepo.Update(metrics);
                return true;
            }
            var dbVal = metricsEntryRepo.GetValueAt(metricsid, at);
            if (dbVal != value)
            {
                var metrics = GetOrCreateFor(entity, name, descr);
                var me = new MetricsEntryEntity()
                             {
                                 Metrics = metrics,
                                 RecordDate = at,
                                 Value = value
                             };
                metricsEntryRepo.Save(me);
                return true;
            }
            return false;
        }

        public MetricsEntity CreateMetrics(string name, IClassifiable entity, string descr)
        {
            MetricsEntity metrics;
            metrics = new MetricsEntity(name, descr ?? String.Format("for {0}({1})", entity.ObjectName, entity.Id));
            metricsRepo.Save(metrics);
            relationService.AddRelation(entity, metrics, RelationDirection.Forward, relationNode);
            return metrics;
        }

        public void EnsureRelation(IClassifiable entity, MetricsEntity metrics)
        {
            if(!relationService.IsRelated(entity, metrics, relationNode))
            {
                relationService.RemoveRelations(metrics, relationNode);
                relationService.AddRelation(entity, metrics, RelationDirection.Forward, relationNode);
            }
            
        }

        public void DeleteFor(IClassifiable entity)
        {
            lock (locker)
            {
                var dbMetrics = GetFor(entity);
                metricsRepo.DeleteAll(dbMetrics);
            }
        }
        public void UpdateMetrics(MetricsEntity metrics)
        {
            lock (locker)
            {
                if (metrics == null || metrics.IsTransient())
                {
                    throw new Exception(metrics + " is transient or null. Use service to get metrics");
                }
                foreach (var e in metrics.Entries) e.Metrics = metrics;
                metricsRepo.Update(metrics);
            }
        }
        public void UpdateMetrics(IList<MetricsEntity> metrics)
        {
            lock (locker)
            {
                if (metrics == null) throw new Exception("Metrics are null");
                foreach (var metric in metrics)
                {
                    UpdateMetrics(metric);
                }
            }
        }
    }
}
