using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Data.Metrics
{
    public class MetricsHolderRepository<T> : ClassifiableRepository<T> where T : Entity, IMetricsHolder
    {
        protected MetricsService metricsService;
        public MetricsHolderRepository(IDbContext context, 
            MetricsService metricsService,
            ILog log, RelationService relRepo)
            : base(context, log, relRepo)
        {
            this.metricsService = metricsService;
        }

        public override void Delete(T entity)
        {
            metricsService.DeleteFor(entity);
            base.Delete(entity);
        }

        public void LoadMetricsIfNull(T entity)
        {
            if (entity != null && entity.Metrics == null) entity.Metrics = metricsService.GetFor(entity);
        }
        public void LoadMetricsIfNull(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                LoadMetricsIfNull(entity);
            }
        }

        public override IList<T> GetAll()
        {
            var result = base.GetAll();
            if (ComponentsLoadLevel == ComponentsLoadLevel.Always)
            {
                foreach (T entity in result)
                {
                    entity.Metrics = metricsService.GetFor(entity);
                }
            }
            return result;
        }

        public override T Get(int id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            var entity = base.Get(id, lockMode);
            if (entity != null && ComponentsLoadLevel >= ComponentsLoadLevel.Single)
            {
                entity.Metrics = metricsService.GetFor(entity);
            }
            return entity;
        }
        public override T Load(int id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            var entity = base.Load(id, lockMode);
            if (entity != null && ComponentsLoadLevel>=ComponentsLoadLevel.Single)
            {
                entity.Metrics = metricsService.GetFor(entity);
            }
            return entity;
        }
        public override void Evict(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                wu.Session.Evict(entity);
                if (entity.Metrics != null)
                {
                    wu.Session.Evict(entity.Metrics);
                }
            }
        }

        public override T SaveOrUpdate(T entity)
        {
            var e = base.SaveOrUpdate(entity);
            metricsService.UpdateMetrics(entity.Metrics);
            return e;
        }
        public override T Save(T entity)
        {
            base.Save(entity);
            metricsService.UpdateMetrics(entity.Metrics);
            return entity;
        }
        public override T Update(T entity)
        {
            metricsService.UpdateMetrics(entity.Metrics);
            base.Update(entity);
            return entity;
        }

        public override object ExecuteDetachedExpression(System.Linq.Expressions.Expression expression, System.Type elementType, System.Type ienumerableExpressionType)
        {
            var result = base.ExecuteDetachedExpression(expression, elementType, ienumerableExpressionType);
            if (result is IEnumerable<T>)
            {
                if (ComponentsLoadLevel == ComponentsLoadLevel.Always)
                {
                    foreach (T entity in result as IEnumerable<T>)
                    {
                        entity.Metrics = metricsService.GetFor(entity);
                    }
                }
            }
            else if (result is T)
            {
                if (ComponentsLoadLevel >= Core.ComponentsLoadLevel.Single)
                {
                    metricsService.GetFor(result as T);
                }
            }
            return result;
        }
    }
}
