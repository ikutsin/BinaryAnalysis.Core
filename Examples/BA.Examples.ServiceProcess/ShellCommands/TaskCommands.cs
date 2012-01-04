using System;
using System.Linq;
using Autofac;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ServiceProcess.ShellCommands
{
    public class TaskCommands : ShellCommandSet
    {
        [CommandDescription("Get predefined taskparameters")]
        public void Available()
        {
            var goals = Context.Resolve<TaskFactory>().GetAvailableNamedGoals();
            var cnt = 1;
            foreach (var goal in goals)
            {
                Writer.WriteLine("{0}: {1}", cnt++, goal);
            }
            Writer.WriteLine("\tTotal: {0}", goals.Count);
        }
        [CommandDescription("Schedule goal")]
        public void Schedule(string input)
        {
            var goals = Context.Resolve<TaskFactory>().GetAvailableNamedGoals();
            int num = -1;
            var name = input;
            if (Int32.TryParse(input, out num) && num > 0)
            {
                Writer.WriteLine("Taking numeric");
                //if(goals.Count<=num)
                name = goals[num - 1];
            }
            var goal = goals.Where(x => x == name).FirstOrDefault();
            if (goal != null)
            {
                Writer.WriteLine("Scheduling '{0}'", name);
                var result = Context.Resolve<SchedulerInstance>().Schedule(name);
                Writer.WriteLine("Result '{0}'", result);
            }
            else
            {
                Writer.WriteLine("'{0}' not found.", name);
            }
        }
    }
}
