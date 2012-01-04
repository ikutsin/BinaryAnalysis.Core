using System;
using System.Linq;
using BinaryAnalysis.Browsing.Windowless.Decorators;
using BinaryAnalysis.Browsing.Windowless.Proxies;
using NUnit.Framework;
using BinaryAnalysis.Browsing.Windowless;
using System.IO;
using BinaryAnalysis.Browsing.Local;
using BinaryAnalysis.Browsing.Extensions;

namespace BinaryAnalysis.Tests.Common.Browsing
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void SimpleLocalFileTest()
        {
            var inFile = "_data/usanov-net.txt";
            var session = new LocalFilesBrowser();
            var resp = session.LoadFile(inFile);

            var links = resp.ExtractHyperlinks();
            Assert.IsTrue(links.Count() > 0);
        }
        [Test]
        public void TestLocalFileNavigation()
        {
            var inFile = @"D:\_\NUnit-2.5.2.9222\NUnitFitTests.html";
            var outFile = @"c:\out.txt";
            if (File.Exists(outFile)) File.Delete(outFile);

            var session = new LocalFilesBrowser();
            var resp = session.LoadFile(inFile);
            //resp.SaveToFile(outFile);
        }
        [Test]
        public void TestNavigateGet()
        {
            var session = new BrowsingSession();
            session.CurrentProxy = new DirectBrowsing();

            using (var resp = session.NavigateGet(new Uri("http://www.binaryanalysis.org")))
            {
                Console.WriteLine(resp.StatusCode);
                Console.WriteLine(resp.ResponseContent);
                Console.WriteLine(resp.ResponseContent);

                Console.WriteLine(resp.Headers);
            }
        }
        [Test]
        public void TestNavigateReferrer()
        {
            var session = new BrowsingSession();
            session.CurrentProxy = new DirectBrowsing();

            session.AddDecorator(new AutoRefererDecorator());
            session.AddDecorator(new CommonUserAgentDecorator());
            session.NavigateGet(new Uri("http://www.binaryanalysis.org/asdfadsf"));
            session.NavigateGet(new Uri("http://www.binaryanalysis.org/yetrysdf"));
        }
    }
}
