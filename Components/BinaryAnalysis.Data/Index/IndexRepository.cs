using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core;
using NHibernate;
using NHibernate.Search;
using BinaryAnalysis.Data.Core.SessionManagement;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.Standard;
using NHibernate.Search.Backend;
using NHibernate.Search.Impl;
using Lucene.Net.Index;
using System.Reflection;
using NHibernate.Search.Attributes;
using log4net;

namespace BinaryAnalysis.Data.Index
{
    public class IndexRepository : IDisposable
    {
        public class IndexManager<T> : IIndexManager where T:class
        {
            IndexRepository thisRepo;
            public IndexManager(IndexRepository thisRepo)
            {
                this.thisRepo = thisRepo;
            }

            public void Optimize()
            {
                var searchFactory = SearchFactoryImpl.GetSearchFactory(thisRepo.context.CurrentConfiguration);
                searchFactory.Optimize(typeof(T));
            }
            public void Purge()
            {
                using (var repo = new IndexableRepository<T>(thisRepo.context, thisRepo.log))
                {
                    repo.PurgeAllIndex();
                }
            }
            public void Fill()
            {
                using (var repo = new IndexableRepository<T>(thisRepo.context, thisRepo.log))
                {
                    repo.IndexAll();
                }
            }
        }

        public IndexManager<T> ManageIndex<T>() where T : class
        {
            return new IndexManager<T>(this);
        }

        List<IIndexManager> _indexManagers;
        public List<IIndexManager> IndexManagers
        {
            get
            {
                if (_indexManagers == null)
                {
                    _indexManagers = context.CurrentConfiguration
                        .ClassMappings.Select(x => x.MappedClass)
                        .Select(c => new
                        {
                            clazz = c,
                            attr = (IndexedAttribute)
                                c.GetCustomAttributes(typeof(IndexedAttribute), true).FirstOrDefault()
                        })
                        .Where(p => p.attr != null)
                        .Select(p =>
                        {
                            Type indexMgrGeneric = typeof(IndexManager<>).MakeGenericType(p.clazz);
                            return (IIndexManager)Activator.CreateInstance(indexMgrGeneric, this);
                        }).ToList();
                }
                return _indexManagers;
            }
        }

        protected ISessionManager SessionManager { get; set; }

        IDbContext context;
        ILog log;
        
        public IndexRepository(IDbContext context, ILog log)
        {
            SessionManager = context.SessionManager;
            SessionManager.NotifyRepoCreated(this);
            this.context = context;
            this.log = log;
        }

        public IList<T> RawQuery<T>(string field, string query)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                IFullTextSession fullTextSession = NHibernate.Search.Search.CreateFullTextSession(wu.Session);
                //QueryParser parser = new QueryParser(field, new StandardAnalyzer());
                //var luceneQuery = parser.Parse(query);
                try
                {
                    var qq = fullTextSession.CreateFullTextQuery<T>(field, query);
                    return qq.List<T>();
                }
                catch (ObjectNotFoundException ex)
                {
                    return new List<T>();
                }
            }
        }
        public void Dispose()
        {
            SessionManager.NotifyRepoDisposed(this);
        }
    }
}
