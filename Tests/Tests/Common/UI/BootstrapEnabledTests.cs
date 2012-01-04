using System;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Common.UI
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void UiGridQueryTest()
        {
            throw new NotImplementedException();
            //ContextExtensionsHolder extHolder = Container.Resolve<ContextExtensionsHolder>();
            //var dbContext = extHolder["db"] as DatabaseContextExtension;
            //var box = dbContext.getEntries(
            //    "BinaryAnalysis.Goals.WebQuerying.Data.FingerprintBoxMap, BinaryAnalysis.Goals.WebQuerying, Version=1.0.4273.42393, Culture=neutral, PublicKeyToken=null"
            //    ,"BinaryAnalysis.Goals.WebQuerying.Data.FingerprintEntity, BinaryAnalysis.Goals.WebQuerying, Version=1.0.4273.42393, Culture=neutral, PublicKeyToken=null"
            //    ,10,1,"",true,false,"");
        }
    }
}
