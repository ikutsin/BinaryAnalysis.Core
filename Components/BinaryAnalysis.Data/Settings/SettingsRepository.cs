using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using log4net;
using BinaryAnalysis.Data.Classification;

namespace BinaryAnalysis.Data.Settings
{
    public class SettingsRepository : ClassifiableRepository<SettingsEntity>
    {
        public const string SETTINGS_PATH = @"BinaryAnalysis.Core/SettingsRelation";

        RelationService relationService;
        TaxonomyNode settingsRelation;

        public SettingsRepository(
            IDbContext context,
            TaxonomyTree taxonomyTree,
            RelationService relationService,
            ILog log)
            : base(context, log, relationService) 
        {
            settingsRelation = taxonomyTree.GetOrCreatePath(SETTINGS_PATH, "SettingsRepository relations");
            this.relationService = relationService;
        }

        public SettingsEntity GetOrCreate(IClassifiable entity)
        {
            var dbSetting = GetOneFor(entity);
            if (dbSetting != null) return dbSetting;
            return new SettingsEntity() { Name = "For " + entity };
        }

        protected SettingsEntity GetOneFor(IClassifiable entity)
        {
            return relationService.GetRelated<SettingsEntity>(entity, RelationDirection.Forward, settingsRelation).FirstOrDefault();
        }

        public void DeleteFor(IClassifiable entity)
        {
            var dbSetting = GetOneFor(entity);
            if (dbSetting != null) Delete(dbSetting);
        }

        //public override SettingsEntity Update(SettingsEntity entity)
        //{
        //    return base.Update(entity);
        //}
        //public override SettingsEntity Save(SettingsEntity entity)
        //{
        //    return base.Save(entity);
        //}
        //public override SettingsEntity SaveOrUpdate(SettingsEntity entity)
        //{
        //    return base.SaveOrUpdate(entity);
        //}

        public void SaveOrUpdateFor(IClassifiable entity, SettingsEntity setting)
        {
            var dbSetting = GetOneFor(entity);
            if (dbSetting != null)
            {
                if (dbSetting.Id != setting.Id) 
                    throw new DataLayerException("Another setting is associated with object, shouldn't be there");
                Update(setting);
            }
            else
            {
                setting.Name = String.Format("for {0}({1})", entity.ObjectName, entity.Id);
                setting = Save(setting);
                relationService.SetRelation(entity, setting, RelationDirection.Forward, settingsRelation);
            }
        }
    }
}
