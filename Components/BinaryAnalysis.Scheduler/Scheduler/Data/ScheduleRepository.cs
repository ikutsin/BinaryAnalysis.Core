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
    public class ScheduleRepository : SettingsHolderRepository<ScheduleEntity>
    {
        public ScheduleRepository(IDbContext context, ILog log,
            SettingsService settingsService, RelationService relRepo)
            : base(context, log, settingsService, relRepo)
        {
        }

        public void Normalize()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                //make running tasks idle
                var runningTasks = AsQueryable(wu.Session).Where(x => x.ExecutionState == ScheduleExecutionState.Running);
                runningTasks.AsParallel().ForAll(x=>x.ExecutionState=ScheduleExecutionState.Idle);
                UpdateAll(runningTasks);
                log.Debug(String.Format("{0} schedules state normalized", runningTasks.Count()));
            }
        }

        public void RemoveOldExecutables(int daysToKeep = 14)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var oldDate = DateTime.Now.AddDays(-daysToKeep);
                var runningTasks = AsQueryable(wu.Session).Where(
                    x => x.ExecutionState == ScheduleExecutionState.Finished)
                    .Where(x => x.LastExecution < oldDate);
                DeleteAll(runningTasks);
                log.Info(String.Format("{0} schedules older than {1} days removed", runningTasks.Count(), daysToKeep));
            }
        }

        public List<ScheduleEntity> GetExecutables()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                var result = AsQueryable(wu.Session)
                    .Where(x => x.ExecutionState < ScheduleExecutionState.Running &&
                                x.MessageState < ScheduleMessageState.Error)
                    .Where(x => x.NextExecution < DateTime.Now)
                    .OrderBy(x => x.NextExecution);

                LoadSettingsIfNull(result);
                log.Debug(String.Format("Found {0} schedules for running", result.Count()));
                return result.ToList();
            }
        }

        public ScheduleEntity GetByTaskName(string taskName)
        {
            var e = this.FindOne(new Dictionary<string, object>() { 
                { "TaskName", taskName }
            });
            return e;
        }
    }
}
