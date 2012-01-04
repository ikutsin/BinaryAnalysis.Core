using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Core.SessionManagement
{
    public interface ISessionManager
    {
        void Init(IDbContext context);

        DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type = DbWorkUnitType.Read);
        void NotifyRepoCreated(object repo);
        void NotifyRepoDisposed(object repo);
    }
}
