using System;
using System.Collections.Generic;
using log4net;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    public partial class ScriptFlow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ScriptFlow));

        SchedulerTask goal;
        internal bool IsGoalFinished { get; set; }
        internal List<KeyValuePair<string, long>> ScheduledScripts { get; set; }
        internal List<KeyValuePair<string, long>> ScheduledScriptsNext { get; set; }


        public ScriptFlow(SchedulerTask goal)
        {
            this.goal = goal;

            IsGoalFinished = false;
            ScheduledScripts = new List<KeyValuePair<string, long>>();
            ScheduledScriptsNext = new List<KeyValuePair<string, long>>();
            messages = new List<ScriptAssertionMessage>();
        }

        public void FireCustomEvent(object input)
        {
            Log.Debug("Firing custom event with input: "+input);
            goal.OnScriptCustomEvent(goal.Scripts[goal.RunningScriptName], input);
        }

        #region flow
        public void Fail(string message)
        {
            throw new FlowException(message);
        }
        public void FinishScript(string message = null)
        {
            if(!String.IsNullOrEmpty(message)) AddMessage(message);
            throw new FlowException(message, FlowExceptionType.Normal);
        }
        public void MarkGoalAsFinished()
        {
            IsGoalFinished = true;
        }
        public void FinishGoal(string message = null)
        {
            MarkGoalAsFinished();
            FinishScript(message);
        }

        public void Rerun(TimeSpan dueIn, string message = null)
        {
            Rerun((int)dueIn.TotalSeconds);
        }
        /// <summary>
        /// Reschedule current script
        /// </summary>
        /// <param name="dueInSeconds"></param>
        /// <param name="message"></param>
        public void Rerun(int dueInSeconds = 0, string message = null)
        {
            Schedule(goal.RunningScriptName, dueInSeconds);
            FinishScript(message);
        }

        public string CurrentScriptName
        {
            get { return goal.RunningScriptName; }
        }

        public void ScheduleThis(int dueInSeconds = 0)
        {
            Schedule(goal.RunningScriptName, dueInSeconds);
        }

        public void ScheduleImmediate(string name)
        {
            Schedule(name, -60*60);
        }

        public void Schedule(string name, TimeSpan dueIn)
        {
            Schedule(name, (int)dueIn.TotalSeconds);
        }
        public void Schedule(string name, int dueInSeconds = 0)
        {
            AddMessage(String.Format("Schedule '{0}' due in {1}sec", name, dueInSeconds));
            ScheduledScripts.Add(new KeyValuePair<string, long>(name, dueInSeconds));
        }
        #endregion
    }
}
