using System;
using System.Collections.Generic;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Scheduler.Task.Settings;

namespace BinaryAnalysis.Scheduler.Scheduler
{
    /// <summary>
    /// Threadsafe settings wrapper
    /// </summary>
    public class EntityTaskSettings : ITaskSettings
    {
        object _locker = new object();

        SettingsEntity settings;
        public EntityTaskSettings(SettingsEntity settings)
        {
            if (settings == null) throw new Exception("Settings should not be NULL");
            this.settings = settings;
        }

        public T Get<T>(string settingName)
        {
            lock (_locker)
            {
                var val = Get(settingName);
                return (T)val;
            }
        }
        public object Get(string settingName)
        {
            lock (_locker)
            {
                var e = settings.GetEntry(settingName);
                return e == null ? null : e.GetValue();
            }
        }
        public void Set(string settingName, object value)
        {
            lock (_locker)
            {
                settings.SetEntry(settingName, value);
            }
        }

        public T GetOrDefault<T>(string settingName) where T : class, new() 
        {
            lock (_locker)
            {
                return GetOrDefault<T>(settingName, () => new T());
            }
        }
        public T GetOrDefault<T>(string settingName, Func<T> defaultFunc)// where T : class
        {
            lock (_locker)
            {
                var entry = settings.GetEntry(settingName);
                if (entry != null)
                {
                    return entry.GetValue<T>();
                }
                var ret = defaultFunc();
                settings.AddEntry(settingName, ret);
                return ret;
            }
        }

        public bool Has(string settingName)
        {
            return settings.GetEntry(settingName) != null;
        }

        public Dictionary<string, object> AsDictionary()
        {
            var result = new Dictionary<string, object>();
            foreach (var entry in settings.Entries)
            {
                result.Add(entry.Name, entry.GetValue());
            }
            return result;
        }
    }
}
