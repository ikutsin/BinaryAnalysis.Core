using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Extensions.Browsing;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Common.Browsing
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void CorrectBrowsingSessionCheck()
        {
            var session = Container.Resolve<IBrowsingSession>();
            Assert.IsTrue(session is StatefullBrowsingSessionWrapper, "Wrong default session set");
        }
    }
}
