using BinaryAnalysis.Modularity;
using NUnit.Framework;
using Autofac;

namespace BinaryAnalysis.Tests.Helpers
{
    public class TestBootstrapEnabledTests
    {
        Bootstrap bootstrap;
        public IContainer Container { get; private set; } 
        [SetUp]
        public void InitBootstrap()
        {
            bootstrap = new Bootstrap();
            Container = bootstrap.Container;
        }
        [TearDown]
        public void DisposeBootstrap()
        {
            Container.Dispose();
            bootstrap.Dispose();
            Container = null;
        }
    }
}
