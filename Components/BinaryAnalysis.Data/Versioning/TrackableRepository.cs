using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;
using NHibernate.Event;

namespace BinaryAnalysis.Data.Versioning
{
    public class TrackableRepository<T> : ClassifiableRepository<T> where T : Entity, ITrackable
    {
        private readonly TrackingRepository trackRepo;

        public TrackableRepository(IDbContext context, ILog log, RelationService relRepo,
            TrackingRepository trackRepo) : base(context, log, relRepo)
        {
            this.trackRepo = trackRepo;
        }

        public override T SaveOrUpdate(T entity)
        {
            TrackChanges(entity);
            return base.SaveOrUpdate(entity);
        }
        public override T Update(T entity)
        {
            TrackChanges(entity);
            return base.Update(entity);
        }
        public override void Delete(T entity)
        {
            CleanChangeHistory(entity);
            base.Delete(entity);
        }

        public void CleanChangeHistory(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var trackings =
                    trackRepo.AsQueryable(wu.Session).Where(
                        x => x.ObjectName == entity.ObjectName && x.ObjectID == entity.Id);
                trackRepo.DeleteAll(trackings);
            }            
        }

        public IList<ChangeSet> GetChanges(T trackable)
        {
            var changeSets = new Dictionary<DateTime, ChangeSet>();
            var changes = trackRepo.GetChangesFor(trackable.ObjectName, trackable.Id);
            foreach (var change in changes)
            {
                if (!changeSets.ContainsKey(change.TrackingTime))
                {
                    changeSets.Add(change.TrackingTime, new ChangeSet() { TrackingTime = change.TrackingTime });
                }
                changeSets[change.TrackingTime].Properties.Add(change.PropertyName, change.GetValue());
            }
            return changeSets.Values.ToList();
        }

        protected void TrackChanges(T currentEntity)
        {
            if(currentEntity.Id<=0) return;
            var trackingTime = DateTime.Now;

            T dbEntity = null;
            //Is it required: Evict(currentEntity);
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                dbEntity = wu.Session.Get<T>(currentEntity.Id);
                if (dbEntity != null) wu.Session.Evict(dbEntity);
            }
            if (dbEntity == null) return;

            log.Debug(String.Format("Tracking changes for {0}({1})", typeof(T), dbEntity.Id));

            //TODO: cache?
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            foreach (var prop in props)
            {
                foreach (object attr in prop.GetCustomAttributes(true))
                {
                    var trackUpdates = attr as TrackUpdatesAttribute;
                    if (trackUpdates != null)
                    {
                        var name = prop.Name;
                        if (!String.IsNullOrEmpty(trackUpdates.Name)) name = trackUpdates.Name;
                        var currentVal = prop.GetValue(currentEntity, null);
                        var dbVal = prop.GetValue(dbEntity, null);
                        if (currentVal != dbVal)
                        {
                            var te = new TrackingEntity()
                                         {
                                             ObjectID = dbEntity.Id,
                                             ObjectName = dbEntity.ObjectName,
                                             PropertyName = name,
                                             TrackingTime = trackingTime
                                         };
                            te.SetValue(dbVal);
                            log.Debug(String.Format("\t{0} is changed {1}=>{2}", name, dbVal, currentVal));
                            trackRepo.Save(te);
                        }
                    }
                }
            }
        }
    }            
}
