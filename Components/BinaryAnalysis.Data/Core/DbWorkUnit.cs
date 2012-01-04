using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using NHibernate;

namespace BinaryAnalysis.Data.Core
{
    public delegate void DbWorkUnitHandler(DbWorkUnit unit);
    public enum DbWorkUnitType : int
    {
        Read, Write
    }
    public abstract class DbWorkUnit : IDisposable
    {
        public event DbWorkUnitHandler Start;
        public event DbWorkUnitHandler Finish;

        public DbWorkUnitType Type
        {
            get;
            protected set;
        }

        public DbWorkUnit(DbWorkUnitType type = DbWorkUnitType.Read) 
        {
            this.Type = type;
        }
        public void Dispose()
        {
            OnFinish();
        }

        public abstract ISession Session { get; }

        public virtual void OnStart() 
        {
            if (Start != null) Start(this);
        }
        public virtual void OnFinish()
        {
            if (Finish != null) Finish(this);
        }
    }
}
