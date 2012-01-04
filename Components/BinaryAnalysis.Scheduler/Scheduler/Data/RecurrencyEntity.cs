using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Settings;

namespace BinaryAnalysis.Scheduler.Scheduler.Data
{
     [BoxTo(typeof(RecurrencyBoxMap))]
    public class RecurrencyEntity : Entity, ISettingsHolder
    {
        public const string OBJECT_NAME = "Recurrency";

        public virtual string ObjectName { get { return OBJECT_NAME; } }

        public virtual string Cron { get; set; }
        public virtual string TaskName { get; set; }

        public virtual ScheduleTaskParallelism Parallelism { get; set; }
        public virtual DateTime LastSchedule { get; set; }

        public virtual SettingsEntity Settings { get; set; }
    }
    public class RecurrencyEntityMap : EntityClassMap<RecurrencyEntity>
    {
        public RecurrencyEntityMap()
        {
            Map(x => x.Cron).Not.Nullable();
            Map(x => x.TaskName).Not.Nullable();
            Map(x => x.Parallelism).Not.Nullable();

            Map(x => x.LastSchedule).Not.Nullable();
        }
    }
}
