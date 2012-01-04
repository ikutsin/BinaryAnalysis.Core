using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;
using NHibernate;

namespace BinaryAnalysis.Data.Customization
{
    public abstract class SimplePersistentAction<T>
    {
        protected readonly IDbContext _context;
        protected readonly RepositoryFinder _repoFinder;
        protected readonly ILog _log;
        protected readonly IRepository<Entity> _repo;

        public SimplePersistentAction(IDbContext context, RepositoryFinder repoFinder, ILog log)
        {
            _context = context;
            _repoFinder = repoFinder;
            _log = log;

            _repo = _repoFinder.CreateRepository<Entity>();
        }

        protected DbWorkUnit GetSessionFor(DbWorkUnitType type = DbWorkUnitType.Read)
        {
            return _context.SessionManager.WorkUnitFor(_repo, type);
        }

        public abstract T ActionCommand();
    }
}
