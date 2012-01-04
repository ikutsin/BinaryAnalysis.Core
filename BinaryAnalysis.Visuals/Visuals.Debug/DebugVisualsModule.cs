using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.UI.Commons;

namespace BinaryAnalysis.Visuals.Debug
{
    public class DebugVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Visual dependencies
            builder.RegisterType<DebugVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();
        }
    }
    public class DebugVisualDependencies : IVisualDependencies
    {
        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.debug.Evaler()"},
                                       Path = "Debug/Evaler", Weight = 0
                                   },
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.debug.EvalerWindow()"},
                                       Path = "Debug/Evaler Window", Weight = 0
                                   },
                                new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.debug.EntityList()"},
                                       Path = "Debug/EntityList", Weight = 0
                                   },
                                new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.debug.Taxonomy()"},
                                       Path = "Debug/Taxonomy", Weight = 0
                                   }
                           };
            }
        }

        public Dictionary<string, List<string>> Dependencies
        {
            get
            {  
                return new Dictionary<string, List<string>>
                           {
                               {"BinaryAnalysis.Debug.js", new List<string>
                                                        {
                                                            "jqwindow/jquery.window.min.js", 
                                                            "jqwindow/css/jquery.window.css",
                                                            "Templates/Debug.htm"
                                                        }},
                               {"Templates/Navigation.htm", new List<string>  {"BinaryAnalysis.Debug.js"}}
                           };
            }
        }
    }
}
