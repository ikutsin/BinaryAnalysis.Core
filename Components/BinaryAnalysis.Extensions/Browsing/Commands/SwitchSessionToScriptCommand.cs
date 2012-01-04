using System;
using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Browsing.Commands
{
    public class SwitchSessionToCommand : IScriptUtilityCommand
    {
        private readonly IComponentContext context;
        public Type InputType { get { return typeof(string); } }
        public Type ReturnType { get { return typeof(void); } }

        public SwitchSessionToCommand(IComponentContext context)
        {
            this.context = context;
        }

        public object Execute(ScriptUtility x, object input)
        {
            var name = input as string;
            var sess = context.ResolveNamed<IBrowsingSession>(name);
            if (sess == null) throw new Exception("Session name '" + name + "' is not found");

            var browser = x.Exec<IBrowsingSession>("GetBrowsingSession") as StatefullBrowsingSessionWrapper;
            if (browser != null)
            {
                browser.Parent = sess;
            }
            else
            {
                x.TaskBag.Set(GetBrowsingSessionScriptCommand.SETTING_BROWSING_SESSION, sess);
            }
            return null;
        }
    }
}
