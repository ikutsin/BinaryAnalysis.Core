using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    public abstract class AbstractSessionManager : ISessionManager
    {
        protected IDbContext context;
        public void Init(IDbContext context)
        {
            this.context = context;
        }
        public abstract void NotifyRepoDisposed(object repo);
        public abstract void NotifyRepoCreated(object repo);
        public abstract DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type = DbWorkUnitType.Read);
    }
}
