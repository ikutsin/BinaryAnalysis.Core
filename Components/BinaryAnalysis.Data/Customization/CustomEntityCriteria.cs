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
    public abstract class CustomEntityCriteria<TM, TE>
		where TM : EntityBoxMap
		where TE : Entity
    {
        protected readonly RepositoryFinder _repoFinder;
        protected readonly ILog _log;
        private readonly IRepository<TE> repo;

        public IRepository<TE> UsedRepository
        {
            get { return repo; }
        }
    
        private readonly NHibernateBoxTransformation<TM, TE> _transformation;

        public CustomEntityCriteria(RepositoryFinder repoFinder, ILog log,
            NHibernateBoxTransformation<TM, TE> transformation)
        {
            _repoFinder = repoFinder;
            _log = log;
            _transformation = transformation;
            repo = repoFinder.CreateRepository<TE>();
        }

        protected abstract void CriteriaCommand(ICriteria criteria);
        
        public T UniqueResult<T>()
        {
            return UsedRepository.CriteriaUnique<T>(CriteriaCommand);
        }
        public TE UniqueResult()
        {
            return UsedRepository.CriteriaUnique<TE>(CriteriaCommand);
        }
        public IList<TE> ListResult()
        {
            return UsedRepository.CriteriaList(CriteriaCommand).Cast<TE>().ToList();
        }
        public TM UniqueResultBox()
        {
            _transformation.Entries = new List<TE> {UniqueResult()};
            return _transformation.ToBox().FirstOrDefault();
        }
        public IBox<TM> ListResultBox()
        {
            _transformation.Entries = ListResult();
            return _transformation.ToBox();
        }
    }
}
