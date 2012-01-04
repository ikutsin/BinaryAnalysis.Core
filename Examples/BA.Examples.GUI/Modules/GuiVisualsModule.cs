using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.UI.Commons;

namespace BA.Examples.GUI.Modules
{
    public class GuiVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Visual dependencies
            builder.RegisterType<GuiVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();
        }
    }
    public class GuiVisualDependencies : IVisualDependencies
    {
        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.test.Form()"},
                                       Path = "Test/Form", Weight = 0
                                   },
                           };
            }
        }

        public Dictionary<string, List<string>> Dependencies
        {
            get
            {  
                return new Dictionary<string, List<string>>
                           {
                               {"BinaryAnalysis.GUI.js", new List<string>
                                                        {
                                                            "Templates/FormTest.htm"
                                                        }},
                               {"Templates/Navigation.htm", new List<string>  {"BinaryAnalysis.GUI.js"}}
                           };
            }
        }
    }
}
