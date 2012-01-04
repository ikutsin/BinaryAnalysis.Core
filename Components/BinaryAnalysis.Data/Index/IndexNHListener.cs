using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core;
using NHibernate.Event;
using NHibernate.Search.Event;

namespace BinaryAnalysis.Data.Index
{
    public class IndexNHListener : FullTextIndexEventListener, ITypedListener
    {
        public ListenerType[] ListenerTypes
        {
            get
            {
                return new ListenerType[]
                {
                    ListenerType.PostUpdate, ListenerType.PostInsert, ListenerType.PostDelete
                };
            }
        }
    }
}
