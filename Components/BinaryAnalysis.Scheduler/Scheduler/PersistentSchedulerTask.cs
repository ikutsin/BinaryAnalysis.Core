using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Scheduler.Task;
using log4net;

namespace BinaryAnalysis.Scheduler.Scheduler
{
    public class PersistentSchedulerTask : SchedulerTask
    {
        internal ScheduleEntity Entity { get; set; }
        public PersistentSchedulerTask(ILog log) : base(log)
        {
        }

    }
}
