using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Extensions.Dependencies
{
    public class ContainerLagacyDependencies : NoDependencies
    {
        public IComponentContext Context { get; set; }
    }
}
