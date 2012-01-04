using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core;
using log4net;

namespace BinaryAnalysis.Scheduler.Scheduler.Data
{
    public class RecurrencyRepository : SettingsHolderRepository<RecurrencyEntity>
    {
        public RecurrencyRepository(IDbContext context, ILog log,
            SettingsService settingsService, RelationService relRepo)
            : base(context, log, settingsService, relRepo)
        {
        }

        public IList<RecurrencyEntity> GetByName(string name)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var result = AsQueryable(wu.Session)
                    .Where(x => x.TaskName == name)
                    .ToList();
                LoadSettingsIfNull(result);
                return result;
            }
        }

        public RecurrencyEntity GetExact(string cron, string name)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var result = AsQueryable(wu.Session)
                    .FirstOrDefault(x => x.TaskName == name && x.Cron == cron);
                LoadSettingsIfNull(result);
                return result;
            }
        }
    }
}
