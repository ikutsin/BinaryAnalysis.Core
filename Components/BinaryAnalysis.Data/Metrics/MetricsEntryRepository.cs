using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;
using NHibernate;
using NHibernate.Criterion;

namespace BinaryAnalysis.Data.Metrics
{
    public class MetricsEntryRepository : Repository<MetricsEntryEntity>
    {
        public MetricsEntryRepository(IDbContext context, ILog log) : base(context, log)
        {
        }

        public decimal GetValueAt(int metricsid, DateTime at)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof (MetricsEntryEntity))
                    .Add(Restrictions.Eq("Metrics.Id", metricsid))
                    .Add(Restrictions.Le("RecordDate", at))
                    .AddOrder(Order.Desc("RecordDate"));

                return criteria.SetProjection(Projections.Property("Value")).SetMaxResults(1).UniqueResult<decimal>();
            }
        }
    }
}
