using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using log4net;

namespace BinaryAnalysis.Data.State
{
    [Flags]
    enum StateExpiration 
    {
        Time, Trigger
    }
    /// <summary>
    /// Thread unsafe staste repo
    /// </summary>
    public class StateRepository : ClassifiableRepository<StateEntity>
    {
        public const string TRIGGER_PATH = @"BinaryAnalysis.Core/StateTriggerRelation";
        
        TaxonomyTree taxonomyTree;
        RelationService relationService;
        TaxonomyNode triggerRelation;

        public StateRepository(
            IDbContext context,
            TaxonomyTree taxonomyTree,
            RelationService relationService,
            ILog log)
            : base(context, log, relationService)
        {
            triggerRelation = taxonomyTree.GetOrCreatePath(TRIGGER_PATH, "StateRepository trigger relations");

            this.taxonomyTree = taxonomyTree;
            this.relationService = relationService;
        }

        public StateEntity GetState(string key)
        {
            Cleanup();
            var entity = FindOne(new Dictionary<string, object>() { { "Name", key } });
            if (entity != null) this.Evict(entity);
            return entity;
        }

        public List<String> FindKeys(string prefix, params string[] str)
        {
            //optimize
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var q = AsQueryable(wu.Session);
                q = q.Where(x => x.Name.StartsWith(prefix));
                foreach (var s in str)
                {
                    q = q.Where(x => x.Name.Contains(s));
                }
                return q.Select(x => x.Name).ToList();
            }
        }

        public StateEntity CreateAndPersist(string key, object obj, TaxonomyNode trigger = null, long durationSeconds = 0, string description = null) 
        {
            if (trigger == null && durationSeconds <= 0) throw new DataLayerException("Select at least one trigger");
            StateEntity entity = new StateEntity()
            {
                DieDate = DateTime.Now.AddSeconds(durationSeconds > 0 ? durationSeconds : TimeSpan.FromDays(1).TotalSeconds),
                Name = key,
                Description = description
            };
            entity.SetValue(obj);

            if(this.IsValid(entity))
            {
                this.Save(entity);
                if (trigger != null)
                    relationService.AddRelation(trigger, entity, RelationDirection.Both, triggerRelation);
                return entity;
            }
            log.Debug("There is an attempt to save an invalid state");
            return null;
        }

        public void Trigger(TaxonomyNode trigger, bool withChildren = false) 
        {
            if(trigger==null) return;
            var states = relationService.GetRelated<StateEntity>(trigger, RelationDirection.Both, triggerRelation);
            
            //TODO: optimize
            if(withChildren) 
            {
                foreach(var taxonChild in trigger.GetAllChildren()) 
                {
                    var childStates = relationService.GetRelated<StateEntity>(taxonChild, RelationDirection.Both, triggerRelation);
                    foreach(var childState in childStates) {
                        states.Add(childState);
                    }
                }
            }
            
            foreach (var state in states)
            {
                log.Debug("DeleteTrigger for: " + state.Name);
                this.Delete(state);
            }
        }

        public void Cleanup()
        {
            DateTime currentTime = DateTime.Now;
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                var stateEntities = this.AsQueryable(wu.Session)
                    .Where(x => x.DieDate < currentTime).ToList();
                if (stateEntities.Count > 0)
                {
                    log.Debug("State cleanup: " + stateEntities.Count);
                    foreach (var state in stateEntities)
                    {
                        wu.Session.Delete(state);
                    }
                }
            }
        }
    }
}
