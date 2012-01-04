using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;

namespace BinaryAnalysis.Data.Metrics
{
    public interface IMetricsHolder : IClassifiable
    {
        IList<MetricsEntity> Metrics { get; set; }
    }
}
