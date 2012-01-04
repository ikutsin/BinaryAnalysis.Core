using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;
using BinaryAnalysis.Visuals.Scheduler.BrowserContext;

namespace BinaryAnalysis.Visuals.Scheduler
{
    public class SchedulerVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SchedulerContextExtension>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "scheduler"));

            //Visual dependencies
            builder.RegisterType<SchedulerVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();
        }
    }
    public class SchedulerVisualDependencies : IVisualDependencies
    {
        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.scheduler.Available()"},
                                       Path = "Debug/Schedules", Weight = 0
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
                               {"BinaryAnalysis.Scheduler.js", new List<string>
                                                        {
                                                            "Templates/ScheduleSettings.htm",
                                                            "Templates/AvailableSchedules.htm"
                                                        }},
                               {"Templates/Navigation.htm", new List<string>  {"BinaryAnalysis.Scheduler.js"}}
                           };
            }
        }
    }
}
