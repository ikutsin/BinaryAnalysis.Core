using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using BA.Examples.Console.Data;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BA.Examples.Console.Modules
{
    public class TestScriptsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HQLQueryAction>().InstancePerDependency();
        }
    }
}
