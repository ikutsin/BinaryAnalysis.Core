using System;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Scheduler.Task.Commands
{
    public class RunGoalInlineScriptCommand : IScriptUtilityCommand
    {
        private readonly TaskFactory taskFactory;
        public Type InputType { get { return typeof(string); } }
        public Type ReturnType { get { return typeof(void); } }

        public RunGoalInlineScriptCommand(TaskFactory taskFactory)
        {
            this.taskFactory = taskFactory;
        }

        public object Execute(ScriptUtility x, object input)
        {
            var script = input as string;
            x.Flow.AddMessage("RunGoalInline: " + script);
            var task = taskFactory.InitTaskFromContainer(script, x.Settings.AsDictionary());
            task.Progress.Update += (p) => x.Flow.AddMessage(p.Message);
            
            x.Flow.IsTrue(taskFactory.RunTaskUntilFinished(task)>=ScheduleMessageState.Error).Fail("Inline action error");
            return null;
        }
    }
}
