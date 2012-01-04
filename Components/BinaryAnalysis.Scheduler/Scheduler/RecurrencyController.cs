using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using log4net;
using NCrontab;

namespace BinaryAnalysis.Scheduler.Scheduler
{
    public class RecurrencyController
    {
        private readonly ILog log;
        private readonly RecurrencyRepository repo;
        private object repoLocker = new object();
        private IList<Tuple<RecurrencyEntity, CrontabSchedule>> crontab;

        public RecurrencyController(ILog log,
            RecurrencyRepository repo)
        {
            this.log = log;
            this.repo = repo;

            var config = RecurrencyConfigSection.GetConfig();
            if (config != null)
            {
                log.Info("Reading config. Found cron items: " + config.CronTab.Count);
                foreach (RecurrencyConfigElement cron in config.CronTab)
                {
                    if(cron.Recreate)
                    {
                        var e = repo.GetExact(cron.Cron, cron.TaskName);
                        if(e!=null) repo.Delete(e);
                    }
                    AddSchedule(cron.Cron, cron.TaskName);
                }
            }
            
        }

        public void Reload()
        {
            if (crontab != null)
            {
                log.Info("Reload requested");
                lock (repoLocker)
                {
                    crontab = null;
                }
            }
        }


        public void AddSchedule(string cron, string taskName,
            Dictionary<string, object> opts = null,
            ScheduleTaskParallelism parallelism = ScheduleTaskParallelism.AllowOthers)
        {
            lock (repoLocker)
            {
                var e = repo.GetExact(cron, taskName);
                if (e == null)
                {
                    e = new RecurrencyEntity()
                            {
                                Cron = cron,
                                Parallelism = parallelism,
                                TaskName = taskName,
                            };
                    repo.Save(e);
                    repo.LoadSettingsIfNull(e);
                    if (opts != null)
                    {
                        foreach (var p in opts)
                        {
                            e.Settings.SetEntry(p.Key, p.Value);
                        }
                        repo.Update(e);
                    }
                    Reload();
                }
                else
                {
                    log.Debug("Cron: "+cron+" "+taskName+" already exist in crontab db.");
                }
            }
        }

        internal void CheckSchedules(SchedulerInstance instance)
        {
            lock (repoLocker)
            {
                if (crontab == null)
                {
                    var cronEntries = repo.GetAll();
                    crontab = cronEntries.Select(x => Tuple.Create(x, CrontabSchedule.Parse(x.Cron))).ToList();
                    log.Info(crontab.Count + " Cron elements loaded");
                }
                if (crontab.Count > 0)
                {
                    var currentTime = DateTime.Now;
                    try
                    {
                        foreach (var tuple in crontab)
                        {
                            if (tuple.Item2.GetNextOccurrence(tuple.Item1.LastSchedule) < currentTime)
                            {
                                log.Info(String.Format("Scheduling {0} ({1})",
                                    tuple.Item1.TaskName, tuple.Item1.Cron));
                                instance.Schedule(tuple.Item1.TaskName, tuple.Item1.Parallelism);

                                tuple.Item1.LastSchedule = currentTime;
                                repo.Update(tuple.Item1);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error checking schedules", ex);
                    }
                }
            }
        }
    }
}
