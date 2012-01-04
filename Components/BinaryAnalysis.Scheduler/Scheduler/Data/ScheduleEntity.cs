using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.Scheduler.Scheduler.Data
{
    [BoxTo(typeof(ScheduleBoxMap))]
    public class ScheduleEntity : Entity, ISettingsHolder
    {
        public const string OBJECT_NAME = "Schedule";

        public virtual string ObjectName { get { return OBJECT_NAME; } }

        public virtual string TaskName { get; set; }

        public virtual ScheduleMessageState MessageState { get; set; }
        public virtual ScheduleExecutionState ExecutionState { get; set; }
        public virtual ScheduleTaskParallelism Parallelism { get; set; }

        public virtual DateTime LastExecution { get; set; }
        public virtual DateTime NextExecution { get; set; }

        public virtual SettingsEntity Settings { get; set; }
    }
    public class ScheduleEntityMap : EntityClassMap<ScheduleEntity>
    {
        public ScheduleEntityMap()
        {
            Map(x => x.TaskName).Not.Nullable();

            Map(x => x.Parallelism).Not.Nullable();
            Map(x => x.MessageState).Not.Nullable();
            Map(x => x.ExecutionState).Not.Nullable();

            Map(x => x.LastExecution).Not.Nullable();
            Map(x => x.NextExecution).Not.Nullable();
        }
    }
    
}
