using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using log4net;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    public class SingleSessionManager : AbstractSessionManager
    {
        ILog log;
        ISession session;
        
        public SingleSessionManager(ILog log)
        {
            this.log = log;
        }

        public override DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type = DbWorkUnitType.Read)
        {
            if (session == null) session = context.SessionFactory.OpenSession();
            else if(!session.IsOpen)
            {
                session.Dispose();
                session = context.SessionFactory.OpenSession();
            }
            
            return new SimpleDbWorkUnit(session, type);
        }


        public override void NotifyRepoDisposed(object repo)
        {
        }
        public override void NotifyRepoCreated(object repo)
        {
        }
    }
}
