using BinaryAnalysis.Tests.Helpers;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;
using Autofac;
using BinaryAnalysis.Data.Index;

namespace BinaryAnalysis.Tests.Data.IndexModuleTest
{
    [TestFixture]
    class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestIndexQuery()
        {
            var testRepo = Container.Resolve<IndexableTestRepository>();
            var indexRepo = Container.Resolve<IndexRepository>();

            testRepo.DeleteAll(testRepo.GetAll());

            var r = indexRepo.RawQuery<IndexableTestEntity>("Name", "te"); 
            Assert.AreEqual(r.Count, 0);

            testRepo.SaveOrUpdate(new IndexableTestEntity() { Name = "te st1", Name2 = "asd1" });

            r = indexRepo.RawQuery<IndexableTestEntity>("Name", "te");
            Assert.IsTrue(r.Count>=1);
        }
    }
}
