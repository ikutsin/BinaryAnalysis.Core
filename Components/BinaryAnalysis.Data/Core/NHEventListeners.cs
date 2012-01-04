using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Event.Default;
using NHibernate.Util;
using NHibernate.Event;

namespace BinaryAnalysis.Data.Core
{
	/// <summary> 
	/// Table is taken from HNibernate 3.0.0 source code
	/// </summary>
	[Serializable]
	public class NHEventListeners
	{
		public static readonly IDictionary<ListenerType, System.Type> EventInterfaceFromType =
			new Dictionary<ListenerType, System.Type>(28);

        static NHEventListeners()
		{
			EventInterfaceFromType[ListenerType.Autoflush] = typeof (IAutoFlushEventListener);
			EventInterfaceFromType[ListenerType.Merge] = typeof (IMergeEventListener);
			EventInterfaceFromType[ListenerType.Create] = typeof (IPersistEventListener);
			EventInterfaceFromType[ListenerType.CreateOnFlush] = typeof (IPersistEventListener);
			EventInterfaceFromType[ListenerType.Delete] = typeof (IDeleteEventListener);
			EventInterfaceFromType[ListenerType.DirtyCheck] = typeof (IDirtyCheckEventListener);
			EventInterfaceFromType[ListenerType.Evict] = typeof (IEvictEventListener);
			EventInterfaceFromType[ListenerType.Flush] = typeof (IFlushEventListener);
			EventInterfaceFromType[ListenerType.FlushEntity] = typeof (IFlushEntityEventListener);
			EventInterfaceFromType[ListenerType.Load] = typeof (ILoadEventListener);
			EventInterfaceFromType[ListenerType.LoadCollection] = typeof (IInitializeCollectionEventListener);
			EventInterfaceFromType[ListenerType.Lock] = typeof (ILockEventListener);
			EventInterfaceFromType[ListenerType.Refresh] = typeof (IRefreshEventListener);
			EventInterfaceFromType[ListenerType.Replicate] = typeof (IReplicateEventListener);
			EventInterfaceFromType[ListenerType.SaveUpdate] = typeof (ISaveOrUpdateEventListener);
			EventInterfaceFromType[ListenerType.Save] = typeof (ISaveOrUpdateEventListener);
			EventInterfaceFromType[ListenerType.Update] = typeof (ISaveOrUpdateEventListener);
			EventInterfaceFromType[ListenerType.PreLoad] = typeof (IPreLoadEventListener);
			EventInterfaceFromType[ListenerType.PreUpdate] = typeof (IPreUpdateEventListener);
			EventInterfaceFromType[ListenerType.PreDelete] = typeof (IPreDeleteEventListener);
			EventInterfaceFromType[ListenerType.PreInsert] = typeof (IPreInsertEventListener);
			EventInterfaceFromType[ListenerType.PreCollectionRecreate] = typeof (IPreCollectionRecreateEventListener);
			EventInterfaceFromType[ListenerType.PreCollectionRemove] = typeof (IPreCollectionRemoveEventListener);
			EventInterfaceFromType[ListenerType.PreCollectionUpdate] = typeof (IPreCollectionUpdateEventListener);
			EventInterfaceFromType[ListenerType.PostLoad] = typeof (IPostLoadEventListener);
			EventInterfaceFromType[ListenerType.PostUpdate] = typeof (IPostUpdateEventListener);
			EventInterfaceFromType[ListenerType.PostDelete] = typeof (IPostDeleteEventListener);
			EventInterfaceFromType[ListenerType.PostInsert] = typeof (IPostInsertEventListener);
			EventInterfaceFromType[ListenerType.PostCommitUpdate] = typeof (IPostUpdateEventListener);
			EventInterfaceFromType[ListenerType.PostCommitDelete] = typeof (IPostDeleteEventListener);
			EventInterfaceFromType[ListenerType.PostCommitInsert] = typeof (IPostInsertEventListener);
			EventInterfaceFromType[ListenerType.PostCollectionRecreate] = typeof (IPostCollectionRecreateEventListener);
			EventInterfaceFromType[ListenerType.PostCollectionRemove] = typeof (IPostCollectionRemoveEventListener);
			EventInterfaceFromType[ListenerType.PostCollectionUpdate] = typeof (IPostCollectionUpdateEventListener);
			EventInterfaceFromType = new UnmodifiableDictionary<ListenerType, System.Type>(EventInterfaceFromType);
		}
	}
}
