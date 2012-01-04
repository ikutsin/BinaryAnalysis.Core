using System;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Browsing.Commands
{
    public class ClearDomainCacheForScriptCommand : IScriptUtilityCommand
    {
        public Type InputType { get { return typeof(Uri); } }
        public Type ReturnType { get { return typeof(void); } }

        public object Execute(ScriptUtility x, object input)
        {
            var uri = input as Uri;
            var browser = x.Exec<IBrowsingSession>("GetBrowsingSession") as StatefullBrowsingSessionWrapper;
            if (browser != null)
            {
                browser.ClearDomainCacheFor(uri);
                x.Flow.AddMessage("State clean done for " + uri.Host);
            }
            else
            {
                throw new Exception("Not a StatefullBrowsingSession");
            }
            return null;
        }
    }
}
