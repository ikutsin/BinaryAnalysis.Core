using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data;
using log4net;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    public class TestWithSettingEntity : Entity, ISettingsHolder
    {
        public const string OBJECT_NAME = "TestWithSetting";
        public virtual string Str { get; set; }
        public virtual string ObjectName
        {
            get { return OBJECT_NAME; }
        }
        public virtual SettingsEntity Settings { get; set; }
    }
    public class TestWithSettingRepository : SettingsHolderRepository<TestWithSettingEntity>
    {
        public TestWithSettingRepository(IDbContext context, ILog log,
            SettingsService settingsService, RelationService relRepo)
            : base(context, log, settingsService, relRepo) { }
    }
    public class TestWithSettingEntityMap : EntityClassMap<TestWithSettingEntity>
    {
        public TestWithSettingEntityMap()
        {
            Map(x => x.Str);
        }
    }
}
