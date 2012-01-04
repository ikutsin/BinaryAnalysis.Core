using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Customization;
using log4net;
using NHibernate;
using NHibernate.Criterion;

namespace BinaryAnalysis.UI.Commons.Data
{
    class JqGridCustomCriteria<TM, TE> : CustomEntityCriteria<TM, TE>
        where TM : EntityBoxMap
        where TE : Entity
    {
        public JqGridCustomCriteria(RepositoryFinder repoFinder, ILog log, NHibernateBoxTransformation<TM, TE> transformation) : base(repoFinder, log, transformation)
        {
        }

        private int rows;
        private int page;
        private string sidx;
        private bool ascOrder;
        private Dictionary<string, string> searchQueries;
        public void SetParameters(int rows, int page, string sidx, bool ascOrder, Dictionary<string,string> searchQueries)
        {
            this.rows = rows;
            this.page = page;
            this.sidx = sidx;
            this.ascOrder = ascOrder;
            this.searchQueries = searchQueries;
        }


        #region Overrides of CustomCriteria<TM,TE>

        protected override void CriteriaCommand(ICriteria criteria)
        {
            if (searchQueries != null)
            {
                foreach (var searchQuery in searchQueries)
                {
                    Type pType = typeof (TE).GetProperty(searchQuery.Key).PropertyType;
                    if (pType==typeof(string))
                    {
                        criteria.Add(Restrictions.Like(searchQuery.Key, "%" + searchQuery.Value + "%"));
                    }
                    else if (pType == typeof(Int32))
                    {
                        int i = Int32.TryParse(searchQuery.Value, out i) ? i : 0;
                        criteria.Add(Restrictions.Eq(searchQuery.Key, i));
                    }
                    else
                    {
                        throw new Exception("Unknown type for querying " + pType);
                    }
                }
            }
            criteria.SetFirstResult(Math.Max(0, (page - 1) * rows));
            criteria.SetMaxResults(rows);
            if (!String.IsNullOrEmpty(sidx))
            {
                criteria.AddOrder(ascOrder ? Order.Asc(sidx) : Order.Desc(sidx));
            }
        }

        #endregion
    }
}
