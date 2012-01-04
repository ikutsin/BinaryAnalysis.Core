using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using System.Threading.Tasks;
using System.Threading;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Script;
using log4net;

namespace BinaryAnalysis.Scheduler.Scheduler
{
    public delegate void OnSchedulerProgress(SchedulerInstance _this);

    public class SchedulerInstance
    {
        private const int TIMER_CHECK_INTERVAL = 10;
        protected readonly int workersAmount;
        protected TaskFactory<PersistentSchedulerTask> taskFactory;
        protected SchedulerFactoryTaskScheduler taskScheduler;
        protected Timer queueCheckTimer;

        protected List<PersistentSchedulerTask> runningTasks;
        
        public static SchedulerInstance Instance { get; protected set; }
        private readonly ILog log;
        private readonly RecurrencyController recurrencyController;
        private readonly ScheduleRepository repo;
        private object repoLocker = new object();

        private bool isFinishing = false;

        protected IContainer container;
        public SchedulerInstance(ILog log, 
            RecurrencyController recurrencyController,
            ScheduleRepository repo, int workers = 5)
        {
            if (Instance != null) throw new InvalidOperationException("Scheduler already initialized");
            workersAmount = workers;
            this.log = log;
            this.recurrencyController = recurrencyController;
            this.repo = repo;
            Instance = this;
        }
        public bool IsStarted { get; protected set; }
        public RecurrencyController RecurrencyController { get { return recurrencyController; } }

        public void Start(IContainer container)
        {
            if (IsStarted) throw new InvalidOperationException("Service already started");
            IsStarted = true;
            isFinishing = false;
            log.Info("Goal task factory started");

            taskScheduler = new SchedulerFactoryTaskScheduler(workersAmount);
            runningTasks = new List<PersistentSchedulerTask>();
            taskFactory = new TaskFactory<PersistentSchedulerTask>(taskScheduler);
            queueCheckTimer = new Timer(CheckQueue, this, TimeSpan.FromSeconds(TIMER_CHECK_INTERVAL), TimeSpan.FromSeconds(TIMER_CHECK_INTERVAL));
            this.container = container;

            lock (repoLocker)
            {
                repo.Normalize();
                repo.RemoveOldExecutables();
            }
            recurrencyController.CheckSchedules(this);

            OnFactoryStated();
        }
        public void Stop()
        {
            log.Debug("Stopping goal task factory...");
            isFinishing = true;

            queueCheckTimer.Dispose();

            while (runningTasks.Count > 0)
            {
                log.Debug(String.Format("Waiting {0} tasks to finish", runningTasks.Count));
                Thread.Sleep(500);
            }

            IsStarted = false;
            OnFactoryFinished();

            log.Info("Goal task factory stopped");
        }

        public void Destroy()
        {
            if (IsStarted) Stop();
            Instance = null;
        }

        public bool Schedule(string name, 
            ScheduleTaskParallelism parallelism = ScheduleTaskParallelism.AllowOthers)
        {
            TaskParameters tp = GetNamedTaskParameter(name);
            if (tp == null) return false;
            tp.TaskName = name;
            Schedule(tp, parallelism);
            return true;
        }

        protected TaskParameters GetNamedTaskParameter(string name)
        {
            log.Debug("Looking for tp to schedule: " + name);
            return container.ResolveNamed<TaskParameters>(name);
        }

        /// <summary>
        /// This method is only for paramteter customizing (merge with Schedule(string name, ?)
        /// TaskName should be preinitialized and point to existing named tp in container.
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="parallelism"></param>
        /// <param name="dueInSeconds"></param>
        protected void Schedule(TaskParameters tp, 
            ScheduleTaskParallelism parallelism = ScheduleTaskParallelism.AllowOthers, 
            int dueInSeconds = 0)
        {
            lock (runningTasks)
            {
                if (!IsStarted) throw new InvalidOperationException("Task scheduler is not running");
                log.Info("Scheduling "+tp);
                var entity = new ScheduleEntity()
                                 {
                                     NextExecution = DateTime.MaxValue,
                                     Parallelism = parallelism,
                                     TaskName = tp.TaskName,
                                 };
                lock (repoLocker)
                {
                    repo.Save(entity);
                    repo.LoadSettingsIfNull(entity);
                    foreach (KeyValuePair<string, object> setting in tp.Settings)
                    {
                        entity.Settings.AddEntry(setting.Key, setting.Value);
                    }
                    entity.NextExecution = DateTime.Now.AddSeconds(Math.Max(0, dueInSeconds));
                    repo.Update(entity);
                }
            }
        }

        #region Event model
        public event TaskProgress TaskStarted;
        public event TaskProgress TaskFinished;
        public event ScriptProgress ScriptStarted;
        public event ScriptProgress ScriptFinished;
        public event ScriptCustomEvent ScriptCustomEvent;

        protected void OnTaskStarted(SchedulerTask task)
        {
            if (TaskStarted != null) TaskStarted(task);
        }
        protected void OnScriptCustomEvent(SchedulerTask task, ISchedulerTaskScript script, object input)
        {
            if (ScriptCustomEvent != null) ScriptCustomEvent(task, script, input);
        }
        protected void OnTaskFinished(SchedulerTask task)
        {
            if (TaskFinished != null) TaskFinished(task);
        }
        protected virtual void OnScriptStarted(SchedulerTask task, ISchedulerTaskScript script)
        {
            if (ScriptStarted != null) ScriptStarted(task, script);
        }
        protected virtual void OnScriptFinished(SchedulerTask task, ISchedulerTaskScript script)
        {
            if (ScriptFinished != null) ScriptFinished(task, script);
        }

        public event OnSchedulerProgress FactoryStarted;
        public event OnSchedulerProgress FactoryFinished;

        protected virtual void OnFactoryStated()
        {
            if (FactoryStarted != null) FactoryStarted(this);
        }
        protected virtual void OnFactoryFinished()
        {
            if (FactoryFinished != null) FactoryFinished(this);
        }
        #endregion

        bool TimerIsLocked = false;
        object TimerLocker = new object();
        protected void CheckQueue(object state)
        {
            if (isFinishing) return;
            if (TimerIsLocked) return;
            lock (TimerLocker)
            {
                if(TimerIsLocked) return;
                TimerIsLocked = true;

                lock (runningTasks)
                {

                    recurrencyController.CheckSchedules(this);

                    if (taskScheduler.IsQueueEmpty)
                    {
                        //ScheduleNext
                        var entities = repo.GetExecutables();
                        if (entities.Count > 0)
                        {

                            if (entities.First().Parallelism == ScheduleTaskParallelism.Standalone)
                            {
                                if (runningTasks.Count == 0)
                                {
                                    StartTask(entities.First());
                                }
                                else
                                {
                                    log.Debug("Waiting tasks to run standalone " + entities.First());
                                    return; //waiting for free queue
                                }
                            }

                            var names = runningTasks.Select(x => x.TaskName).ToList();
                            foreach (var entity in entities)
                            {
                                switch (entity.Parallelism)
                                {
                                    case ScheduleTaskParallelism.AllowAll:
                                        StartTask(entity);
                                        names.Add(entity.TaskName);
                                        break;
                                    case ScheduleTaskParallelism.AllowOthers:
                                        if (!names.Contains(entity.TaskName))
                                        {
                                            StartTask(entity);
                                            names.Add(entity.TaskName);
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            log.Debug("Nothing to execute");
                        }
                    }
                }
                TimerIsLocked = false;
            }
        }

        protected void StartTask(ScheduleEntity entity)
        {
            if (isFinishing) return;

            lock (runningTasks)
            {
                entity.ExecutionState = ScheduleExecutionState.Running;
                lock (repoLocker) repo.Update(entity);

                var task = container.Resolve<PersistentSchedulerTask>();
                task.ScriptFinished +=
                    (t, s) =>
                        {
                            var e = (t as PersistentSchedulerTask).Entity;
                            e.LastExecution = DateTime.Now;
                            e.MessageState = s.Flow.MessagesState;
                            if (t.ScheduledScripts.Count == 0 || s.Flow.MessagesState >= ScheduleMessageState.Error)
                            {
                                e.ExecutionState = ScheduleExecutionState.Finished;
                            }
                            else
                            {
                                e.NextExecution = t.ScheduledScripts.First().Date;
                                e.ExecutionState = ScheduleExecutionState.Idle;
                            }
                            //settings should be stored automatically
                            lock (repoLocker) repo.Update(entity);
                        };

                task.TaskName = entity.TaskName;
                task.Entity = entity;
                task.Settings = new EntityTaskSettings(entity.Settings);

                task.Scripts = GetNamedTaskParameter(task.TaskName).Scripts;
                StartTask(task);
            }
        }

        protected void StartTask(PersistentSchedulerTask schedule)
        {
            //run taks
            lock(runningTasks) {

                log.Debug("Creating task queue for " + schedule);

                var task = taskFactory.StartNew((s) => {
                    var ss = s as PersistentSchedulerTask;
                    ss.ScriptFinished += OnScriptFinished;
                    ss.ScriptStarted += OnScriptStarted;
                    ss.ScriptCustomEvent += OnScriptCustomEvent;
                    ss.TaskFinished += OnTaskFinished;
                    ss.TaskStarted += OnTaskStarted;
                    log.Debug("Starting: " + ss);
                    ss.Execute(container);
                    return ss; 
                },schedule)
                .ContinueWith((s) =>
                {
                    try
                    {
                        var ss = s.Result as PersistentSchedulerTask;
                        ss.ScriptFinished -= OnScriptFinished;
                        ss.ScriptStarted -= OnScriptStarted;
                        ss.TaskFinished -= OnTaskFinished;
                        ss.ScriptCustomEvent -= OnScriptCustomEvent;
                        ss.TaskStarted -= OnTaskStarted;
                        runningTasks.Remove(ss);
                        log.Debug("Finished: " + ss);
                        CheckQueue(this);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Unhandler error in schedule task", ex);
                    }
                });
                runningTasks.Add(schedule);
            }
        }

        
    }
}
