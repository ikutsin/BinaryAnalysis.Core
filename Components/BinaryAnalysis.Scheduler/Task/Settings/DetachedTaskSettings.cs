using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Scheduler.Task.Settings
{
    public class DetachedTaskSettings : ITaskSettings
    {
        Dictionary<string,object> settings = new Dictionary<string, object>();
        public T Get<T>(string settingName)
        {
            var val = Get(settingName);
            return (T)val;
        }

        public object Get(string settingName)
        {
            if (settings.ContainsKey(settingName))
            {
                return settings[settingName];
            }
            return null;
        }

        public void Remove(string settingName)
        {
            if (settings.ContainsKey(settingName))
            {
                settings.Remove(settingName);
            }
        }

        public void Set(string settingName, object value)
        {
            if (settings.ContainsKey(settingName))
            {
                settings[settingName] = value;
            }
            else
            {
                settings.Add(settingName,value);
            }

        }

        public T GetOrDefault<T>(string settingName) where T : class, new()
        {
            return GetOrDefault<T>(settingName, () => new T());
        }

        public T GetOrDefault<T>(string settingName, Func<T> defaultFunc)
        {
            var entry = Get(settingName);
            if (entry != null)
            {
                return (T) entry;
            }
            var ret = defaultFunc();
            Set(settingName, ret);
            return ret;
        }

        public bool Has(string settingName)
        {
            return settings.ContainsKey(settingName);
        }

        public Dictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>(settings);
        }
    }
}
