using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Scheduler.Task.Settings
{
    public interface ITaskSettings
    {
        T Get<T>(string settingName);
        object Get(string settingName);
        void Set(string settingName, object value);
        T GetOrDefault<T>(string settingName) where T : class, new();
        T GetOrDefault<T>(string settingName, Func<T> defaultFunc);
        bool Has(string settingName);
        Dictionary<string, object> AsDictionary();
    }
}
