using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using NHibernate;
using log4net;

namespace BinaryAnalysis.Data.Versioning
{
    public class TrackingRepository : Repository<TrackingEntity>
    {
        public TrackingRepository(IDbContext context, ILog log) : base(context, log) { }

        public IList<TrackingEntity> GetChangesFor(string objectName, int objectID, string propertyName = null)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return GetLastChangesFor(wu.Session, objectName, objectID, propertyName).ToList();
            }
        }
        protected IQueryable<TrackingEntity> GetLastChangesFor(ISession session, string objectName, int objectID, string propertyName = null)
        {
            var q= AsQueryable(session).Where(x => x.ObjectName == objectName && x.ObjectID == objectID);
            if (!String.IsNullOrEmpty(propertyName)) q = q.Where(x=>x.PropertyName == propertyName);
            return q.OrderByDescending(x => x.TrackingTime);
        }

        public TrackingEntity GetLastChangeFor(string objectName, int objectID, string propertyName)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return GetLastChangesFor(wu.Session, objectName, objectID, propertyName).FirstOrDefault();
            }
        }
    }
}
