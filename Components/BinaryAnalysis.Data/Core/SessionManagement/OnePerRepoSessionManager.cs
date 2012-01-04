using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using log4net;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    public class OnePerRepoSessionManager : AbstractSessionManager
    {
        ILog log;
        public OnePerRepoSessionManager(ILog log)
        {
            this.log = log;
        }

        protected Dictionary<object, ISession> repositories = new Dictionary<object, ISession>();

        public override DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type = DbWorkUnitType.Read)
        {
            if (!repositories.ContainsKey(repo)) throw new Exception("Object not found in the list");
            
            var sess = repositories[repo];
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
            unit.Session.Flush();
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
            if (!repositories.ContainsKey(repo)) throw new Exception("Object not found in the list");
            var sess = repositories[repo];
            if (sess.IsOpen)
            {
                try
                {
                    sess.Flush();
                }catch(Exception ex)
                {
                    log.Debug(ex);
                }
            }
            sess.Dispose();
        }
        public override void NotifyRepoCreated(object repo)
        {
            if (repositories.ContainsKey(repo)) throw new Exception("Object is already in the list");
            
            var sess = context.SessionFactory.OpenSession();
            sess.CacheMode = CacheMode.Get;
            repositories.Add(repo, sess);
        }
    }
}
