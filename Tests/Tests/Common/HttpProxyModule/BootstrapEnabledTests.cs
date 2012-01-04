using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.Tests.Helpers;
using NUnit.Framework;
using Autofac;

namespace BinaryAnalysis.Tests.Common.HttpProxyModule
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void HttpProxyBackupRestoreWithDeleteTest()
        {
            var backup = Container.Resolve<HttpProxyFileBackup>();

            var repo = Container.Resolve<HttpProxyRepository>();
            if (repo.GetAll().Count == 0)
            {
                repo.Save(new HttpProxyEntity() {IP = ("http://111.111.111.111")});
            }

            backup.Backup();

            var proxise = repo.GetAll();
            Assert.IsTrue(proxise.Count>0);
            var cnt = proxise.Count;

            foreach (var httpProxyEntity in proxise)
            {
                repo.Delete(httpProxyEntity);
            }

            backup.Restore();
            proxise = repo.GetAll();
            Assert.AreEqual(cnt, proxise.Count);
        }
        [Test]
        public void HttpProxyBackupRestoreTest()
        {
            var backup = Container.Resolve<HttpProxyFileBackup>();
            backup.DbTransform.ImportStrategy = BoxImporterStrategy.RewriteExisting;

            var repo = Container.Resolve<HttpProxyRepository>();
            if (repo.GetAll().Count == 0)
            {
                repo.Save(new HttpProxyEntity() { IP = ("http://111.111.111.111") });
            }

            backup.Backup();

            var proxise = repo.GetAll();
            Assert.IsTrue(proxise.Count > 0);
            var cnt = proxise.Count;

            backup.Restore();
            proxise = repo.GetAll();
            Assert.AreEqual(cnt, proxise.Count);
        }
    }
}
