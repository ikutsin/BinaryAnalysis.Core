using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ConsoleClient.ShellCommands
{
    public class SchedulerCommands : ShellCommandSet
    {
        [CommandDescription("Get predefined taskparameters")]
        public void Available()
        {
            var goals = GetGoals();
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
            var goals = GetGoals();
            int num = -1;
            var name = input;
            if(Int32.TryParse(input, out num)&&num>0)
            {
                Writer.WriteLine("Taking numeric");
                //if(goals.Count<=num)
                name = goals[num - 1];
            }
            var goal = goals.Where(x => x == name).FirstOrDefault();
            if(goal!=null)
            {
                Writer.WriteLine("Scheduling '{0}'", name);
                var schedulerClient = Context.Resolve<ISchedulerService>();
                var result = schedulerClient.ScheduleNamedGoal(name);
                Writer.WriteLine("Result '{0}'", result);
            } else
            {
                Writer.WriteLine("'{0}' not found.", name);
            }
        }
        [CommandDescription("Get waiting schedules")]
        public void Waiting()
        {
            var q = new BoxQuery<ScheduleBoxMap>()
                .Where(x => (x.ExecutionState < ScheduleExecutionState.Running) &&
                            (x.MessageState < ScheduleMessageState.Error))
                .Where(x => x.NextExecution < DateTime.Now)
                .OrderBy(x => x.NextExecution);

            var boxed = q.ToList();
            Program.DumpBoxed(boxed);
        }
        [CommandDescription("Get finished schedules")]
        public void Finished()
        {
            var q = new BoxQuery<ScheduleBoxMap>()
                .Where(x => x.ExecutionState == ScheduleExecutionState.Finished);

            var boxed = q.ToList();
            Program.DumpBoxed(boxed);
        }

        List<string> GetGoals()
        {
            var schedulerClient = Context.Resolve<ISchedulerService>();
            return schedulerClient.GetAvailableNamedGoals();
        }
    }
}
