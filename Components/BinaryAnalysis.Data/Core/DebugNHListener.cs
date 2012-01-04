using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Event;
using log4net;

namespace BinaryAnalysis.Data.Core
{
    public class DebugNHListener :

        IAutoFlushEventListener,
        IMergeEventListener,
        IPersistEventListener,
        IDeleteEventListener,
        IDirtyCheckEventListener,
        IEvictEventListener,
        IFlushEventListener,
        IFlushEntityEventListener,
        ILoadEventListener,
        IInitializeCollectionEventListener,
        ILockEventListener,
        IRefreshEventListener,
        IReplicateEventListener,
        ISaveOrUpdateEventListener,
        IPreLoadEventListener,
        IPreUpdateEventListener,
        IPreDeleteEventListener,
        IPreInsertEventListener,
        IPreCollectionRecreateEventListener,
        IPreCollectionRemoveEventListener,
        IPreCollectionUpdateEventListener,
        IPostLoadEventListener,
        IPostUpdateEventListener,
        IPostDeleteEventListener,
        IPostInsertEventListener,
        IPostCollectionRecreateEventListener,
        IPostCollectionRemoveEventListener,
        IPostCollectionUpdateEventListener,

        ITypedListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DebugNHListener));

        public ListenerType[] ListenerTypes
        {
            get
            {
                return new ListenerType[] { 
                ListenerType.Autoflush,
                ListenerType.Merge,
                ListenerType.Create,
                ListenerType.CreateOnFlush,
                ListenerType.Delete,
                ListenerType.DirtyCheck,
                ListenerType.Evict,
                ListenerType.Flush,
                ListenerType.FlushEntity,
                ListenerType.Load,
                ListenerType.LoadCollection,
                ListenerType.Lock,
                ListenerType.Refresh,
                ListenerType.Replicate,
                ListenerType.SaveUpdate,
                ListenerType.Save,
                ListenerType.Update,
                ListenerType.PreLoad,
                ListenerType.PreUpdate,
                ListenerType.PreDelete,
                ListenerType.PreInsert,
                ListenerType.PreCollectionRecreate,
                ListenerType.PreCollectionRemove,
                ListenerType.PreCollectionUpdate,
                ListenerType.PostLoad,
                ListenerType.PostUpdate,
                ListenerType.PostDelete,
                ListenerType.PostInsert,
                ListenerType.PostCommitUpdate,
                ListenerType.PostCommitDelete,
                ListenerType.PostCommitInsert,
                ListenerType.PostCollectionRecreate,
                ListenerType.PostCollectionRemove,
                ListenerType.PostCollectionUpdate
                };
            }
        }

        public void OnAutoFlush(AutoFlushEvent @event)
        {
            log.Debug("AutoFlushEvent :" + @event);
        }

        public void OnMerge(MergeEvent @event, System.Collections.IDictionary copiedAlready)
        {
            log.Debug("OnMerge :" + @event);
        }

        public void OnMerge(MergeEvent @event)
        {
            log.Debug("OnMerge :" + @event);
        }

        public void OnDelete(DeleteEvent @event, Iesi.Collections.ISet transientEntities)
        {
            log.Debug("OnDelete :" + @event);
        }

        public void OnDelete(DeleteEvent @event)
        {
            log.Debug("OnDelete :" + @event);
        }

        public void OnDirtyCheck(DirtyCheckEvent @event)
        {
            log.Debug("OnDirtyCheck :" + @event);
        }

        public void OnEvict(EvictEvent @event)
        {
            log.Debug("OnEvict :" + @event);
        }

        public void OnFlush(FlushEvent @event)
        {
            log.Debug("OnFlush :" + @event);
        }

        public void OnFlushEntity(FlushEntityEvent @event)
        {
            log.Debug("OnFlushEntity :" + @event);
        }

        public void OnLoad(LoadEvent @event, LoadType loadType)
        {
            log.Debug("OnLoad :" + @event);
        }

        public void OnLock(LockEvent @event)
        {
            log.Debug("OnLock :" + @event);
        }

        public void OnRefresh(RefreshEvent @event, System.Collections.IDictionary refreshedAlready)
        {
            log.Debug("OnRefresh :" + @event);
        }

        public void OnRefresh(RefreshEvent @event)
        {
            log.Debug("OnRefresh :" + @event);
        }

        public void OnReplicate(ReplicateEvent @event)
        {
            log.Debug("OnReplicate :" + @event);
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            log.Debug("OnPreUpdate :" + @event);
            return false;
        }

        public void OnPreLoad(PreLoadEvent @event)
        {
            log.Debug("OnPreLoad :" + @event);
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            log.Debug("OnPreDelete :" + @event);
            return false;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            log.Debug("OnPreInsert :" + @event);
            return false;
        }

        public void OnPreRecreateCollection(PreCollectionRecreateEvent @event)
        {
            log.Debug("OnPreRecreateCollection :" + @event);
        }

        public void OnPreRemoveCollection(PreCollectionRemoveEvent @event)
        {
            log.Debug("OnPreRemoveCollection :" + @event);
        }

        public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
        {
            log.Debug("OnPreUpdateCollection :" + @event);
        }

        public void OnPostLoad(PostLoadEvent @event)
        {
            log.Debug("OnPostLoad :" + @event);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            log.Debug("OnPostInsert :" + @event);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            log.Debug("OnPostUpdate :" + @event);
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            log.Debug("OnPostDelete :" + @event);
        }

        public void OnPostRecreateCollection(PostCollectionRecreateEvent @event)
        {
            log.Debug("OnPostRecreateCollection :" + @event);
        }

        public void OnPostRemoveCollection(PostCollectionRemoveEvent @event)
        {
            log.Debug("OnPostRemoveCollection :" + @event);
        }

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
            log.Debug("OnPostUpdateCollection :" + @event);
        }

        public void OnPersist(PersistEvent @event, System.Collections.IDictionary createdAlready)
        {
            log.Debug("OnPersist :" + @event);
        }

        public void OnPersist(PersistEvent @event)
        {
            log.Debug("OnPersist :" + @event);
        }

        public void OnInitializeCollection(InitializeCollectionEvent @event)
        {
            log.Debug("OnInitializeCollection :" + @event);
        }

        public void OnSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            log.Debug("OnSaveOrUpdate :" + @event);
        }
    }
}
