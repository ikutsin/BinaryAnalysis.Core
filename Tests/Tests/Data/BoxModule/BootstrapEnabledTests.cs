using System.Linq;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;
using Autofac;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.Tests.Data.BoxModule
{
    [TestFixture]
    class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void QueryBoxLocalTest()
        {
            var box = Container.Resolve<BoxQuery<HttpProxyBoxMap>>().AsQueryable()
                .Where(x => x.Country == "zz");

            Assert.AreEqual(box.Take(20).Count(), 0);
            Assert.IsNull(box.Take(20).FirstOrDefault());
            Assert.AreEqual(box.Take(20).ToList().Count, 0);

            var repo = Container.Resolve<HttpProxyRepository>();
            repo.Save(new HttpProxyEntity()
            {
                Comment = "zzz",
                IP = ("http://usanov.net")
            });
        }

        [Test]
        public void QueryBoxToTakeListCountTest()
        {
            var box = Container.Resolve<BoxQuery<HttpProxyBoxMap>>().AsQueryable()
                .Where(x => x.Country == "zz");
            
            Assert.AreEqual(box.Take(20).Count(), 0);
            Assert.IsNull(box.Take(20).FirstOrDefault());
            Assert.AreEqual(box.Take(20).ToList().Count, 0);

            var repo = Container.Resolve<HttpProxyRepository>();
            repo.Save(new HttpProxyEntity()
            {
                Comment = "zzz", IP = ("http://usanov.net")
            });

            box = new BoxQuery<HttpProxyBoxMap>()
                .Where(x => x.Comment == "zzz");
            Assert.IsTrue(box.Count()>= 1);
            Assert.IsTrue(box.Take(20).Count()>= 1);
            Assert.IsNotNull(box.Take(20).FirstOrDefault());
            Assert.IsTrue(box.Take(20).ToList().Count>= 1);
        }

        [Test]
        public void BoxOfSettingsEntityTest()
        {
            IContainer container = Container;
            var repo = container.Resolve<SettingsRepository>();
            var s = repo.Get(1);
            if (s == null)
            {
                s = new SettingsEntity() { Name = "testsetting" };
                s.AddEntry("asd","ddd");
                repo.Save(s);
            }

            var transformer = container.Resolve<NHibernateBoxTransformation<SettingsBoxMap, SettingsEntity>>();
            transformer.Entries = new[] {s};
            var boxed = transformer.ToBox();
            var xmlBoxer = new XmlBoxPresentation<SettingsBoxMap>();
            var ss = xmlBoxer.AsString(boxed);
            var box = xmlBoxer.FromString(ss);
            Assert.IsTrue(box.First().Name == s.Name);
            Assert.AreEqual(s.Entries.First().Name, box.First().Entries.First().Name);
        }
    }
}
