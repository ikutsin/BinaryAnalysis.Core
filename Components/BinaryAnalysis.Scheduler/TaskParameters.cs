using System;
using System.Collections.Generic;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Scheduler
{
    [Serializable]
    public class TaskParameters
    {
        public TaskParameters(
            Dictionary<string, ISchedulerTaskScript> scripts,
            Dictionary<string, object> settings = null)
        {
            Scripts = scripts;
            if (Scripts==null || Scripts.Count<=0) throw new InvalidOperationException("There should be at least one script");
            if (settings == null) this.Settings = new Dictionary<string, object>();
            else Settings = settings;
        }

        public string TaskName { get; internal set; }
        public Dictionary<string, object> Settings { get; set; }
        public Dictionary<string, ISchedulerTaskScript> Scripts { get; set; }

        public override string ToString()
        {
            return String.Format("TP {0}", TaskName);
        }
    }
}
