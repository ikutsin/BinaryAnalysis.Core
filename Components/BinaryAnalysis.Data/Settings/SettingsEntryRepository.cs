using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using log4net;

namespace BinaryAnalysis.Data.Settings
{
    public class SettingsEntryRepository : Repository<SettingsEntryEntity>
    {
        public SettingsEntryRepository(IDbContext context, ILog log) : base(context, log) { }

        public SettingsEntryEntity GetEntryByKey(SettingsEntity entity, string key)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return AsQueryable(wu.Session)
                    .Where(x => x.Settings == entity && x.Name == key)
                    .FirstOrDefault();
            }
        }

    }
}
