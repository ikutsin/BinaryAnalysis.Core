using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Mixin
{
    public interface IMetricsValueMixin
    {
        DateTime MetricsDate { get; set; }
        decimal MetricsValue { get; set; }
    }
}
