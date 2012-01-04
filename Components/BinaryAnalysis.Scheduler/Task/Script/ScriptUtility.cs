using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Scheduler.Task.Settings;

namespace BinaryAnalysis.Scheduler.Task.Script
{
    public class ScriptUtility
    {
        private ScriptUtilityCommands utilityCommands;

        public ScriptUtility(ScriptUtilityCommands utilityCommands)
        {
            this.utilityCommands = utilityCommands;
            TaskBag = new DetachedTaskSettings();
        }

        public Settings.ITaskSettings Settings { get; internal set; }
        public Flow.ScriptFlow Flow { get; internal set; }

        //ugly hack to store taskwide properties
        public DetachedTaskSettings TaskBag { get; set; }

        public void Exec(string name, object input = null)
        {
            utilityCommands.Exec(this, name, input);
        }
        public T Exec<T>(string name, object input = null)
        {
            return utilityCommands.Exec<T>(this, name, input);
        }
        public bool TryExec(string name, object input = null)
        {
            if (!HasCommand(name)) return false;
            utilityCommands.Exec(this, name, input);
            return true;
        }
        public Tuple<bool,T> TryExec<T>(string name, object input = null)
        {
            if (!HasCommand(name)) return new Tuple<bool, T>(false, default(T));
            return new Tuple<bool, T>(true, utilityCommands.Exec<T>(this, name, input));
        }
        public bool HasCommand(string name)
        {
            return utilityCommands.CommandNames.Contains(name);
        }
        public IList<string> UtilityCommands
        {
            get { return utilityCommands.CommandNames.ToList(); }
        }

    }
}
