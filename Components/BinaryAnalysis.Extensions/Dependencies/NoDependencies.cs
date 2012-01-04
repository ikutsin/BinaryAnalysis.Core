using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Dependencies
{
    public class NoDependencies : SchedulerTaskScriptDependencies
    {
        public override IEnumerable<string> RequiredSettings
        {
            get { return null; }
        }
    }
}
