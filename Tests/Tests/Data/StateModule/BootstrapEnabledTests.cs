using System.Linq;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.State;
using System.Threading;
using Autofac;

namespace BinaryAnalysis.Tests.Data.StateModule
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestStateTimeout()
        {
            var tree = Container.Resolve<TaxonomyTree>();
            var stateRepo = Container.Resolve<StateRepository>();

            tree.Root.GetOrAddChild("Tests", "test");

            var trigger = tree.Find("Tests").First().GetOrAddChild("myStateTrigger", "test");
            stateRepo.CreateAndPersist("test", "test", null, 1);
            stateRepo.CreateAndPersist("tes3t", "test", null, 1);
            var state = stateRepo.GetState("test");
            Thread.Sleep(1500);
            Assert.IsNull(stateRepo.GetState("test"));

            state = stateRepo.CreateAndPersist("testTrigger", "testTrigger", trigger);

            stateRepo = Container.Resolve<StateRepository>();
            state = stateRepo.GetState("testTrigger");
            stateRepo.Trigger(trigger);
            Assert.IsNull(stateRepo.GetState("testTrigger"));
        }
    }
}
