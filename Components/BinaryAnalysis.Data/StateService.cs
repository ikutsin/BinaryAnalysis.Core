using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.State;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Classification;

namespace BinaryAnalysis.Data
{
    public class StateService
    {
        StateRepository repo;

        object stateLocker = new object();

        public StateService(StateRepository repo)
        {
            this.repo = repo;
        }

        public T Get<T>(string key) where T : class 
        {
            var state = repo.GetState(String.Format("{0}", key));
            if (state == null) return default(T);
            return state.GetValue<T>();
        }
        public StateEntity Put<T>(string key, object obj, TaxonomyNode trigger = null, long durationSeconds = 0, string description = null)
             where T : class 
        {
            lock (stateLocker)
            {
                Remove<T>(key);
                return repo.CreateAndPersist(String.Format("{0}", key), (T)obj, trigger, durationSeconds, description);
            }
        }
        public void Remove<T>(string key)
             where T : class 
        {
            lock (stateLocker)
            {
                var state = repo.GetState(String.Format("{0}", key));
                if (state != null) repo.Delete(state);
            }
        }
        public void Trigger(TaxonomyNode trigger, bool withChildren = false)
        {
            repo.Trigger(trigger, withChildren);
        }
    }
}
