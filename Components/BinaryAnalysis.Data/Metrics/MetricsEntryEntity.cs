using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.Metrics
{
    [BoxTo(typeof(MetricsEntryBoxMap))]
    public class MetricsEntryEntity : Entity
    {
        public MetricsEntryEntity()
        {
            RecordDate = DateTime.Now;
        }
        public MetricsEntryEntity(decimal value):this()
        {
            Value = value;
        }

        public virtual MetricsEntity Metrics { get; set; }
        public virtual decimal Value { get; set; }
        public virtual DateTime RecordDate { get; set; }

        public override string ToString()
        {
            return ""+Value;
        }
    }
}
