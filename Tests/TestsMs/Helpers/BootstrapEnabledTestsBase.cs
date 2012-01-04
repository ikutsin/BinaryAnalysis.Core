using Autofac;
using BinaryAnalysis.Modularity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinaryAnalysis.TestsMs.Helpers
{
    [TestClass]
    public class BootstrapEnabledTestsBase
    {
        Bootstrap bootstrap;
        public IContainer Container { get; private set; }
        [TestInitialize]
        public void InitBootstrap()
        {
            bootstrap = new Bootstrap();
            Container = bootstrap.Container;
        }
        [TestCleanup]
        public void DisposeBootstrap()
        {
            Container.Dispose();
            bootstrap.Dispose();
            Container = null;
        }
    }
}
