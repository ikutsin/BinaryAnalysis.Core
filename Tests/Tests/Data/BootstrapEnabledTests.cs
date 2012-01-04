using System;
using Autofac;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Versioning;
using BinaryAnalysis.Tests.Helpers;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Data
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestRepoFinder()
        {
            var finder = Container.Resolve<RepositoryFinder>();
            Type clRepo = finder.CreateRepository<TestEntity>().GetType();
            Assert.AreEqual(clRepo, typeof(TrackableRepository<TestEntity>));
        }

        [Test]
        public void TestMultisession()
        {
            var testRepo = Container.Resolve<TestRepository>();
            var entity = new TestEntity() { Name = "test1", Name2 = "asd1" };
            testRepo.Save(entity);
            testRepo.Evict(entity);

            testRepo = Container.Resolve<TestRepository>();
            testRepo.Delete(entity);
            Assert.IsNull(testRepo.Get(entity.Id));

            testRepo = Container.Resolve<TestRepository>();
            Assert.IsNull(testRepo.Get(entity.Id));
        }
    }
}
