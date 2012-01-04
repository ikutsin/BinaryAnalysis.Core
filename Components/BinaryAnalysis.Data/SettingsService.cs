using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Classification;
using log4net;

namespace BinaryAnalysis.Data
{
    public class SettingsService
    {
        SettingsRepository settingsRepo;
        ILog log;

        public SettingsService(SettingsRepository settingsRepo, ILog log)
        {
            this.settingsRepo = settingsRepo;
            this.log = log;
        }

        public SettingsEntity GetFor(IClassifiable entity)
        {
            return settingsRepo.GetOrCreate(entity);
        }
        public void DeleteFor(IClassifiable entity)
        {
            settingsRepo.DeleteFor(entity);
        }
        public void UpdatePersistent(SettingsEntity settings)
        {
            settingsRepo.Update(settings);
        }
        public void SaveFor(IClassifiable entity, SettingsEntity settings)
        {
            if (settings != null)
            {
                settingsRepo.SaveOrUpdateFor(entity, settings);
            }
        }
    }
}
