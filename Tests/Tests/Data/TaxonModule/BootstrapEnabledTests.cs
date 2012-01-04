using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Classification;

namespace BinaryAnalysis.Tests.Data.TaxonModule
{
    [TestFixture]
    class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestTreeBrowsing()
        {
            var tree = Container.Resolve<TaxonomyTree>();
            var sq = tree.GetOrCreatePath("/Tags/SeoQuake/Google");
            Assert.AreEqual("/", tree.Root.Path);
            Assert.AreEqual("/Tags", tree.Root["Tags"].Path);
            Assert.AreEqual("/Tags/SeoQuake/Google", tree.Root["Tags"]["SeoQuake"]["Google"].Path);
            Assert.AreEqual("/Tags/SeoQuake/Google", sq.Path);
            Assert.AreEqual("/Tags/SeoQuake", tree.Root["Tags"]["SeoQuake"].Path);
        }

        [Test]
        public void TestRelationCrud()
        {
            var taxonRepo = Container.Resolve<TaxonRepository>();
            var taxon1 = new TaxonEntity();
            taxon1.Name = "Test1";
            taxonRepo.SaveOrUpdate(taxon1);

            var taxon2 = new TaxonEntity();
            taxon2.Name = "Test2";
            taxonRepo.SaveOrUpdate(taxon2);

            var taxon3 = new TaxonEntity();
            taxon3.Name = "Test3";
            taxonRepo.SaveOrUpdate(taxon3);

            var taxon4 = new TaxonEntity();
            taxon4.Name = "Test4";
            taxonRepo.SaveOrUpdate(taxon4);

            var relationService = Container.Resolve<RelationService>();
            //relationService.Relate(taxon1, taxon2, RelationDirection.Both);
            //relationService.Relate(taxon1, taxon2, RelationDirection.Forward);

            relationService.AddRelation(taxon1, taxon3, RelationDirection.Forward);
            relationService.AddRelation(taxon1, taxon3);

            //relationService.Relate(taxon3, taxon4, RelationDirection.Both);
        }
        [Test]
        public void TestTaxonCrud()
        {
            var taxonRepo = Container.Resolve<TaxonRepository>();
            var taxon = new TaxonEntity();
            taxon.Name = "aaa";
            taxonRepo.SaveOrUpdate(taxon);

            var persistentTaxon = taxonRepo.Get(taxon.Id);
            taxonRepo.Delete(taxon);

            var deletedTaxon = taxonRepo.Get(taxon.Id);
            Assert.IsNull(deletedTaxon);
        }
        [Test]
        public void TestTreeFind()
        {
            var tree = Container.Resolve<TaxonomyTree>();
            var rootParent = tree.Root.Parent;
            tree.Root.GetOrAddChild("test1","null");
            tree.Root.GetOrAddChild("test2", "null");
            tree.Root.GetOrAddChild("test3", "null");

            tree.Root["test1"].GetOrAddChild("to Remove", "null");
            tree.Root["test1"].GetOrAddChild("toRemove1", "null");
            tree.Root["test1"].GetOrAddChild("toRemove2", "null");


            var find1 = tree.Find("test1/*/");
            Assert.AreEqual(find1.Count, 3);
            tree.Root["test1"].Remove(true);

            find1 = tree.Find("test1/*/");
            Assert.AreEqual(find1.Count, 0);
        }
    }
}
