using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Extensions.Health;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Commands;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Modularity.Modules.Scheduler
{
    public class SchedulerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //dependencies
            builder.RegisterType<ScheduleRepository>().InstancePerDependency();
            builder.RegisterType<RecurrencyRepository>().InstancePerDependency();

            builder.RegisterType<PersistentSchedulerTask>().InstancePerDependency();

            builder.RegisterType<RecurrencyController>().SingleInstance();

            builder.RegisterType<ScriptUtilityCommands>().SingleInstance();
            builder.RegisterType<ScriptUtility>().InstancePerDependency();

            builder.RegisterType<RunGoalInlineScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "RunGoalInline"));

            //for single running
            builder.RegisterType<SchedulerTask>().InstancePerDependency();
            builder.RegisterType<TaskFactory>().AsSelf().SingleInstance();

            //config
            builder.RegisterType<NHBrowsingGoalConfig>().As<INHMappingsProvider>().SingleInstance();

            //scheduler instance
            builder.RegisterType<SchedulerInstance>().AsSelf().SingleInstance();

        }
    }
    public class NHBrowsingGoalConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(ScheduleEntityMap),
                    typeof(RecurrencyEntityMap),
                };
            }
        }
    }
}
