using Autofac;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Commands;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Modularity.Modules.Scheduler
{
    public class TaskFactoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ScriptUtilityCommands>().SingleInstance();
            builder.RegisterType<ScriptUtility>().InstancePerDependency();

            builder.RegisterType<SchedulerTask>().InstancePerDependency();
            builder.RegisterType<TaskFactory>().AsSelf().SingleInstance();

            builder.RegisterType<RunGoalInlineScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "RunGoalInline"));

        }
    }
}
