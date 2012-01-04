using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;
using BinaryAnalysis.Visuals.Annotations.BrowserContext;

namespace BinaryAnalysis.Visuals.Annotations
{
    public class AnnotationVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetricsContextExtension>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "metrics"));

            builder.RegisterType<SettingsContextExtension>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "settings"));

            //Visual dependencies
            builder.RegisterType<AnnotationVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();
        }
    }
    public class AnnotationVisualDependencies : IVisualDependencies
    {
        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                                new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.annotations.Health()"},
                                       Path = "File/Health", Weight = 0
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
                               {
                                   "BinaryAnalysis.Annotations.js",
                                   new List<string>
                                       {
                                           "Templates/MetricsGraphs.htm",
                                           "Templates/HealthGraphs.htm",
                                       }
                                   },
                               {
                                   "Templates/Navigation.htm",
                                   new List<string>
                                       {
                                           "BinaryAnalysis.Annotations.js",
                                       }
                                   }
                           };
            }
        }
    }

}
