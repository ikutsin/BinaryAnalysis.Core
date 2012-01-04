using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinaryAnalysis.Modularity;
using BinaryAnalysis.Tests.Helpers.Entities;
using Ciloci.Flee;
using NUnit.Framework;
using Jint;
using Autofac;
using BinaryAnalysis.Data.Box;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace BinaryAnalysis.Tests.Common
{
    [TestFixture]
    public class RedistTests
    {
        [Test]
        public void AutofacConfigListTest()
        {
            using (var bootstrap = new Bootstrap())
            {
                IContainer container = bootstrap.Container;
                var listc = container.Resolve<TestListInput>();

                Assert.AreEqual(3, listc.PropTest.Count);
                Assert.AreEqual(3, listc.CtorPropTest.Count);
            }
            
        }

        [Test]
        public void ThreadNamesTest()
        {
            var rnd = new Random();
            var range = Enumerable.Range(0, 50);

            var rangeBack = new Dictionary<int, int>();
            var rangeBackLocker = new object();

            range.AsParallel().AsOrdered()
                .WithDegreeOfParallelism(4).ForAll(x =>
            {
                lock (rangeBackLocker)
                {
                    rangeBack.Add(x, Thread.CurrentThread.ManagedThreadId);
                }
                Thread.Sleep(100 * rnd.Next(10));         
            });

            Assert.AreEqual(rangeBack.GroupBy(x=>x.Value).Count(), 4);
        }

        [Test]
        public void ExpresionBuilderTest()
        {
            using (var bootstrap = new Bootstrap())
            {
                IContainer container = bootstrap.Container;
                var eval = container.Resolve<NHibernateBoxQueryEvaluator>();

                var query = new BoxQuery<TestEntity>();
                var el = query.Where(x => x.Name!="test").SerializeQuery();

                //var e = eval.EvaluateList(el);
                //var maps = (IList<TestEntityBoxMap>)e.MappedList;
                //query.Execute(e.Expression);
            }
        }

        [Test]
        public void JintTest()
        {
            string script = @"
return this;
 name = '209.97.203.60';
 port1 = 1312;
 port2 = 3749;
 port3 = 8691;
 port4 = 9215;
 port5 = 7419;
 port6 = 9777;
 port7 = 8194;
 port8 = 8864;
 port9 = 6929;
 port10 = 8378;
 WQGYJD = port9 + (101629-341) / 88;
 return(name + ':' + WQGYJD);
";
            JintEngine context = new JintEngine();
            var result = context.Run(script);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ExpressionEvaluationTest()
        {
            ExpressionContext context = new ExpressionContext();
            context.Variables.Add("rand", new Random());

            IDynamicExpression e = context.CompileDynamic("rand.nextDouble() + 100");
            double result = (double)e.Evaluate();

            //no LINQ
            //var doc = new XDocument(new XElement("test", 
            //    new XElement("val", "result1"),
            //    new XElement("val", "result2"),
            //    new XElement("val", "result3")
            //    ));
            //context.Variables.Add("doc", doc);

            //e = context.CompileDynamic("doc.Elements().First().Value");
            //var r = e.Evaluate();

            //no Dynamic
            //dynamic expando = new ExpandoObject();
            //expando.test = "Passed";
            //context.Variables.Add("expando", expando);
            //e = context.CompileDynamic("expando.test");
            //var r = e.Evaluate();

        }
    }


    public class TestListInput
    {
        public TestListInput(IList<string> recurrencyList)
        {
            CtorPropTest = recurrencyList;
        }

        public IList<string> PropTest { get; set; }
        public IList<string> CtorPropTest { get; set; }
    
    }
}
