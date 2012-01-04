using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using log4net;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    public class OnePerCallSessionManager : AbstractSessionManager
    {
        ILog log;
        public OnePerCallSessionManager(ILog log)
        {
            this.log = log;
        }

        public override DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type = DbWorkUnitType.Read)
        {
            var sess = context.SessionFactory.OpenSession();
            var wu = new SimpleDbWorkUnit(sess, type);
            wu.Start += new DbWorkUnitHandler(wu_Start);
            wu.Finish += new DbWorkUnitHandler(wu_Finish);
            wu.OnStart();
            return wu;
        }

        void wu_Finish(DbWorkUnit unit)
        {
            unit.Start -= wu_Start;
            unit.Finish -= wu_Finish;

            if (unit.Type == DbWorkUnitType.Write)
            {
                if (unit.Session.Transaction.IsActive)
                {
                    unit.Session.Transaction.Commit();
                }
            }
            var sess = unit.Session;
            if (sess.IsOpen)
            {
                try
                {
                    sess.Flush();
                }
                catch (Exception ex)
                {
                    log.Debug(ex);
                }
            }
            sess.Dispose();
        }
        void wu_Start(DbWorkUnit unit)
        {
            unit.Session.FlushMode = FlushMode.Auto;
            if (unit.Type == DbWorkUnitType.Write)
            {
                unit.Session.Transaction.Begin();
            }
        }

        public override void NotifyRepoDisposed(object repo)
        {
        }
        public override void NotifyRepoCreated(object repo)
        {
        }
    }
}
