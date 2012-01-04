using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    class SimpleDbWorkUnit : DbWorkUnit
    {
        ISession session;
        public SimpleDbWorkUnit(ISession session, DbWorkUnitType type)
            : base(type)
        {
            this.session = session;
        }
        public override ISession Session
        {
            get { return session; }
        }
    }
}
