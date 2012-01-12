using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Core;
using BinaryAnalysis.Extensions.Health;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Scheduler.Task.Settings;
using BinaryAnalysis.Tests.Helpers;
using BinaryAnalysis.Tests.Helpers.Scripts;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Common.Schedulder
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void NamedListResolvingTest()
        {
            var ctx = Container.Resolve<IComponentContext>();
            var taskNames = ctx.ComponentRegistry.Registrations
                .Select(c => c.Services.FirstOrDefault() as KeyedService)
                .Where(s => s != null && s.ServiceType == typeof(TaskParameters))
                .Select(s => s.ServiceKey).ToList();

            Assert.IsTrue(taskNames.Count>0, "No task parameters");
        }

        [Test]
        public void SimpleRunTest()
        {
            var factory = Container.Resolve<TaskFactory>();
            var task = factory.InitTaskFromContainer("do-nothing");
            factory.RunTaskUntilFinished(task);
        }

        [Test]
        public void ScriptDependenciesResolvingTest()
        {
            var ctxDep = Container.Resolve<TestDependenciesInContext>();
            var task = new SchedulerTaskOverride();
            task.TestSettings = new DetachedTaskSettings();
            task.TestSettings.Set("setting_test", new object());

            var inctxScripts = Container.ResolveNamed<TaskParameters>("dep-incontext").Scripts;
            var outctxScript = Container.ResolveNamed<TaskParameters>("dep-outcontext").Scripts;

            //Context based resolving
            var inctxDep = task.TestDependency(inctxScripts.First().Value, Container);
            Assert.AreEqual(inctxDep, ctxDep);
            Assert.IsNotNull((inctxDep as TestDependenciesInContext).TestRepo);

            //new type resolving - properties wiring
            var outctxDep = task.TestDependency(outctxScript.First().Value, Container);
            Assert.IsNotNull((outctxDep as TestDependencies).TestRepo);

            try
            {
                task.TestSettings = new DetachedTaskSettings();
                task.TestDependency(outctxScript.First().Value, Container);
                Assert.Fail("Param resolving is not working");
            }
            catch (AssertionException e)
            {
                throw e;
            }
            catch
            {
                
            }
        }

        [Test]
        public void RunTestTaskTest()
        {
            TaskFactory taskFactory = Container.Resolve<TaskFactory>();
            var task = taskFactory.InitTaskFromContainer("sleep-1000");
            taskFactory.RunTaskUntilFinished(task);
        }

        [Test]
        public void RunParallelTaskTest()
        {
            var repo = Container.Resolve<ScheduleRepository>();
            repo.DeleteAll(repo.GetAll());

            bool isFinished = false;
            SchedulerInstance scheduler = SchedulerInstance.Instance;
            if (SchedulerInstance.Instance == null) scheduler = Container.Resolve<SchedulerInstance>();
            scheduler.TaskFinished += (t) =>
            {
                isFinished = true;
            };

            Container.Resolve<SchedulerServiceTracker>().Start();

            scheduler.Start(Container);
            scheduler.Schedule("sleep-5000", ScheduleTaskParallelism.AllowAll);
            scheduler.Schedule("sleep-5000", ScheduleTaskParallelism.AllowAll);
            scheduler.Schedule("sleep-5000", ScheduleTaskParallelism.AllowAll);
            scheduler.Schedule("sleep-5000", ScheduleTaskParallelism.AllowAll);

            var cnt = 20;
            while (!isFinished && cnt-- >= 0)
            {
                Thread.Sleep(500);
            }
            scheduler.Stop();
            scheduler.Destroy();
            if (cnt == 0)
            {
                Assert.Fail("Service didn't finish in reasonable amount of time");
            }
        }

        [Test]
        public void RunSchedulerTaskTest()
        {
            var repo = Container.Resolve<ScheduleRepository>();
            repo.DeleteAll(repo.GetAll());

            bool isFinished = false;
            SchedulerInstance scheduler = SchedulerInstance.Instance;
            if (SchedulerInstance.Instance == null) scheduler = Container.Resolve<SchedulerInstance>();
            scheduler.TaskFinished += (t) =>
                                          {
                                              isFinished = true;
                                          };
            Container.Resolve<SchedulerServiceTracker>().Start(); 
            scheduler.Start(Container);
            scheduler.Schedule("sleep-5000");

            var cnt = 20;
            while (!isFinished && cnt-->=0)
            {
                Thread.Sleep(500);
            }
            scheduler.Stop();
            scheduler.Destroy();
            if (cnt == 0)
            {
                Assert.Fail("Service didn't finish in reasonable amount of time");
            }
        }

        [Test]
        public void FactoryTaskCustomEventTest()
        {
            TaskFactory taskFactory = Container.Resolve<TaskFactory>();
            var task = taskFactory.InitTaskFromContainer("cusomEvent-fire");
            bool isFired = false;
            task.ScriptCustomEvent += (t, s, i) =>
            {
                isFired = i == task.Settings.Get("setting_message");
            };
            taskFactory.RunTaskUntilFinished(task);
            Assert.IsTrue(isFired, "Event has not been fired");
        }

        [Test]
        public void SchedulerCustomEventTest()
        {
            var repo = Container.Resolve<ScheduleRepository>();
            repo.DeleteAll(repo.GetAll());

            bool isFinished = false;
            bool isFired = false;
            SchedulerInstance scheduler = SchedulerInstance.Instance;
            if (scheduler == null) scheduler = Container.Resolve<SchedulerInstance>();
            scheduler.TaskFinished += (t) =>
            {
                isFinished = true;
            };
            scheduler.ScriptCustomEvent += (t, s, i) =>
            {
                isFired = true;
            };
            Container.Resolve<SchedulerServiceTracker>().Start();
            scheduler.Start(Container);
            scheduler.Schedule("cusomEvent-fire");

            var cnt = 20;
            while (!isFinished && cnt-- >= 0)
            {
                Thread.Sleep(500);
            }
            scheduler.Stop();
            scheduler.Destroy();
            if (cnt == 0)
            {
                Assert.Fail("Service didn't finish in reasonable amount of time");
            }
            Assert.IsTrue(isFired, "Event has not been fired");
        }
    }
}
