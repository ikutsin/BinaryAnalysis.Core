using System;
using BinaryAnalysis.Tests.Helpers;
using BinaryAnalysis.Tests.Helpers.Entities;
using NUnit.Framework;
using BinaryAnalysis.Data.Settings;
using Autofac;

namespace BinaryAnalysis.Tests.Data.SettingsModule
{
    [TestFixture]
    public class BootstrapEnabledTests : TestBootstrapEnabledTests
    {
        [Test]
        public void TestSettingsCRUD()
        {
            var testRepo = Container.Resolve<TestWithSettingRepository>();
            var e = new TestWithSettingEntity();

            testRepo.Save(e);

            e = testRepo.Get(e.Id);
            Assert.IsTrue(e.Settings != null);

            e = testRepo.Load(e.Id);
            Assert.IsTrue(e.Settings != null);

            e.Settings.SetEntry("test", "test");
            e.Str = "ss";
            testRepo.Update(e);
            e = testRepo.Load(e.Id);
            Assert.IsTrue(e.Settings["test"].GetValue().ToString() == "test");

            e.Settings.SetEntry("test", "test3");
            //e.Str = e.Str;
            testRepo.Update(e);
            e = testRepo.Load(e.Id);
            Assert.IsTrue(e.Settings["test"].GetValue().ToString() == "test3");


            testRepo.Delete(e);

            e = new TestWithSettingEntity();
            e.Settings = new SettingsEntity();
            e.Settings.SetEntry("test", "test2");
            testRepo.SaveOrUpdate(e);
            testRepo.Evict(e);
            e = testRepo.Get(e.Id);
            Assert.IsTrue(e.Settings["test"].GetValue().ToString() == "test2");
        }

        [Test]
        public void TestSettingsSaveOrUpdate()
        {
            var testRepo = Container.Resolve<TestWithSettingRepository>();

            var e = new TestWithSettingEntity();
            testRepo.Save(e);
            e = testRepo.Get(e.Id);
            e = testRepo.Load(e.Id);
            e.Settings.SetEntry("lol", "lol");
            testRepo.Update(e);
            e = testRepo.Get(e.Id);
            Assert.AreEqual(e.Settings.Entries.Count, 1);
        }

        [Test]
        public void TestSettingsValidation()
        {
            var settRepo = Container.Resolve<SettingsRepository>();
            var isValid = settRepo.IsValid(new SettingsEntity());
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestSettingsProperties()
        {
            var settingsRepo = Container.Resolve<SettingsRepository>();
            var entryRepo = Container.Resolve<SettingsEntryRepository>();

            var datetime = DateTime.Now;

            var settings = new SettingsEntity("testSettingProps");
            settings.SetEntry("str", "stringvalue");
            settings.SetEntry("int", 10);
            settings.SetEntry("double", 10.4);
            settings.SetEntry("bit", true);
            settings.SetEntry("date", datetime);
            settingsRepo.SaveOrUpdate(settings);

            var e = entryRepo.GetEntryByKey(settings, "str");
            settings = settingsRepo.Load(settings.Id);
            Assert.IsNotNull(settings);

            var date = settings["date"].GetValue<DateTime>();
            Assert.AreEqual(datetime.ToString(), date.ToString());
        }
    }
}
