using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Script;
using BinaryAnalysis.Scheduler.Task.Settings;
using log4net;

namespace BinaryAnalysis.Scheduler
{
    public class TaskFactory
    {
        protected ILog log;
        private readonly ILifetimeScope container;

        public TaskFactory(ILog log, ILifetimeScope container)
        {
            this.log = log;
            this.container = container;
        }

        public TaskParameters CreateSingleScriptTask<T>(Dictionary<string, object> paramz = null)
            where T : ISchedulerTaskScript, new()
        {
            if (paramz == null) paramz = new Dictionary<string, object>();
            var scripts = new Dictionary<string, ISchedulerTaskScript>();
            string name = "Single_" + typeof(T).FullName;
            scripts.Add(name, new T());

            var tp = new TaskParameters(scripts, paramz);
            tp.TaskName = name;
            return tp;
        }

        //methods
        public List<string> GetAvailableNamedGoals()
        {
            return container.ComponentRegistry.Registrations
                .Select(c => c.Services.FirstOrDefault() as KeyedService)
                .Where(s => s != null && s.ServiceType == typeof(TaskParameters))
                .Select(s => s.ServiceKey).Cast<string>().ToList();
        }

        public List<Tuple<string, string>> GetGoalProperties(string name)
        {
            var task = InitTaskFromContainer(name);
            return
                task.GetPropertyList(container).Select(
                    x => new Tuple<string, string>(x.Item1, (x.Item2 == null ? null : x.Item2.ToString())))
                    .ToList();
        }

        public List<Tuple<string, string>> GetRequiredProperties(string name)
        {
            var task = InitTaskFromContainer(name);
            return
                task.GetRequiredPropertyList(container).Select(
                    x => new Tuple<string, string>(x.Item1, (x.Item2 == null ? null : x.Item2.ToString())))
                    .ToList();
        }

        

        public SchedulerTask InitTaskFromContainer(string name, Dictionary<string,object> opts = null)
        {
            var paramz = container.ResolveNamed<TaskParameters>(name);
            if(paramz==null) throw new Exception(String.Format("TaskParameters named '{0}' not found",name));
            paramz.TaskName = name;
            if(opts!=null)
            {
                foreach (var p in opts)
                {
                    if (paramz.Settings.ContainsKey(p.Key)) {paramz.Settings[p.Key] = p.Value;}
                    else paramz.Settings.Add(p.Key, p.Value);
                }
            }
            return InitTaskFromParameters(paramz);
        }
        public SchedulerTask InitTaskFromParameters(TaskParameters paramz)
        {
            SchedulerTask task = container.Resolve<SchedulerTask>();
            task.TaskName = paramz.TaskName;
            task.Settings = new DetachedTaskSettings();
            task.Scripts = paramz.Scripts;
            foreach (var p in paramz.Settings)
            {
                //XXX: skip task and script speciffic params
                if (p.Key.StartsWith("task_"))
                {
                    log.Debug("skip param: " + p.Key);
                    continue;
                }
                task.Settings.Set(p.Key, p.Value);
            }
            return task;
        }
        public ScheduleMessageState RunTaskUntilFinished(SchedulerTask task)
        {
            ScheduleMessageState stateResult = ScheduleMessageState.None;
            using (var ctx = container.BeginLifetimeScope())
            {
                var stepsCnt = 0;
                do
                {
                    log.Info(String.Format("Starting step {0} for {1}", ++stepsCnt, task));
                    stateResult = task.Execute(ctx).Item2;
                } while (stateResult < ScheduleMessageState.Error && task.ScheduledScripts.Count > 0);
            }
            return stateResult;
        }
    }
}
