using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using NHibernate.Linq;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Metrics
{
    [BoxTo(typeof(MetricsBoxMap))]
    public class MetricsEntity : Entity, IClassifiable
    {
        public const string OBJECT_NAME = "Metrics";

        public MetricsEntity()
        {
            EntriesMax = 100;
            Entries = new List<MetricsEntryEntity>();
        }

        public MetricsEntity(string name, string descr = null):this()
        {
            Name = name;
            Description = descr;
        }

        public int EntriesMax { get; set; }

        [NotNullNotEmpty, Length(50)]
        public virtual string Name { get; set; }

        [Length(150)]
        public virtual string Description { get; set; }

        public string ObjectName
        {
            get { return OBJECT_NAME; }
        }

        public override string ToString()
        {
            return ObjectName + " " + Id + "=(" + Name +")";
        }

        public decimal GetLastValueOrDefault(decimal @default = 0)
        {
            var lastResult = Entries.OrderBy(x => x.RecordDate).LastOrDefault();
            if (lastResult == null) return @default;
            return lastResult.Value;
        }
        public DateTime GetLastChange()
        {
            var lastResult = Entries.OrderBy(x => x.RecordDate).LastOrDefault();
            if (lastResult == null) return new DateTime();
            return lastResult.RecordDate;
        }

        #region Entries
        public virtual IList<MetricsEntryEntity> Entries { get; set; }

        private object assignmentLocker = new object();

        public virtual void AddEntry(decimal value)
        {
            AddEntry(value, DateTime.Now);
        }
        public virtual void AddEntry(decimal value, DateTime at)
        {
            lock (assignmentLocker)
            {
                Entries.Add(new MetricsEntryEntity(value) { RecordDate = at, Metrics = this});
            }
        }
        public bool IsChanging(decimal value)
        {
            return IsChanging(value, DateTime.Now);
        }

        public bool IsChanging(decimal value, DateTime at)
        {
            lock (assignmentLocker)
            {
                var prev = Entries.Where(x => x.RecordDate < at)
                    .OrderBy(x => x.RecordDate).LastOrDefault();

                return (prev == null || prev.Value != value);
            }
        }

        #endregion
    }
}
