using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Task.Flow;
using BinaryAnalysis.Scheduler.Task.Settings;
using BinaryAnalysis.UI.BrowserContext;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BinaryAnalysis.Visuals.Scheduler.BrowserContext
{
    [ComVisible(true)]
    public class SchedulerContextExtension : IBrowserContextExtension
    {
        private readonly TaskFactory _taskFactory;
        private readonly ILog _log;
        private ITaskSettings lastSettings;

        public SchedulerContextExtension(TaskFactory taskFactory, ILog log)
        {
            _taskFactory = taskFactory;
            _log = log;
        }

        public string available()
        {
            var goals = _taskFactory.GetAvailableNamedGoals();
            return JsonConvert.SerializeObject(goals.Select(x => new { Name = x }));
        }

        public string runGetSettings(string name, string paramz = null)
        {
            var p = ParseParams(paramz);
            var runnerForm = new TaskRunnerForm(name, p, true);
            var showSettingsResult = runnerForm.ShowDialog();
            lastSettings = runnerForm.CurrentTask.Settings;
            return getLastSettings();
        }
        public string run(string name, string paramz = null)
        {
            var p = ParseParams(paramz);
            var runnerForm = new TaskRunnerForm(name, p);
            var showSettingsResult = runnerForm.ShowDialog();
            if (showSettingsResult == DialogResult.Yes)
            {
                lastSettings = runnerForm.CurrentTask.Settings;
            }
            else
            {
                lastSettings = null;
            }
            return getLastSettings();
        }

        private Dictionary<string, object> ParseParams(string paramz)
        {
            Dictionary<string, object> p = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(paramz))
            {
                var pp = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramz);
                foreach (var o in pp)
                {
                    p.Add(o.Key, ParseValue(o.Value));
                }
            }
            return p;
        }
        private object ParseValue(object obj)
        {
            int integer;
            if (obj is JArray)
            {
                return (obj as JArray).Select(ParseValue).ToArray();
            }
            if (Int32.TryParse(obj.ToString(), out integer))
            {
                return integer;
            }
            return obj;
        }

        public string props(string name)
        {
            var props = _taskFactory.GetGoalProperties(name);
            var obj = props.Select(x => new {Name = x.Item1, Value = x.Item2});
            return JsonConvert.SerializeObject(obj);
        }
        public string propsRequired(string name)
        {
            var props = _taskFactory.GetRequiredProperties(name);
            var obj = props.Select(x => new { Name = x.Item1, Value = x.Item2 });
            return JsonConvert.SerializeObject(obj);
        }

        public string getLastSettings()
        {
            if (lastSettings == null) return null;
            return JsonConvert.SerializeObject(lastSettings.AsDictionary()
                .Select(x => new { Key = x.Key, Value = PrepareValues(x.Key, x.Value) }));
        }
        protected object PrepareValues(string name, object val)
        {
            if (val is DumpMessage)
            {
                var dump = (val as DumpMessage);
                try
                {
                    var ex = (dump.GetDumpObj<Exception>());
                    var ret = new Dictionary<string, string>();
                    ret.Add("Message", ex.Message);
                    ret.Add("StackTrace", ex.StackTrace);
                    return ret;
                }
                catch
                {
                    return val;
                }

            }
            if (val is IList)
            {
                var ret = new Dictionary<string, string>();
                int i = 0;
                foreach (var v in (val as IList))
                {
                    ret.Add((i++).ToString(), v.ToString());
                }
                return ret;
            }
            return val;
        }


    }
}
