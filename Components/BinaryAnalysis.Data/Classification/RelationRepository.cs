using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Mixin;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace BinaryAnalysis.Data.Classification
{
    //XXX: Performance change to IQueriable to Criterias
    public class RelationRepository : Repository<RelationEntity>
    {
        protected IDbContext context;

        public RelationRepository(IDbContext context, ILog log) : base(context, log)
        {
            this.context = context;
        }

        public RelationEntity GetRelation(IClassifiable relatable, IClassifiable related, TaxonEntity type = null) 
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return AsQueryable(wu.Session)
                    .Where(x => x.RelatedObjectName == related.ObjectName && x.RelatedObjectID == related.Id)
                    .Where(x => x.ObjectName == relatable.ObjectName && x.ObjectID == relatable.Id)
                    .Where(x => x.Type == type)
                    .FirstOrDefault();
            }
        }

        private ICriteria CreateGetRelatedCriteria(DbWorkUnit wu, IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            var criteria = wu.Session.CreateCriteria<RelationEntity>()
                .Add(Restrictions.Eq("ObjectName", classifiable.ObjectName))
                .Add(Restrictions.Eq("ObjectID", classifiable.Id));
            if(type!=null)criteria.Add(Restrictions.Eq("Type", type));
            if (direction != RelationDirection.Undefined) criteria.Add(Restrictions.Eq("Direction", (int)direction));
            return criteria;
        }
        private ICriteria CreateGetByRelatedCriteria(DbWorkUnit wu, IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            var criteria = wu.Session.CreateCriteria<RelationEntity>()
                .Add(Restrictions.Eq("RelatedObjectName", classifiable.ObjectName))
                .Add(Restrictions.Eq("RelatedObjectID", classifiable.Id));

            if (type != null) criteria.Add(Restrictions.Eq("Type", type));
            if (direction != RelationDirection.Undefined) criteria.Add(Restrictions.Eq("Direction", (int)direction));
            return criteria;
        }

        public IList<RelatedIdsResult> GetRelatedIds(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            if (classifiable == null)
            {
                log.Warn("GetRelatedIds null request");
                return null;
            }
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var criteria = CreateGetRelatedCriteria(wu, classifiable, direction, type)
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("RelatedObjectID"), "Id")
                        .Add(Projections.Property("RelatedObjectName"), "Name"))
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(RelatedIdsResult)));
                return criteria.List<RelatedIdsResult>();
            }
        }

        public IList<RelatedIdsResult> GetByRelatedIds(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            if (classifiable == null)
            {
                log.Warn("GetByRelatedIds null request");
                return null;
            }
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var criteria = CreateGetByRelatedCriteria(wu, classifiable, direction, type)
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("ObjectID"), "Id")
                        .Add(Projections.Property("ObjectName"), "Name"))
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(RelatedIdsResult)));
                return criteria.List<RelatedIdsResult>();
            }
        }

        public IList<RelationEntity> GetRelated(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            if (classifiable == null)
            {
                log.Warn("GetRelated null request");
                return null;
            }
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var criteria = CreateGetRelatedCriteria(wu, classifiable, direction, type);
                return criteria.List<RelationEntity>();
            }
        }

        public IList<RelationEntity> GetByRelated(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null)
        {
            if (classifiable == null)
            {
                log.Warn("GetByRelated null request");
                return null;
            }
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var criteria = CreateGetByRelatedCriteria(wu, classifiable, direction, type);
                return criteria.List<RelationEntity>();
            }
        }
        public IList<RelationEntity> GetAllRelationsFor(IClassifiable classifiable)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var ret = this.AsQueryable(wu.Session)
                    .Where(x => (x.ObjectName == classifiable.ObjectName && x.ObjectID == classifiable.Id) ||
                        (x.RelatedObjectName == classifiable.ObjectName && x.RelatedObjectID == classifiable.Id));
                return ret.ToList();
            }
        }
        public IList<RelationEntity> GetRelationsByType(TaxonomyNode node)
        {
            return GetRelationsByType(node.entity);
        }
        public IList<RelationEntity> GetRelationsByType(TaxonEntity taxonEntity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {

                var ret = this.AsQueryable(wu.Session)
                    .Where(x => x.Type == taxonEntity);
                return ret.ToList();
            }
        }

        public IList<T> GetRelated<T>(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null) where T : Entity, IClassifiable
        {
            List<T> ret = new List<T>();
            using (var repo = new Repository<T>(context, log))
            {
                var idList = GetRelated(classifiable, direction, type).Select(y => y.RelatedObjectID).ToList();
                if (idList.Count > 0)
                {
                    using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
                    {
                        ret = repo.AsQueryable(wu.Session).Where(x => idList.Contains(x.Id)).ToList();
                    }
                }
                ret.ForEach(x => repo.Evict(x));
            }
            
            return ret;
        }
        public IList<T> GetByRelated<T>(IClassifiable classifiable, RelationDirection direction = RelationDirection.Undefined, TaxonEntity type = null) where T : Entity, IClassifiable
        {
            List<T> ret = new List<T>();
            using (var repo = new Repository<T>(context, log))
            {
                var idList = GetByRelated(classifiable, direction, type).Select(y => y.ObjectID).ToList();
                if (idList.Count() > 0)
                {
                    using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
                    {
                        ret = repo.AsQueryable(wu.Session).Where(x => idList.Contains(x.Id)).ToList();
                    }
                }
                ret.ForEach(x => repo.Evict(x));
            }
            return ret;
        }
    }
}
