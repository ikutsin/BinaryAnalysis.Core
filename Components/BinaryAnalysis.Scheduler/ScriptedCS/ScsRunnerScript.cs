using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Autofac;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Scheduler.ScriptedCS
{
    public class ScsRunnerScriptDependencies : SchedulerTaskScriptDependencies
    {
        public IComponentContext Context { get; set; }

        #region Overrides of SchedulerTaskScriptDependencies
        public override IEnumerable<string> RequiredSettings
        {
            get { return null; }
        }
        #endregion
    }
    public class ScsRunnerScript : AbstractTaskScript
    {
        public const string SETTINGS_INPUT = "ScsRunnerScript_code";
        public const string SETTINGS_RESULT = "ScsRunnerScript_result";
        public override void Execute()
        {
            lock (typeof(InteractiveBase))
            {
                InteractiveBase.x = x;
                InteractiveBase.context = ((ScsRunnerScriptDependencies) Dependencies).Context;

                var code = x.Settings.Get<string>(SETTINGS_INPUT);
                object value = EvaluationHelper.Evaluate(code);
                
                x.Flow.IsNotNull(value).Info("Value: " + value);
                x.Settings.Set(SETTINGS_RESULT, value);
            }
        }

        public override Type DependencyClassType
        {
            get { return typeof(ScsRunnerScriptDependencies); }
        }
    }
}
