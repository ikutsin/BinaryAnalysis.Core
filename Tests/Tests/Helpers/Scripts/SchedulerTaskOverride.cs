using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Script;
using BinaryAnalysis.Scheduler.Task.Settings;
using log4net;

namespace BinaryAnalysis.Tests.Helpers.Scripts
{
    class SchedulerTaskOverride : SchedulerTask
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SchedulerTaskOverride));
        public SchedulerTaskOverride() : base(log)
        {
        }

        public SchedulerTaskScriptDependencies TestDependency(ISchedulerTaskScript script, IComponentContext context)
        {
            var dep = ResolveScriptDependency(script, context);
            ValidateDependencies(dep);
            return dep;
        }
        public Dictionary<string, ISchedulerTaskScript> TestScripts 
        { 
            get { return base.Scripts; }
            set { base.Scripts = value; }
        }
        public ITaskSettings TestSettings
        {
            get { return base.Settings; }
            set { base.Settings = value; }
        }

        
    }
}
