using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.Metrics
{
    public class MetricsEntityMap : EntityClassMap<MetricsEntity>
    {
        public MetricsEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Nullable();
            Map(x => x.EntriesMax);
            HasMany(x => x.Entries)
                .KeyColumn("Metrics_Id").Inverse()
                .OrderBy("RecordDate");
        }
    }

}
