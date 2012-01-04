using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Log;
using NHibernate;
using NHibernate.Search;
using log4net;

namespace BinaryAnalysis.Data.Index
{
    public class ActivityLogRepository : Repository<ActivityLogEntity>
    {
        public ActivityLogRepository(IDbContext context, ILog log) : base(context, log)
        {
            
        }

    }
}
