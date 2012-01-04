using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.Metrics
{
    public class MetricsEntryEntityMap : EntityClassMap<MetricsEntryEntity>
    {
        public MetricsEntryEntityMap()
        {
            Map(x => x.Value).Not.Nullable();
            Map(x => x.RecordDate).Not.Nullable();
            References(x => x.Metrics, "Metrics_Id").Not.Nullable();
        }
    }
}
