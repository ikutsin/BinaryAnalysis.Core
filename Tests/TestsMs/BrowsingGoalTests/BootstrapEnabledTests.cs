using BinaryAnalysis.TestsMs.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinaryAnalysis.TestsMs.BrowsingGoalTests
{
    [TestClass]
    public class BootstrapEnabledTests : BootstrapEnabledTestsBase
    {
        //[TestMethod]
        //[HostType("Moles")]
        public void ParallelSessionProxyChangeTest()
        {
            //var workersList = new List<Tuple<int, IBrowsingProxy, IBrowsingProxy>>();
            //var mole = new MStatefullBrowsingSessionWrapper()
            //               {
            //                   NavigateGetUri =
            //                    (_) =>
            //                        {
            //                            throw new Exception();
            //                            return null;
            //                        },
            //                   TrySwitchProxyUri =
            //                    (_) =>
            //                        {
            //                            return TrySwitchProxy
            //                            return new 
            //                        },
            //                    ClearCacheEntryUri = 
            //                    (_)=>
            //                        {

            //                        },
            //                   InstanceBehavior = MoleBehaviors.Fallthrough
            //               };

            //SessionSyncGoal goal = Container.Resolve<SessionSyncGoal>();
            //goal.BindMole(mole);
            //goal.Execute("testingSync", new SettingsEntity(), Container);
        }

    }
}
