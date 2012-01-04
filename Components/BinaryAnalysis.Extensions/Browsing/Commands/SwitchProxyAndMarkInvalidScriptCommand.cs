using System;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Browsing.Commands
{
    public class SwitchProxyAndMarkInvalidScriptCommand : IScriptUtilityCommand
    {
        public Type InputType { get { return typeof(Uri); } }
        public Type ReturnType { get { return typeof(bool); } }

        public object Execute(ScriptUtility x, object input)
        {
            var uri = input as Uri;
            var browser = x.Exec<IBrowsingSession>("GetBrowsingSession") as StatefullBrowsingSessionWrapper;
            if (browser != null)
            {
                x.Exec("ClearCacheEntry", input);
            }
            return x.Exec<IBrowsingSession>("GetBrowsingSession").TrySwitchProxy(uri);
        }
    }
}
