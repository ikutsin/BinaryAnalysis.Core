using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Customization;
using BinaryAnalysis.Data.Mixin;
using log4net;
using NHibernate;

namespace BinaryAnalysis.Data.Metrics
{
    public class BulkMetricsImportAction : SimplePersistentAction<object>
    {
        public BulkMetricsImportAction(IDbContext context, RepositoryFinder repoFinder, ILog log) : base(context, repoFinder, log)
        {
        }

        #region Overrides of SimplePersistentAction<object>

        public override object ActionCommand()
        {
            throw new NotImplementedException("This part is not implemented, as a simgle solution moved to domain module");

            //FindByValue<T>(string metricsName, decimal value, DateTime at)
            //select t.* from metricsentry as t
            //join (select y.metrics_id, max(y.recorddate) as d from metricsentry as y 
            //where metrics_id in (select id from metrics where name = 'AlexaTopFile')
            //and y.recorddate<=634586400000000000
            //group by metrics_id) as yy
            //on t.metrics_id = yy.metrics_id and yy.d = t.recorddate
            //where t.value>10

            using (DbWorkUnit wu = GetSessionFor(DbWorkUnitType.Write))
            {
                return wu.Session.CreateCriteria(typeof (MetricsEntity)).List();
            }
        }

        #endregion
    }
}
