using NUnit.Framework;
using RazorEngine;

namespace BinaryAnalysis.Tests.Common
{
    [TestFixture]
    public class RazorTests
    {
        [SetUp]
        public void Heatup()
        {
            //HeatupRazorEngine
            //http://razorengine.codeplex.com/discussions/242605
            bool loaded = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly != null;
        }
        [Test]
        public void SimpleTemplateTest()
        {
            string template = "Hello @Model.Name! Welcome to Razor!";
            string result = Razor.Parse(template, new {Name = "World"});
            Assert.IsTrue(result.StartsWith("Hello World"));
        }

        [Test]
        public void InlineHelperTemplateTest()
        {            
            string template =
@"@helper MyMethod(string name) {
    Hello @name
};
@MyMethod(Model.Name);! Welcome to Razor!";
            string result = Razor.Parse(template, new { Name = "World" });
            Assert.IsTrue(result.Contains("Hello World"));
        }

    }
}
