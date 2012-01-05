using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Scheduler.Task.Flow;
using BinaryAnalysis.Scheduler.Task.Script;
using BinaryAnalysis.Scheduler.Task.Settings;
using log4net;

namespace BinaryAnalysis.Scheduler.Task
{
    public delegate void TaskProgress(SchedulerTask sender);
    public delegate void ScriptCustomEvent(SchedulerTask sender, ISchedulerTaskScript script, object input);
    public delegate void ScriptProgress(SchedulerTask sender, ISchedulerTaskScript script);

    public class SchedulerTask
    {
        public const string SETTING_PROGRESS = "task_progress";
        public const string SETTING_RUNNING_SCRIPT = "task_RunningScriptName";
        public const string SETTING_SCRIPTS = "task_schedules";
        public const string SETTING_PREFIX_MESSAGES = "task_msg_";
        public const string SETTING_DUMP = "task_lastDump";
        public const string SETTING_LAST_STATE = "task_lastMessageState";

        private ILog log;

        public SchedulerTask(ILog log)
        {
            this.log = log;
        }

        public ITaskSettings Settings { get; protected internal set; }
        public string TaskName { get; internal set; }

        internal protected virtual Dictionary<string, ISchedulerTaskScript> Scripts { get; set; }

        public string RunningScriptName
        {
            get { return Settings.GetOrDefault<string>(SETTING_RUNNING_SCRIPT, () => null); }
            set { Settings.Set(SETTING_RUNNING_SCRIPT, value); }
        }
        internal List<BrowsingGoalScriptSchedule> ScheduledScripts
        {
            get { return Settings.GetOrDefault<List<BrowsingGoalScriptSchedule>>(SETTING_SCRIPTS); }
            set { Settings.Set(SETTING_SCRIPTS, value); }
        }
        public ProgressStatus Progress
        {
            get { return Settings.GetOrDefault<ProgressStatus>(SchedulerTask.SETTING_PROGRESS); }
        }

        private ScriptUtility scriptUtility;
        internal Tuple<DateTime, ScheduleMessageState> Execute(IComponentContext context)
        {
            if(Scripts == null) throw new InvalidOperationException("Scripts not set");
            if (Settings == null) throw new InvalidOperationException("Settings not set");
            if (TaskName == null) throw new InvalidOperationException("TaskName not set");

            InitScriptUtility(context);
            if (ScheduledScripts.Count > 0)
            {
                var kvp = ScheduledScripts.First();
                if (kvp.Date > DateTime.Now) throw new InvalidOperationException("Don't try to call tasks from the future");
                return ExecuteScript(kvp.ScriptName, context);
            }
            OnTaskStarted();
            return ExecuteScript(Scripts.Keys.First(), context);
        }

        private void InitScriptUtility(IComponentContext context)
        {
            if (scriptUtility == null)
            {
                scriptUtility = context.Resolve<ScriptUtility>();
                scriptUtility.Settings = Settings;
            }
        }

        public List<Tuple<string, object>> GetRequiredPropertyList(IComponentContext context)
        {
            var props = new HashSet<string>();
            foreach (var script in Scripts)
            {
                var dep = ResolveScriptDependency(script.Value, context);
                if (dep != null && dep.RequiredSettings != null)
                {
                    props.UnionWith(dep.RequiredSettings);
                }
            }
            return props.Select(x => new Tuple<string, object>(x, Settings.Get(x))).ToList();
        }

        public List<Tuple<string, object>> GetPropertyList(IComponentContext context)
        {
            var props = new HashSet<string>();
            foreach (var script in Scripts)
            {
                var dep = ResolveScriptDependency(script.Value, context);
                if (dep != null && dep.RequiredSettings != null)
                {
                    props.UnionWith(dep.RequiredSettings);
                }
                
            }
            var ret = props.Select(x => new Tuple<string, object>(x, Settings.Get(x))).ToList();
            foreach (var script in Scripts)
            {
                var depName = script.Value.DependencyClassType == null ? null : script.Value.DependencyClassType.Name;
                ret.Add(new Tuple<string, object>(script.Key + "_dependency", depName));
            }
            return ret;
        }

        protected SchedulerTaskScriptDependencies ResolveScriptDependency(ISchedulerTaskScript script, IComponentContext context)
        {
            if (script.DependencyClassType == null || script.DependencyClassType == typeof(void))
            {
                //TODO: return some default dependency
                return null;
            }

            var result = context.ResolveOptional(script.DependencyClassType) as SchedulerTaskScriptDependencies;

            //creating instance manually
            if (result == null)
            {
                result = Activator.CreateInstance(script.DependencyClassType)
                         as SchedulerTaskScriptDependencies;
                if (result == null) throw new Exception("Script Dependency can not be resolved. Check the dependency type");
                context.InjectProperties(result);
            }

            if(result==null) throw new Exception("Script Dependency can not be resolved");
            return result;
        }

        protected void ValidateDependencies(SchedulerTaskScriptDependencies result)
        {
            if (result!=null && result.RequiredSettings != null)
            {
                foreach (string requiredSetting in result.RequiredSettings)
                {
                    //TODO: match required seeting type
                    if (!Settings.Has(requiredSetting))
                        throw new Exception("Required setting '" + requiredSetting + "' is missing.");
                }
            }
        }

        protected Tuple<DateTime, ScheduleMessageState> ExecuteScript(string scriptName, IComponentContext context)
        {
            var script = Scripts[scriptName];
            if (script == null) throw new Exception("Script not found");
            RunningScriptName = scriptName;

            var flow = new ScriptFlow(this);
            flow.OnNewMessage +=
                (m) =>
                {
                    if (m.Type == ScheduleMessageState.Error)
                    {
                        log.Error(String.Format("{0}: {1}", m.Type, m.Message));
                    }
                    else
                    {
                        log.Info(String.Format("{0}: {1}", m.Type, m.Message));
                    }
                    Progress.SetProgress(m.Message);
                };
            scriptUtility.Flow = flow;

            flow.AddMessage(String.Format("Starting {1} for {0}", TaskName, scriptName));
            OnScriptStarted(script);
            //init
            try
            {
                flow.AddMessage(String.Format("Initializing {0}", scriptName), ScheduleMessageState.Debug);
                var dependency = ResolveScriptDependency(script, context);
                ValidateDependencies(dependency);
                script.Init(dependency, Settings, flow, scriptUtility);
            }
            catch (Exception ex)
            {
                flow.AddMessage("Script init failed", ScheduleMessageState.Error);
                flow.Dump(ex);
            }
            //execution
            if (flow.MessagesState < ScheduleMessageState.Error)
            {
                try
                {
                    script.Execute();
                }
                catch (FlowException ex)
                {
                    if (ex.ExceptionType == FlowExceptionType.Fail)
                    {
                        flow.AddMessage(ex.Message, ScheduleMessageState.Error);
                    }
                    else if (!String.IsNullOrEmpty(ex.Message))
                    {
                        flow.AddMessage(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    flow.AddMessage("Script failed with exception", ScheduleMessageState.Error);
                    flow.Dump(ex);
                }
            }
            //execution done - changing and saving settings
            Settings.Set(SETTING_LAST_STATE, flow.MessagesState);

            var currentTime = DateTime.Now;
            if (flow.MessagesState < ScheduleMessageState.Error)
            {
                //update schedules
                ScheduledScripts = ScheduledScripts.Skip(1).ToList();
                flow.ScheduledScripts.ForEach(x =>
                {
                    if (Scripts.Any(s => s.Key == x.Key))
                    {
                        ScheduledScripts.Add(new BrowsingGoalScriptSchedule() { ScriptName = x.Key, Date = currentTime.AddSeconds(x.Value) });
                    }
                    else
                    {
                        flow.AddMessage(String.Format("Script {0} not found for schedule.", x.Key), ScheduleMessageState.Error);
                    }
                });
                ScheduledScripts = ScheduledScripts.OrderBy(x => x.Date).ToList();
                flow.AddMessage(String.Format("{0} scripts in queue", ScheduledScripts.Count), ScheduleMessageState.Debug);
            }
            flow.AddMessage(String.Format("Script {0}.{1} finished", TaskName, RunningScriptName), flow.MessagesState);
            Settings.Set(SETTING_PREFIX_MESSAGES + scriptName, flow.Messages);

            Progress.SetProgressRemaining(RunningScriptName + " complete",
                (int)((0.0 + ProgressStatus.PROGRESS_MAX) * (Math.Min(ScheduledScripts.Count, Scripts.Count) / (0.0+Scripts.Count))));

            OnScriptFinished(script);
            if (ScheduledScripts.Count == 0 || flow.MessagesState >= ScheduleMessageState.Error)
            {
                flow.AddMessage(String.Format("Task '{0}' finished", TaskName));
                OnTaskFinished();
            }
            RunningScriptName = null;
            return new Tuple<DateTime, ScheduleMessageState>(
                (ScheduledScripts.Count == 0) ? DateTime.MaxValue :
                ScheduledScripts.First().Date, flow.MessagesState);
        }

        #region Event model
        public event TaskProgress TaskStarted;
        public event TaskProgress TaskFinished;
        public event ScriptProgress ScriptStarted;
        public event ScriptProgress ScriptFinished;
        public event ScriptCustomEvent ScriptCustomEvent;
        protected void OnTaskStarted()
        {
            if (TaskStarted != null) TaskStarted(this);
        }
        protected void OnTaskFinished()
        {
            if (TaskFinished != null) TaskFinished(this);
        }
        protected virtual void OnScriptStarted(ISchedulerTaskScript script)
        {
            if (ScriptStarted != null) ScriptStarted(this, script);
        }
        protected virtual void OnScriptFinished(ISchedulerTaskScript script)
        {
            if (ScriptFinished != null) ScriptFinished(this, script);
        }
        internal virtual void OnScriptCustomEvent(ISchedulerTaskScript script, object input)
        {
            if (ScriptCustomEvent != null) ScriptCustomEvent(this, script, input);
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0}({1})", TaskName, RunningScriptName);
        }
    }
}
