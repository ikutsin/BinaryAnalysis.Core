using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.ScriptingHelper.BrowserContext;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;

namespace BinaryAnalysis.ScriptingHelper
{
    public class ScriptingHelperVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ScriptingHelperContextExtension>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "scriptingHelper"));

            //Visual dependencies
            builder.RegisterType<ScriptingHelperVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();
        }
    }
    public class ScriptingHelperVisualDependencies : IVisualDependencies
    {
        #region Implementation of IVisualDependencies

        public Dictionary<string, List<string>> Dependencies
        {
            get
            {
                return new Dictionary<string, List<string>>
                           {
                               {"Templates/Navigation.htm", new List<string> {"BinaryAnalysis.ScriptingHelper.js"}}
                           };
            }
        }

        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.scriptingHelper.startStandalone()"},
                                       Path = "Debug/Scripter", Weight = 0
                                   },
                           };                
            }
        }

        #endregion
    }
}
