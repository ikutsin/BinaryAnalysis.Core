/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 12.02.2011
 * Time: 22:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using BinaryAnalysis.Modularity;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;
using Autofac;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data;

namespace BinaryAnalysis.Tests.Common
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	[TestFixture]
	public class CommonTests
	{
        [Test]
        public void ContextReloadTest()
        {
            using (var bootstrap = new Bootstrap())
            {
                var container = bootstrap.Container;
                var tree = container.Resolve<TaxonomyTree>();
                tree.Root.GetOrAddChild("zzz","");
                tree.Root.GetOrAddChild("zzz2",null);

                container = bootstrap.Container;
                tree = container.Resolve<TaxonomyTree>();
                tree.Root.GetOrAddChild("rrr",null);
                tree.Root["zzz"].Remove();
                tree.Root["zzz2"].Remove();
                tree.Root.GetOrAddChild("zzz",null);
            }

            Action<IContainer> testRel = (container) =>
            {
                var rel = container.Resolve<RelationService>();
                var tree = container.Resolve<TaxonomyTree>();
                if (tree.Find("zzz1").Count == 0)
                {
                    tree.Root.GetOrAddChild("zzz", "zzz");
                    tree.Root.GetOrAddChild("zzz1", "zzz1");

                    rel.AddRelation(tree.Root["zzz"], tree.Root["zzz1"], RelationDirection.Both);
                }
                else
                {
                    tree.Root["zzz1"].Remove();
                    tree.Root.AddChild("zzz2", null);
                    rel.AddRelation(tree.Root["zzz"], tree.Root["zzz2"], RelationDirection.Both);
                }
            };
            using (var bootstrap = new Bootstrap())
            {
                testRel(bootstrap.Container);
            }
        }
		[Test]
		public void BootstrapTest() {
            using (var bootStrap = new Bootstrap())
            {
                IContainer container = bootStrap.Container;
                Assert.IsNotNull(container);

                var testRepo = container.Resolve<TestRepository>();
                testRepo.Test();
            }
		}
	}
}