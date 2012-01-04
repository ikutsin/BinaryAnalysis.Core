using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Event;

namespace BinaryAnalysis.Data.Core
{
    /// <summary>
    /// http://www.java2s.com/Open-Source/CSharp/Persistence-Frameworks/NHibernate/NHibernate/Event/EventListeners.cs.htm
    /// </summary>
    public interface ITypedListener
    {
        ListenerType[] ListenerTypes { get; }
    }
}
