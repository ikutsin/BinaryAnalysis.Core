using System.Collections.Generic;
using BinaryAnalysis.Extensions.Dependencies;
using BinaryAnalysis.Tests.Helpers.Entities;

namespace BinaryAnalysis.Tests.Helpers.Scripts
{
    class TestDependenciesInContext : NoDependencies
    {
        public TestRepository TestRepo { get; set; }
    }
    class TestDependencies : ContainerLagacyDependencies
    {
        public TestRepository TestRepo { get; set; }
        public override IEnumerable<string> RequiredSettings
        {
            get { return new[] {"setting_test"}; }
        }
    }
}
