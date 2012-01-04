using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Scheduler.Task.Script
{
    public abstract class SchedulerTaskScriptDependencies
    {
        public abstract IEnumerable<string> RequiredSettings { get; }
    }
}
