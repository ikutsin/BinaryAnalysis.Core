using System;
using System.Collections.Generic;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;
using Autofac;

namespace BinaryAnalysis.Tests.Data.MetricsModule
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestMetricsCRUD()
        {
            var repo = Container.Resolve<MetricsRepository>();
            var m = new MetricsEntity() {Name = "Test"};
            repo.Save(m);
            Assert.IsTrue(m.Id>0);

            m.AddEntry(1);
            m.AddEntry(2);
            m.AddEntry(3);
            repo.SaveOrUpdate(m);

            var mm = repo.Load(m.Id);
            Assert.AreEqual(mm.Id, m.Id);
            Assert.AreEqual(3, mm.Entries.Count);
        }


        [Test]
        public void TestMetricsRelationsCRUD()
        {
            var service = Container.Resolve<MetricsService>();
            var tree = Container.Resolve<TaxonomyTree>();
            var node = tree.GetOrCreatePath(@"BinaryAnalysis.Tests/MetricsModule");

            var m = service.GetOrCreateFor(node, "Test");
            m.AddEntry(1);
            service.UpdateMetrics(m);

            var ms = service.GetFor(node);
            Assert.AreEqual(1, ms.Count);

            service.DeleteFor(node);
            ms = service.GetFor(node);
            Assert.AreEqual(0, ms.Count);
        }
    }
}
