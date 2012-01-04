using BinaryAnalysis.Tests.Helpers;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;
using Autofac;

namespace BinaryAnalysis.Tests.Data.TrackingModule
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TrackingTest()
        {
            var testRepo = Container.Resolve<TestRepository>();
            testRepo.DeleteAll(testRepo.GetAll());
            var entity = new TestEntity() { Name = "test1", Name2 = "asd1" };
            testRepo.Save(entity);

            var changes = testRepo.GetChanges(entity);
            Assert.IsTrue(changes.Count == 0);

            entity.Name = "test2";
            testRepo.SaveOrUpdate(entity);
            entity = testRepo.Get(entity.Id);
            entity.Name = "test3";
            entity.Name2 = "asd3";
            testRepo.SaveOrUpdate(entity);

            changes = testRepo.GetChanges(entity);
            Assert.AreEqual(changes.Count, 2);
        }
    }
}
