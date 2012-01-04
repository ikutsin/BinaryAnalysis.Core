using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using NHibernate;
using NHibernate.Search;
using log4net;

namespace BinaryAnalysis.Data.Index
{
    public class IndexableRepository<T> : Repository<T> where T:class
    {
        //http://lucene.apache.org/java/2_9_4/api/core/org/apache/lucene/search/Query.html
        public IndexableRepository(IDbContext context, ILog log) : base(context, log) { }

        public void IndexAll()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                using(var fullTextSession = Search.CreateFullTextSession(wu.Session))
                {
                    ICriteria criteria = wu.Session.CreateCriteria(typeof(T));
                    var elements = criteria.List<T>();
                    foreach (var el in elements)
                    {
                        fullTextSession.Index(el);
                    }
                }
            }
            log.Info("Index for " + typeof(T) + " is created");
        }
        internal void PurgeAllIndex()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                using (var fullTextSession = Search.CreateFullTextSession(wu.Session))
                {
                    fullTextSession.PurgeAll(typeof(T));
                }
            }
            log.Info("Index for "+typeof(T)+" is purged");
        }

        public IList<T> QueryProperty(string field, string query)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                IFullTextSession fullTextSession = NHibernate.Search.Search.CreateFullTextSession(wu.Session);

                //QueryParser parser = new QueryParser(field, new StandardAnalyzer());
                //var luceneQuery = parser.Parse(query);
                var qq = fullTextSession.CreateFullTextQuery<T>(field, query);
                return qq.List<T>();
            }
        }
    }
}
