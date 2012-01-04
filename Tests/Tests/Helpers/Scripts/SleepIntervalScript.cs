using System;
using System.Threading;
using BinaryAnalysis.Extensions.Dependencies;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Tests.Helpers.Scripts
{
    public class SleepIntervalScript : AbstractTaskScript
    {
        public const string SETTING_SLEEP_INTERVAL = "testscript_sleep_interval";

        public override Type DependencyClassType
        {
            get { return typeof(NoDependencies); }
        }

        public override void Execute()
        {
            var interval = x.Settings.GetOrDefault(SETTING_SLEEP_INTERVAL, () => 3000);
            x.Flow.AddMessage("Sleep script started with " + interval);
            Thread.Sleep(interval);
            x.Flow.AddMessage("Sleep script finished with " + interval);
        }
    }
}
