using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using log4net;
using NHibernate;
using NHibernate.Criterion;

namespace BinaryAnalysis.Data.Metrics
{
    public class MetricsRepository : ClassifiableRepository<MetricsEntity>
    {
        private readonly MetricsEntryRepository mentryRepo;

        public MetricsRepository(IDbContext context, ILog log, RelationService relRepo,
            MetricsEntryRepository mentryRepo) : base(context, log, relRepo)
        {
            this.mentryRepo = mentryRepo;
        }

        public MetricsEntity FindByNameAndRelationIds(string name, int[] ids)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof (MetricsEntity));
                criteria.Add(Restrictions.In("Id", ids));
                criteria.Add(Restrictions.Eq("Name", name));
                return criteria.UniqueResult<MetricsEntity>();
            }
        }
        public int FindIdByNameAndRelationIds(string name, int[] ids)
        {
            if (ids.Count() == 0) return 0;
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof(MetricsEntity))
                    .Add(Restrictions.In("Id", ids))
                    .Add(Restrictions.Eq("Name", name))
                    .SetProjection(Projections.Property("Id"));
                return criteria.UniqueResult<int>();
            }
        }

        public override MetricsEntity Update(MetricsEntity entity)
        {
            CleanEntriesFor(entity);
            return base.Update(entity);
        }

        public override MetricsEntity SaveOrUpdate(MetricsEntity entity)
        {
            CleanEntriesFor(entity);
            return base.SaveOrUpdate(entity);
        }
        public override MetricsEntity Save(MetricsEntity entity)
        {
            CleanEntriesFor(entity);
            return base.Save(entity);
        }
        private void CleanEntriesFor(MetricsEntity entity)
        {
            var cleanAmount = Math.Max(entity.Entries.Count - entity.EntriesMax, 0);
            var toDelete = entity.Entries.Take(cleanAmount).ToList();
            foreach (var entry in toDelete)
            {
                entity.Entries.Remove(entry);
            }
            foreach (var entry in entity.Entries.Where(e=>e.IsTransient()))
            {
                mentryRepo.Save(entry);
            }
            mentryRepo.DeleteAll(toDelete);
        }
    }
}
