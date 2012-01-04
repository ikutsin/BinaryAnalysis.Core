using System;
using Autofac;
using BinaryAnalysis.Scheduler.Task.Flow;
using BinaryAnalysis.Scheduler.Task.Settings;

namespace BinaryAnalysis.Scheduler.Task.Script
{
    public interface ISchedulerTaskScript
    {
        void Init(
            SchedulerTaskScriptDependencies dependencies,
            ITaskSettings settings,
            ScriptFlow browsingFlow,
            ScriptUtility utils
            );

        Type DependencyClassType { get; }

        void Execute();
        ScriptFlow Flow { get; }
    }

    public abstract class AbstractTaskScript : ISchedulerTaskScript
    {
        public ITaskSettings Settings { get; protected set; }
        public ScriptFlow Flow { get; protected set; }
        public ScriptUtility x { get; protected set; }
        public SchedulerTaskScriptDependencies Dependencies { get; protected set; }

        public void Init(
            SchedulerTaskScriptDependencies dependencies,
            ITaskSettings settings,
            ScriptFlow flow,
            ScriptUtility utils
            )
        {
            this.x = utils;
            this.Settings = settings;
            this.Flow = flow;
            this.Dependencies = dependencies;
            if (dependencies!=null && dependencies.GetType() != DependencyClassType) throw new InvalidOperationException("Wrong dependency class provided");
        }

        public abstract void Execute();
        public abstract Type DependencyClassType { get; }
    }

    public class TaskScriptHandler : AbstractTaskScript
    {
        private readonly Action<TaskScriptHandler> executionHandler;
        private readonly Type dependencyClassType;
        public TaskScriptHandler(
            Action<TaskScriptHandler> executionHandler,
            Type dependencyClassType = null
            )
        {
            this.executionHandler = executionHandler;
            this.dependencyClassType = dependencyClassType;
        }

        public Action<TaskScriptHandler> ExecutionHandler { get { return executionHandler; } }
        public override Type DependencyClassType { get { return dependencyClassType; } }

        public override void Execute()
        {
            ExecutionHandler(this);
        }

    }
}
