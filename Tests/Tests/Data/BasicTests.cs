using BinaryAnalysis.Modularity;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;
using Autofac;

namespace BinaryAnalysis.Tests.Data
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void TestDbConnection()
        {
            using (var bootstrap = new Bootstrap())
            {
                var test = new TestEntity() { Name = "dbentry" };
                IContainer container = bootstrap.Container;
                var repo = container.Resolve<TestRepository>();
                repo.Save(test);
            }
        }
        [Test]
        public void TestUpdateFromAnotherContainer()
        {
            var test = new TestEntity() { Name = "zzz" };
            using (var bootstrap = new Bootstrap())
            {
                IContainer container = bootstrap.Container;
                var repo = container.Resolve<TestRepository>();
                repo.Save(test);
            }
            using (var bootstrap = new Bootstrap())
            {
                var container = bootstrap.Container;
                var repo = container.Resolve<TestRepository>();

                repo.Delete(test);

                test = new TestEntity() { Name = "zzz" };
                repo.Save(test);
            }
        }
    }
}
