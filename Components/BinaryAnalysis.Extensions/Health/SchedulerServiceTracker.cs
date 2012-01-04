using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Scheduler.Scheduler;

namespace BinaryAnalysis.Extensions.Health
{
    public class SchedulerServiceTracker
    {
        private readonly SchedulerInstance _schedulerInstance;
        private readonly IComponentContext _context;

        public SchedulerServiceTracker(SchedulerInstance schedulerInstance, IComponentContext context)
        {
            _schedulerInstance = schedulerInstance;
            _context = context;

        }

        public void Start()
        {
            if (_schedulerInstance.IsStarted) throw new Exception("Start SchedulerServiceTrackHelper before scheduler!");
            if (!_context.IsRegistered<DurationTrackHelper>()) throw new Exception("DurationTrackHelper is not found in context");
            if (!_context.IsRegistered<FrequencyTrackHelper>()) throw new Exception("FrequencyTrackHelper is not found in context");

            DurationTrackHelper factoryDuration  = _context.Resolve<DurationTrackHelper>();
            _schedulerInstance.FactoryStarted += _ => factoryDuration.Start("factoryUptime");
            _schedulerInstance.FactoryFinished += _ => factoryDuration.TrackAndStop();

            FrequencyTrackHelper scriptStartFreq = _context.Resolve<FrequencyTrackHelper>();
            scriptStartFreq.Start(TimeSpan.FromHours(1), "scriptStartPerHour");
            FrequencyTrackHelper scriptFinishFreq = _context.Resolve<FrequencyTrackHelper>();
            scriptFinishFreq.Start(TimeSpan.FromHours(1), "scriptFinishPerHour");
            Dictionary<string, DurationTrackHelper> scriptDurations = new Dictionary<string, DurationTrackHelper>();
            Dictionary<string, DurationTrackHelper> scriptTypeDurations = new Dictionary<string, DurationTrackHelper>();

            object scriptDurationsLocker = new object();
            _schedulerInstance.ScriptStarted +=
                (t, s) =>
                    {
                        scriptStartFreq.Notify();
                        lock(scriptDurationsLocker)
                        {
                            string scriptName = t.TaskName + "_" + t.RunningScriptName;
                            if (!scriptDurations.ContainsKey(scriptName))
                            {
                                var tracker = _context.Resolve<DurationTrackHelper>();
                                scriptDurations.Add(scriptName, tracker);
                                tracker.Start("scripts", scriptName);
                            }
                            string scriptType = s.GetType().Name;
                            if (!scriptTypeDurations.ContainsKey(scriptType) && scriptType!="TaskScriptHandler")
                            {
                                var tracker = _context.Resolve<DurationTrackHelper>();
                                scriptTypeDurations.Add(scriptType, tracker);
                                tracker.Start("scriptType", scriptType);
                            }
                        }
                    };
            _schedulerInstance.ScriptFinished +=
                (t, s) =>
                    {
                        scriptFinishFreq.Notify();
                        lock (scriptDurationsLocker)
                        {
                            string scriptName = t.TaskName + "_" + t.RunningScriptName;
                            if (scriptDurations.ContainsKey(scriptName))
                            {
                                scriptDurations[scriptName].TrackAndStop();
                                scriptDurations.Remove(scriptName);
                            }

                            string scriptType = s.GetType().Name;
                            if (scriptTypeDurations.ContainsKey(scriptType))
                            {
                                scriptTypeDurations[scriptType].TrackAndStop();
                                scriptTypeDurations.Remove(scriptType);
                            }
                        }
                    };

            FrequencyTrackHelper taskStartFreq = _context.Resolve<FrequencyTrackHelper>();
            taskStartFreq.Start(TimeSpan.FromHours(1), "tasksStartPerHour");
            FrequencyTrackHelper taskFinishFreq = _context.Resolve<FrequencyTrackHelper>();
            taskFinishFreq.Start(TimeSpan.FromHours(1), "taskFinishPerHour");

            Dictionary<string, DurationTrackHelper> taskDurations = new Dictionary<string, DurationTrackHelper>();
            object taskDurationsLocker = new object();
            _schedulerInstance.TaskStarted +=
                (t) =>
                    {
                        taskStartFreq.Notify();
                        lock (taskDurationsLocker)
                        {
                            if (!taskDurations.ContainsKey(t.TaskName))
                            {
                                var tracker = _context.Resolve<DurationTrackHelper>();
                                taskDurations.Add(t.TaskName, tracker);
                                tracker.Start("tasks", t.TaskName);
                            }
                        }
                    };
            _schedulerInstance.TaskFinished += 
                (t) =>
                    {
                        taskFinishFreq.Notify();
                        lock (taskDurationsLocker)
                        {
                            string taskName = t.TaskName;
                            if (taskDurations.ContainsKey(taskName))
                            {
                                taskDurations[taskName].TrackAndStop();
                                taskDurations.Remove(taskName);
                            }
                        }
                    };
        }

    }
}
