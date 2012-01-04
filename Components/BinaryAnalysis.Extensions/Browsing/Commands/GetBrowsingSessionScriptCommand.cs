using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Browsing.Commands
{
    public class GetBrowsingSessionScriptCommand : IScriptUtilityCommand
    {
        public const string SETTING_BROWSING_SESSION = "BrowserWL";
        private readonly IComponentContext _context;

        public GetBrowsingSessionScriptCommand(IComponentContext context)
        {
            _context = context;
        }

        #region Implementation of IScriptUtilityCommand

        public Type InputType { get { return typeof (void); } }
        public Type ReturnType { get { return typeof (IBrowsingSession); } }

        public object Execute(ScriptUtility x, object input)
        {
            if(!x.TaskBag.Has(SETTING_BROWSING_SESSION))
            {
                x.TaskBag.Set(SETTING_BROWSING_SESSION, _context.Resolve<IBrowsingSession>());
            }
            return x.TaskBag.Get<IBrowsingSession>(SETTING_BROWSING_SESSION);
        }

        #endregion
    }
}
